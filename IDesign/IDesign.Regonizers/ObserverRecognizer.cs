using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Output;
using IDesign.Recognizers.Checks;

namespace IDesign.Recognizers
{
    public class ObserverRecognizer : Recognizer, IRecognizer
    {
        Result result;

        public IResult Recognize(IEntityNode entityNode)
        {
            result = new Result();

            var usesRelations = entityNode.GetRelations().Where(x => x.GetRelationType() == RelationType.Uses).ToList();

            if (entityNode.GetEntityNodeType() == Models.EntityNodeType.Interface)
                result = (Result)ObserverCheck(entityNode);
            else
                result.Suggestions.Add(new Suggestion("No observer interface found", entityNode.GetSuggestionNode()));

            result.Score = (int)(result.Score / 7f * 100f);

            return result;
        }

        /// <summary>
        ///     Checks if a given node is an observer interface
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IResult ObserverCheck(IEntityNode node)
        {
            var entityNodeChecks = new List<ElementCheck<IEntityNode>>
            {
                new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfMethods(1), "There should be atleast 1 method: update"),
                new ElementCheck<IEntityNode>(x => x.CheckRelationType(RelationType.ImplementedBy), "There are no concrete observers"),
                new ElementCheck<IEntityNode>(x => x.CheckRelationType(RelationType.UsedBy), "This class is not used by anything"),
            };

            CheckElements(result, new List<IEntityNode> { node }, entityNodeChecks);

            var usedBy = node.GetRelations().Where(x => x.GetRelationType() == RelationType.UsedBy).ToList();

            if (usedBy.Count() > 0 && result.Score > 0)
                result.Score += usedBy.Select(x => SubjectCheck(x.GetDestination(), node)).Max(x => x.GetScore());
            else
                result.Score = 0;

            return result;
        }

        /// <summary>
        ///     Checks if a given node is a subject 
        /// </summary>
        /// <param name="subjectNode"></param>
        /// <param name="observerNode"></param>
        /// <returns></returns>
        private IResult SubjectCheck(IEntityNode subjectNode, IEntityNode observerNode)
        {
            var _result = new Result();

            var entityNodeChecks = new List<ElementCheck<IEntityNode>>
            {
                new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfMethods(3), "There should be atleast 3 methods: add, remove and notify"),
                new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfMethodsWithParameter(new List<string> { observerNode.GetName() }, 2), "There should be 2 methods which both have one of the same parameters: add(IObserver), remove(IObserver)"),
                new ElementCheck<IEntityNode>(x => x.CheckRelationType(RelationType.ImplementedBy), "This class isn't implemented by any other classes"),
            };

            CheckElements(_result, new List<IEntityNode> { subjectNode }, entityNodeChecks);

            if (subjectNode.GetEntityNodeType() == Models.EntityNodeType.Class)
            {
                _result.Score += ConcreteSubjectCheck(subjectNode, observerNode).GetScore();
            }
            else
            {
                var concreteSubjects = subjectNode.GetRelations().Where(x => x.GetRelationType() == RelationType.ImplementedBy).Select(x => x.GetDestination());

                if (concreteSubjects.Count() > 0)
                    _result.Score += (int)concreteSubjects.Select(x => ConcreteSubjectCheck(x, observerNode).GetScore()).Sum() / concreteSubjects.Count();
            }

            return _result;
        }

        /// <summary>
        ///     Checks if a given node is a concrete subject 
        /// </summary>
        /// <param name="subjectNode"></param>
        /// <param name="observerNode"></param>
        /// <returns></returns>
        private IResult ConcreteSubjectCheck(IEntityNode subjectNode, IEntityNode observerNode)
        {
            var _result = new Result();

            var fieldChecks = new List<ElementCheck<IField>>
            {
                new ElementCheck<IField>(x => x.CheckFieldType("List<" + observerNode.GetName() + ">"), $"There should be a 1 list of type: {observerNode.GetName()}"),
            };

            CheckElements(_result, subjectNode.GetFields(), fieldChecks);

            return _result;
        }
    }
}
