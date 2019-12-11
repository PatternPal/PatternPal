using System.Collections.Generic;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Output;
using IDesign.Recognizers.Checks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace IDesign.Recognizers
{
    public class FactoryRecognizer : Recognizer, IRecognizer
    {
        public IResult Recognize(IEntityNode entityNode)
        {
            var result = new Result();
            var methodChecks = new List<ElementCheck<IMethod>>()
            {
                new ElementCheck<IMethod>(x => CreatedClassExtendsReturnTypeInterface(entityNode, x)||
                CreatedClassImplementsReturnTypeInterface(entityNode, x),
                "Return type is niet hetzelfde als wat er gemaakt wordt" )
            };
            CheckElements(result, entityNode.GetMethods(), methodChecks);
            if (result.GetSuggestions() != null)
            {
                var BestMethod = new Method(result.GetSuggestions().First().GetSyntaxNode() as MethodDeclarationSyntax);
                var classChecks = new List<ElementCheck<IEntityNode>>()
            {
                new ElementCheck<IEntityNode>(x => x.ClassImlementsInterface(BestMethod),
                "Method isnt implemented in a interface")
            };
                 var entityList = new List<IEntityNode>();
                entityList.Add(entityNode);
                CheckElements(result,entityList , classChecks);
            }
            result.Score = (int)(result.Score / 2f * 100f);
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
