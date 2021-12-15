using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers.Recognizers
{
    public class ObserverRecognizer : IRecognizer
    {
        public IResult Recognize(IEntity entityNode)
        {
            var result1 = ObserverWithSubjectInterfaceCheck(entityNode);
            var result2 = ObserverWithoutSubjectInterfaceCheck(entityNode);

            return result1.GetScore() >= result2.GetScore() ? result1 : result2;
        }

        /// <summary>
        ///     Checks if a given node is an implementation of observer pattern with an subject interface
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IResult ObserverWithSubjectInterfaceCheck(IEntity node)
        {
            var result = new Result();

            //Tests for implementation of observer pattern with an subject interface
            var checks = new GroupCheck<IEntity, IEntity>(
                new List<ICheck<IEntity>>
                {
                    new ElementCheck<IEntity>(x => x.CheckMinimalAmountOfMethods(1), "MethodAny", 0.33f),
                    new ElementCheck<IEntity>(x => x.CheckRelationType(RelationType.ImplementedBy),
                        "NodeImplementedByAny", 0.33f),
                    new ElementCheck<IEntity>(x => x.CheckRelationType(RelationType.UsedBy), "NodeUsedByAny",
                        0.33f),
                    new GroupCheck<IEntity, IEntity>(
                        new List<ICheck<IEntity>>
                        {
                            new ElementCheck<IEntity>(x => x.CheckMinimalAmountOfMethods(3),
                                "MethodAmountThree", 0.66f),
                            new ElementCheck<IEntity>(
                                x => x.CheckMinimalAmountOfMethodsWithParameter(
                                    new List<string> {node.GetName()}, 2),
                                new ResourceMessage("ObserverMethodParameters", new[] {node.GetName()}), 1f),
                            new ElementCheck<IEntity>(x => x.CheckRelationType(RelationType.ImplementedBy),
                                "NodeImplementedByAny", 0.33f),
                            new GroupCheck<IEntity, IEntity>(
                                new List<ICheck<IEntity>>
                                {
                                    new ElementCheck<IEntity>(x => x.CheckTypeDeclaration(EntityType.Class),
                                        "NodeTypeClass"),
                                    new GroupCheck<IEntity, IField>(
                                        new List<ICheck<IField>>
                                        {
                                            new ElementCheck<IField>(
                                                x => x.CheckFieldType(new List<string> {$"List<{node.GetName()}>"}),
                                                new ResourceMessage("FieldType",
                                                    new[] {node.GetName()}), 1f)
                                        }, d => d.GetFields(), "ObserverConcreteSubjectField")
                                },
                                c => c.GetRelations()
                                    .Where(x => x.GetRelationType() == RelationType.ImplementedBy)
                                    .Select(x => x.GetDestination()), "ObserverConcreteSubject",
                                GroupCheckType.Median)
                        },
                        b => node.GetRelations().Where(x => x.GetRelationType() == RelationType.UsedBy)
                            .Select(x => x.GetDestination()).ToList(), "ObserverSubjectInterface")
                }, a => new List<IEntity> {node}, "Observer");

            result.Results.Add(checks.Check(node));
            result.RelatedSubTypes.Add(node, "Observer");
            foreach (var concrete in node.GetRelations().Where(x =>
                         x.GetRelationType().Equals(RelationType.ImplementedBy)
                     ).Select(x => x.GetDestination()))
            {
                result.RelatedSubTypes.Add(concrete, "ConcreteObserver");
            }

            return result;
        }

        /// <summary>
        ///     Checks if a given node is an implementation of observer pattern without an subject interface
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IResult ObserverWithoutSubjectInterfaceCheck(IEntity node)
        {
            var result = new Result();

            //Tests for implementation of observer pattern without an subject interface
            var checks = new GroupCheck<IEntity, IEntity>(
                new List<ICheck<IEntity>>
                {
                    new ElementCheck<IEntity>(x => x.CheckMinimalAmountOfMethods(1), "MethodAny", 0.33f),
                    new ElementCheck<IEntity>(x => x.CheckRelationType(RelationType.ImplementedBy),
                        "NodeImplementedByAny", 0.33f),
                    new ElementCheck<IEntity>(x => x.CheckRelationType(RelationType.UsedBy), "NodeUsedByAny",
                        0.33f),
                    new GroupCheck<IEntity, IEntity>(
                        new List<ICheck<IEntity>>
                        {
                            new ElementCheck<IEntity>(x => x.CheckMinimalAmountOfMethods(3),
                                "MethodAmountThree", 0.33f),
                            new ElementCheck<IEntity>(
                                x => x.CheckMinimalAmountOfMethodsWithParameter(
                                    new List<string> {node.GetName()}, 2),
                                new ResourceMessage("ObserverMethodParameters", new[] {node.GetName()}), 1f),
                            new ElementCheck<IEntity>(x => x.CheckEntityType(EntityType.Class), "NodeTypeClass",
                                0.33f),
                            new ElementCheck<IEntity>(
                                x => x.CheckMaximumAmountOfRelationTypes(RelationType.Implements, 0),
                                "NodeNotImplementedAny", 0.33f),
                            new GroupCheck<IEntity, IField>(
                                new List<ICheck<IField>>
                                {
                                    new ElementCheck<IField>(
                                        x => x.CheckFieldType(new List<string> {$"List<{node.GetName()}>"}),
                                        new ResourceMessage("FieldType", new[] {node.GetName()}), 1f)
                                }, d => d.GetFields(), "ObserverConcreteSubjectField")
                        },
                        b => node.GetRelations().Where(x => x.GetRelationType() == RelationType.UsedBy)
                            .Select(x => x.GetDestination()).ToList(), "Concrete subject")
                }, a => new List<IEntity> {node}, "Observer");

            result.Results.Add(checks.Check(node));


            result.Results.Add(checks.Check(node));
            result.RelatedSubTypes.Add(node, "Observer");
            foreach (var concrete in node.GetRelations().Where(x =>
                         x.GetRelationType().Equals(RelationType.ImplementedBy)
                     ).Select(x => x.GetDestination()))
            {
                result.RelatedSubTypes.Add(concrete, "ConcreteObserver");
            }

            return result;
        }
    }
}
