using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Output;
using IDesign.Recognizers.Models;
using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Checks;

namespace IDesign.Recognizers
{
    public class DecoratorRecognizer : Recognizer, IRecognizer
    {
        public IResult Recognize(IEntityNode entityNode)
        {
            Result result = new Result();

            var classChecks = new List<ElementCheck<IEntityNode>>()
            {
                new ElementCheck<IEntityNode>(x => x.CheckEntityNodeModifier("Abstract"), "De class mag geen interface zijn"),
                new ElementCheck<IEntityNode>(x => x.CheckRelationType(RelationType.Extends) || x.CheckRelationType(RelationType.Implements), "De class zit niet onder een interface of parent class"),
                new ElementCheck<IEntityNode>(x => x.CheckRelationType(RelationType.ExtendedBy), "De class wordt niet extend door een ander class")
            };
            CheckElements(result, new List<IEntityNode>() { entityNode }, classChecks);

            var test = entityNode.GetMethods();

            var constructorChecks = new List<ElementCheck<IMethod>>()
            {   
                new ElementCheck<IMethod>(x => x.CheckModifier("public") || x.CheckModifier("protected") , "Constructor moet public of protected zijn"),
                new ElementCheck<IMethod>(x => x.CheckParameters(entityNode.GetRelations().Where(y => y.GetRelationType() == RelationType.Implements)
                .Select(y => y.GetDestination().GetName()).ToList()), "Jeanrisotto")
            };
            CheckElements(result, entityNode.GetConstructors(), constructorChecks);

            var parentClass = entityNode.GetRelations().Where(x => x.GetRelationType().Equals(RelationType.Implements) || x.GetRelationType().Equals(RelationType.Extends));
            if (parentClass.Count() > 0 && result.Score > 3)
                result.Score += parentClass.Select(x => CheckInterface(x.GetDestination(), entityNode.GetMethods(), entityNode)).Max(x => x.GetScore());

            result.Score = (int)((result.Score) / 13f * 100f);

            return result;
        }

        public IResult CheckInterface(IEntityNode interfaceNode, IEnumerable<IMethod> methods, IEntityNode core)
        {
            Result result = new Result();

            var methodChecks = new List<ElementCheck<IMethod>>()
            {
                new ElementCheck<IMethod>(x => x.CheckName(methods) , "De interface is leeg")
            };
            CheckElements(result, interfaceNode.GetMethods(), methodChecks);

            var classChecks = new List<ElementCheck<IEntityNode>>()
            {
                 new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfRelationTypes(RelationType.ExtendedBy, 2) || x.CheckMinimalAmountOfRelationTypes(RelationType.ImplementedBy, 2), "De class wordt niet extended of geimplementeerd door een andere class")
            };

            CheckElements(result, new List<IEntityNode>() { interfaceNode }, classChecks);

            var extendsCore = core.GetRelations().Where(x => x.GetRelationType().Equals(RelationType.ExtendedBy));
            extendsCore.Select(x => CheckConcreteDecorator(x.GetDestination(), interfaceNode)).ToList().ForEach(x => result.Score += x.GetScore());

            return result;
        }

        public IResult CheckConcreteDecorator(IEntityNode concreteNode, IEntityNode interfaceNode)
        {
            Result result = new Result();

            var constructorChecks = new List<ElementCheck<IMethod>>()
            {
                 new ElementCheck<IMethod>(x => x.CheckModifier("public"), "Constructor moet public of protected zijn"),
                 new ElementCheck<IMethod>(x => x.CheckParameters(new List<string>() { interfaceNode.GetName() }), "Jeanrisotto"),
                 new ElementCheck<IMethod>(x => x.CheckArguments(interfaceNode.GetName()), "daddy")
            };
            CheckElements(result, concreteNode.GetConstructors(), constructorChecks);

            return result;
        }
    }
}
