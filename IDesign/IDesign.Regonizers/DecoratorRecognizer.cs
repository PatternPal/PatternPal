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
                new ElementCheck<IEntityNode>(x => x.CheckEntityNodeModifier("Abstract"), "De class moet abstract zijn"),
                new ElementCheck<IEntityNode>(x => x.CheckRelationType(RelationType.Extends) || x.CheckRelationType(RelationType.Implements), "De class zit niet onder een interface of parent class"),
                new ElementCheck<IEntityNode>(x => x.CheckRelationType(RelationType.ExtendedBy), "De class wordt niet extend door een ander class")
            };
            CheckElements(result, new List<IEntityNode>() { entityNode }, classChecks);

            var component = entityNode.GetRelations().Where(x => x.GetRelationType().Equals(RelationType.Implements) || x.GetRelationType().Equals(RelationType.Extends));
            var constructorChecks = new List<ElementCheck<IMethod>>()
            {   
                new ElementCheck<IMethod>(x => x.CheckModifier("public") || x.CheckModifier("protected") , "De constructor moet public of protected zijn"),
                new ElementCheck<IMethod>(x => x.CheckParameters(component.Select(y => y.GetDestination().GetName()).ToList()), "De constructor moet de interface of parent als parameter hebben")
            };
            CheckElements(result, entityNode.GetConstructors(), constructorChecks);

            var propertyChecks = new List<ElementCheck<IField>>
            {
               new ElementCheck<IField>(x => x.CheckFieldType(component.Select(y => y.GetDestination().GetName()).ToList()) , "Incorrect type")
            };
            CheckElements(result, entityNode.GetFields(), propertyChecks);

            if (component.Count() > 0 && result.Score > 3)
                result.Score += component.Select(x => CheckComponent(x.GetDestination(), entityNode.GetMethods(), entityNode)).Max(x => x.GetScore());

            result.Score = (int)((result.Score) / 15f * 100f);
            return result;
        }

        public IResult CheckComponent(IEntityNode componentNode, IEnumerable<IMethod> methods, IEntityNode decoratorNode)
        {
            Result result = new Result();

            var methodChecks = new List<ElementCheck<IMethod>>()
            {
                new ElementCheck<IMethod>(x => x.CheckName(methods) , "De interface is leeg")
            };
            CheckElements(result, componentNode.GetMethods(), methodChecks);

            var classChecks = new List<ElementCheck<IEntityNode>>()
            {
                 new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfRelationTypes(RelationType.ExtendedBy, 2) || x.CheckMinimalAmountOfRelationTypes(RelationType.ImplementedBy, 2), "De class wordt niet extended of geimplementeerd door een andere class")
            };

            CheckElements(result, new List<IEntityNode>() { componentNode }, classChecks);

            var concreteComponent = componentNode.GetRelations().Where(x => (x.GetRelationType().Equals(RelationType.ExtendedBy) || x.GetRelationType().Equals(RelationType.ImplementedBy)) && !x.GetDestination().Equals(decoratorNode)).Select(x => x.GetDestination());
            concreteComponent.Where(x => x.GetConstructors().Count() > 0).ToList().ForEach(x => result.Score++);

            var concreteDecorator = decoratorNode.GetRelations().Where(x => x.GetRelationType().Equals(RelationType.ExtendedBy));
            concreteDecorator.Select(x => CheckConcreteDecorator(x.GetDestination(), componentNode)).ToList().ForEach(x => result.Score += x.GetScore());

            return result;
        }

        public IResult CheckConcreteDecorator(IEntityNode concreteDecoratorNode, IEntityNode componentNode)
        {
            Result result = new Result();

            var constructorChecks = new List<ElementCheck<IMethod>>()
            {
                 new ElementCheck<IMethod>(x => x.CheckModifier("public"), "Constructor moet public of protected zijn"),
                 new ElementCheck<IMethod>(x => x.CheckParameters(new List<string>() { componentNode.GetName() }), "De class moet de component als parameter in de constructor hebben"),
                 new ElementCheck<IMethod>(x => x.CheckArguments(componentNode.GetName()), "De class moet de component als argument in de constructor hebben")
            };
            CheckElements(result, concreteDecoratorNode.GetConstructors(), constructorChecks);

            return result;
        }
    }
}
