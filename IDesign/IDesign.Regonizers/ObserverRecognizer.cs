using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models.Output;
using IDesign.Recognizers.Models;
using IDesign.Recognizers.Models.ElementChecks;

namespace IDesign.Recognizers
{
    public class ObserverRecognizer : IRecognizer
    {
        public IResult Recognize(IEntityNode entityNode)
        {
            var result1 = ObserverWithSubjectInterfaceCheck(entityNode);
            var result2 = ObserverWithoutSubjectInterfaceCheck(entityNode);

            if (result1.GetScore() >= result2.GetScore())
                return result1;
            else 
                return result2;
        }

        /// <summary>
        ///     Checks if a given node is an implementation of observer pattern with an subject interface
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IResult ObserverWithSubjectInterfaceCheck(IEntityNode node)
        {
            var result = new Result();

            //Tests for implementation of observer pattern with an subject interface
            var checks = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfMethods(1), "There should be atleast 1 method: update"),
                new ElementCheck<IEntityNode>(x => x.CheckRelationType(RelationType.ImplementedBy), "There are no concrete observers"),
                new ElementCheck<IEntityNode>(x => x.CheckRelationType(RelationType.UsedBy), "This class is not used by anything"),

                new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                {
                    new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfMethods(3), "There should be atleast 3 methods: add, remove and notify"),
                    new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfMethodsWithParameter(new List<string> { node.GetName() }, 2), "There should be 2 methods which both have one of the same parameters: add(IObserver), remove(IObserver)"),
                    new ElementCheck<IEntityNode>(x => x.CheckRelationType(RelationType.ImplementedBy), "This class isn't implemented by any other classes"),

                    new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                    {
                        new ElementCheck<IEntityNode>(x => x.CheckTypeDeclaration(EntityNodeType.Class), $"Should be class"),
                        new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfMethods(3), "There should be atleast 3 methods: add, remove and notify"),

                        new GroupCheck<IEntityNode, IField>(new List<ICheck<IField>>
                        {
                            new ElementCheck<IField>(x => x.CheckFieldType(new List<string> { $"List<{ node.GetName() }>" }), $"There should be a 1 list of type: { node.GetName() }"),
                        }, d => d.GetFields(), "concrete field", GroupCheckType.All),
                    }, c => c.GetRelations().Where(x => x.GetRelationType() == RelationType.ImplementedBy).Select(x => x.GetDestination()), "concrete subject", GroupCheckType.All),
                }, b => node.GetRelations().Where(x => x.GetRelationType() == RelationType.UsedBy).Select(x => x.GetDestination()).ToList(), "subject interface"),
            }, a => new List<IEntityNode> { node }, "observer");

            result.Results.Add(checks.Check(node));

            return result;
        }

        /// <summary>
        ///     Checks if a given node is an implementation of observer pattern without an subject interface
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IResult ObserverWithoutSubjectInterfaceCheck(IEntityNode node)
        {
            var result = new Result();
            
            //Tests for implementation of observer pattern without an subject interface
            var checks = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfMethods(1), "There should be atleast 1 method: update"),
                new ElementCheck<IEntityNode>(x => x.CheckRelationType(RelationType.ImplementedBy), "There are no concrete observers"),
                new ElementCheck<IEntityNode>(x => x.CheckRelationType(RelationType.UsedBy), "This class is not used by anything"),

                new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                {
                    new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfMethods(3), "There should be atleast 3 methods: add, remove and notify"),
                    new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfMethodsWithParameter(new List<string> { node.GetName() }, 2), $"There should be 2 methods which both have one of the same parameters: add({ node.GetName() }), remove({ node.GetName() })"),
                    new ElementCheck<IEntityNode>(x => x.CheckEntityNodeType(EntityNodeType.Class), "Concrete subject should be a class"),

                    new GroupCheck<IEntityNode, IField>(new List<ICheck<IField>>
                    {
                        new ElementCheck<IField>(x => x.CheckFieldType(new List<string> { $"List<{ node.GetName() }>" }), $"There should be a 1 list of type: { node.GetName() }"),
                    }, d => d.GetFields(), "concrete subject fields"),

                    //Check to make sure a subject with an interface doesn't get mistaken for a concrete subject without an subject interface
                    new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                    {
                        new ElementCheck<IEntityNode>(x => x.CheckMaximumAmountOfMethodsWithParameter(new List<string> { node.GetName() }, 0), "Implementations of observer without an subject interface should not have an subject interface"),
                    }, c => c.GetRelations().Where(x => x.GetRelationType() == RelationType.ImplementedBy).Select(x => x.GetDestination()).ToList(), "subject interface"),
                }, b => node.GetRelations().Where(x => x.GetRelationType() == RelationType.UsedBy).Select(x => x.GetDestination()).ToList(), "concrete subject"),
            }, a => new List<IEntityNode> { node }, "observer");

            result.Results.Add(checks.Check(node));

            return result;
        }
    }
}
