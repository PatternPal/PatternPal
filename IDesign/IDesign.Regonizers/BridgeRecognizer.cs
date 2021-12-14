using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDesign.Recognizers
{
    /// <summary>
    ///     This class contains the logic of the Bridge Pattern Recognizer.
    /// </summary>
    public class BridgeRecognizer : IRecognizer
    {
        public IResult Recognize(IEntityNode entityNode)
        {
            Result result = new Result();
            IRelation currentRelation = null;
            string implementerField = null;

            List<ICheck<IEntityNode>> implementerChecks = new List<ICheck<IEntityNode>>
            {
                // Implementer is an abstract class or an interface
                new ElementCheck<IEntityNode>(
                    x => x.CheckIsAbstractClassOrInterface(),
                    "ImplementerAbstractOrInterface",
                    3.25f
                ),

                // Implementer is extended or implemented by other classes
                new ElementCheck<IEntityNode>(
                    x => x.CheckRelationType(RelationType.ImplementedBy) ||
                         x.CheckRelationType(RelationType.ExtendedBy),
                    "NodeImplementedOrInherited",
                    3.25f
                ),

                // Implementer is used by another node
                new ElementCheck<IEntityNode>(
                    x => x.CheckRelationType(RelationType.UsedBy),
                    "ImplementerUsedByAbstraction",
                    2f
                ),
            };

            var implementerGroupCheck = new GroupCheck<IEntityNode, IEntityNode>(
                implementerChecks,
                x => entityNode.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.Uses)).Select(y => y.GetDestination()),
                "BridgeImplementer"
            );

            GroupCheck<IEntityNode, IEntityNode> abstractionGroupCheck = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                // The abstraction should be an abstract class
                new ElementCheck<IEntityNode>(x => x.CheckIsAbstractClass(), "NodeModifierAbstract", 0.5f),

                // Abstraction should be inherited
                new ElementCheck<IEntityNode>(
                    x => x.CheckRelationType(RelationType.ExtendedBy),
                    "NodeInherited",
                    6f
                ),

                // Abstraction uses at least one other node
                new ElementCheck<IEntityNode>(
                    x => x.CheckMinimalAmountOfRelationTypes(RelationType.Uses, 1),
                    "NodeUses1",
                    3f
                ),

                new GroupCheck<IEntityNode, IRelation>(new List<ICheck<IRelation>>
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
                                x => x.CheckFieldIsUsed(implementerField),
                                "ImplementerMethodsUsedInAbstraction",
                                5f
                            ),

                        }, x => entityNode.GetMethodsAndProperties(), "ImplementerMethods")

                    }, x => entityNode.GetFields(), "ImplementerReference")

                }, x => entityNode.GetRelations(), "Abstraction")

            }, x => new List<IEntityNode> { entityNode }, "BridgeAbstraction");

            var bridgeGroupCheck = new GroupCheck<IEntityNode, IEntityNode>(
                new List<ICheck<IEntityNode>>
                {
                    abstractionGroupCheck,
                    implementerGroupCheck
                },
                x => new List<IEntityNode> { entityNode },
                "Bridge"
            );

            result.Results.Add(bridgeGroupCheck.Check(entityNode));

            return result;
        }
    }
}
