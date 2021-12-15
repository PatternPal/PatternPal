using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;
using System.Collections.Generic;
using System.Linq;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers
{
    /// <summary>
    ///     This class contains the logic of the Bridge Pattern Recognizer.
    /// </summary>
    public class BridgeRecognizer : IRecognizer
    {
        public IResult Recognize(IEntity entityNode)
        {
            Result result = new Result();
            List<ICheck<IEntity>> implementerChecks = new List<ICheck<IEntity>>
            {
                // Implementer is an abstract class or an interface
                new ElementCheck<IEntity>(
                    x => x.CheckIsAbstractClassOrInterface(),
                    "ImplementerAbstractOrInterface",
                    3.25f
                ),

                // Implementer is extended or implemented by other classes
                new ElementCheck<IEntity>(
                    x => x.CheckRelationType(RelationType.ImplementedBy) ||
                         x.CheckRelationType(RelationType.ExtendedBy),
                    "NodeImplementedOrInherited",
                    3.25f
                ),

                // Implementer is used by another node
                new ElementCheck<IEntity>(
                    x => x.CheckRelationType(RelationType.UsedBy),
                    "ImplementerUsedByAbstraction",
                    2f
                ),
            };

            var implementerGroupCheck = new GroupCheck<IEntity, IEntity>(
                implementerChecks,
                x => entityNode
                        .GetRelations()
                        .Where(y => y.GetRelationType().Equals(RelationType.Uses))
                        .Select(y => y.GetDestination()),
                "BridgeImplementer"
            );

            var abstractionGroupCheckList = new List<ICheck<IEntity>>
            {
                // The abstraction should be an abstract class or an interface
                new ElementCheck<IEntity>(
                    x => x.CheckIsAbstractClassOrInterface(),
                    "NodeAbstractOrInterface",
                    0.5f
                ),

                // Abstraction should be inherited or implemented
                new ElementCheck<IEntity>(
                    x => x.CheckRelationType(RelationType.ExtendedBy) ||
                         x.CheckRelationType(RelationType.ImplementedBy),
                    "NodeImplementedOrInherited",
                    6f
                ),

                // Abstraction uses at least one other node
                new ElementCheck<IEntity>(
                    x => x.CheckMinimalAmountOfRelationTypes(RelationType.Uses, 1),
                    "NodeUses1",
                    3f
                ),
            };

            var abstractionAsClassGroupcheckList = GetAbstractionGroupCheckList(
                entityNode, 
                entityNode.GetFields()
            );

            var abstractionAsInterfaceGroupcheckList = GetAbstractionGroupCheckList(
                entityNode,
                (entityNode?
                    .GetRelations())
                .FirstOrDefault(y => y.GetRelationType().Equals(RelationType.ImplementedBy))?
                    .GetDestination()
                    .GetFields()
            );

            if (entityNode.CheckEntityType(EntityType.Interface))
            {
                abstractionGroupCheckList.AddRange(abstractionAsInterfaceGroupcheckList);
            }
            else
            {
                abstractionGroupCheckList.AddRange(abstractionAsClassGroupcheckList);
            }

            GroupCheck<IEntity, IEntity> abstractionGroupCheck =
                new GroupCheck<IEntity, IEntity>(
                    abstractionGroupCheckList,
                    x => new List<IEntity> { entityNode },
                    "BridgeAbstraction"
                );

            var bridgeGroupCheck = new GroupCheck<IEntity, IEntity>(
                new List<ICheck<IEntity>>
                {
                    abstractionGroupCheck,
                    implementerGroupCheck
                },
                x => new List<IEntity> { entityNode },
                "Bridge"
            );

            result.Results.Add(bridgeGroupCheck.Check(entityNode));

            return result;
        }

        public List<ICheck<IEntity>> GetAbstractionGroupCheckList(IEntity entityNode, IEnumerable<IField> fieldsToCheck)
        {
            IRelation currentRelation = null;
            string implementerField = null;

            return new List<ICheck<IEntity>>()
            {
                new GroupCheck<IEntity, IRelation>(new List<ICheck<IRelation>>
                {
                    // Abstraction uses the Implementer
                    new ElementCheck<IRelation>(
                        x =>
                        {
                            currentRelation = x;
                            return x.GetRelationType() == RelationType.Uses;
                        },
                        "AbstractionUsesImplementer",
                        2f
                    ),

                    // Abstraction has a reference to the implementer
                    new GroupCheck<IRelation, IField>(new List<ICheck<IField>>
                        {
                            new ElementCheck<IField>(
                                x =>
                                {
                                    implementerField = x.GetName();
                                    return x.CheckFieldType(new List<string>{ currentRelation.GetDestination().GetName() });
                                },
                                "AbstractionHasImplementerReference",
                                7f
                            ),

                            new GroupCheck<IField, IMethod>(new List<ICheck<IMethod>>
                                {
                                    new ElementCheck<IMethod>(
                                        x =>
                                        {
                                            return x.CheckFieldIsUsed(implementerField);
                                        },
                                        "ImplementerMethodsUsedInAbstraction",
                                        5f
                                    ),
                                },
                                x => entityNode.GetAllMethods(),
                                "ImplementerMethods"
                            ),
                        },
                        x => fieldsToCheck,
                        "ImplementerReference"
                    )

                }, x => entityNode.GetRelations(), "Abstraction")
            };
        }
    }
}
    
