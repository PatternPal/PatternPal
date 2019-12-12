using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Recognizers
{
    public class FactoryRecognizer : IRecognizer
    {
        public IResult Recognize(IEntityNode entityNode)
        {
            var result = new Result();
            var methodChecks = new List<ICheck<IMethod>>
            {

                new ElementCheck<IMethod>(x => CreatedClassExtendsReturnTypeInterface(entityNode, x)||
                CreatedClassImplementsReturnTypeInterface(entityNode, x),
                "Return type is not the same as created" ),
                  new ElementCheck<IMethod>(x => x.CheckModifier("public"),
                "Method isnt public" ),
                  new ElementCheck<IMethod>(x => x.IsInterfaceMethod(entityNode),
                  "Method is not implemented in a interface")
            };

            var factoryCheck = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                new GroupCheck<IEntityNode, IMethod>(methodChecks, x => x.GetMethods(), "Has Create()")
            }, x => new List<IEntityNode> { entityNode }, "Factory", GroupCheckType.All);
            var r = factoryCheck.Check(entityNode);

            result.Results = r.GetChildFeedback().ToList();
            return result;
        }

        private bool CreatedClassImplementsReturnTypeInterface(IEntityNode node, IMethod method)
        {
            return method.GetCreatedTypes()
                 .Where(name => node.GetEdgeNode(name).ImplementsInterface(method.GetReturnType())).Any();
        }

        private bool CreatedClassExtendsReturnTypeInterface(IEntityNode node, IMethod method)
        {
            return method.GetCreatedTypes()
                .Where(name => node.GetEdgeNode(name).ExtendsClass(method.GetReturnType())).Any();
        }

    }
}
