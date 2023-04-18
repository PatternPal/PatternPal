using System.Collections.Generic;
using System.Linq;
using PatternPal.Recognizers.Abstractions;
using PatternPal.Recognizers.Checks;
using PatternPal.Recognizers.Models.ElementChecks;
using PatternPal.Recognizers.Models.Output;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace PatternPal.Recognizers.Recognizers
{
    public class StrategyRecognizer : IRecognizer
    {
        private Result result;

        public IResult Recognize(IEntity node)
        {
            result = new Result();
            var relations = node.GetRelations(RelationTargetKind.Entity);

            var strategyPatternCheck = new GroupCheck<IEntity, IEntity>(
                new List<ICheck<IEntity>>
                {
                    //check if node is abstract class or interface
                    new ElementCheck<IEntity>(
                        x => x.CheckTypeDeclaration(EntityType.Interface) |
                             (x.CheckTypeDeclaration(EntityType.Class) && x.CheckModifier("abstract")),
                        "NodeAbstractOrInterface", 2
                    ),

                    //check strategy node methods
                    new GroupCheck<IEntity, IMethod>(
                        new List<ICheck<IMethod>>
                        {
                            new ElementCheck<IMethod>(x => x.GetBody() == null, "MethodBodyEmpty", 1)
                        }, x => x.GetAllMethods(), "StrategyNodeMethods "
                    ),

                    //check strategy node used by relations
                    new GroupCheck<IEntity, IEntity>(
                        new List<ICheck<IEntity>>
                        {
                            new ElementCheck<IEntity>(
                                x => x.CheckMinimalAmountOfRelationTypes(RelationType.UsedBy, 1), "NodeUses1", 1
                            ),

                            //check if field has strategy as type
                            new GroupCheck<IEntity, IEntity>(
                                new List<ICheck<IEntity>>
                                {
                                    new ElementCheck<IEntity>(
                                        x => x.CheckTypeDeclaration(EntityType.Interface) |
                                             (x.CheckTypeDeclaration(EntityType.Class) && x.CheckModifier("abstract")),
                                        "NodeAbstractOrInterface", 1
                                    )
                                }, x => new List<IEntity> {node}, "StrategyFieldStateType"
                            ),

                            //check context class fields
                            new GroupCheck<IEntity, IField>(
                                new List<ICheck<IField>>
                                {
                                    new ElementCheck<IField>(
                                        x => x.CheckMemberModifier("private"), "FieldModifierPrivate", 0.5f
                                    )
                                }, x => x.GetFields(), "StrategyContextField", GroupCheckType.All
                            )
                        },
                        x => x.GetRelations(RelationTargetKind.Entity).Where(y => y.GetRelationType().Equals(RelationType.UsedBy))
                            .Select(y => y.Target.AsT0), "StrategyContext"
                    ),

                    //check inheritance
                    new GroupCheck<IEntity, IEntity>(
                        new List<ICheck<IEntity>>
                        {
                            new ElementCheck<IEntity>(
                                x => x.GetRelations(RelationTargetKind.Entity).All(y => y.GetRelationType() != RelationType.Creates),
                                "NodeDoesNotCreate", 2
                            ),
                            new ElementCheck<IEntity>(
                                x => x.GetRelations(RelationTargetKind.Entity).All(y => y.GetRelationType() != RelationType.Uses),
                                "NodeDoesNotUse", 1
                            )
                        }, x => x.GetRelations(RelationTargetKind.Entity).Where(y => 
                            y.GetRelationType().Equals(RelationType.ExtendedBy) || 
                             y.GetRelationType().Equals(RelationType.ImplementedBy)).Select(y => y.Target.AsT0), "StrategyConcrete", GroupCheckType.All
                    )
                }, x => new List<IEntity> {node}, "Strategy"
            );

            result.Results.Add(strategyPatternCheck.Check(node));


            result.RelatedSubTypes.Add(node, "AbstractStrategy");

            foreach (var concrete in node.GetRelations(RelationTargetKind.Entity).Where(x => 
                         (x.GetRelationType() is RelationType.ExtendedBy or RelationType.ImplementedBy)).Select(x => x.Target.AsT0))
            {
                result.RelatedSubTypes.Add(concrete, "ConcreteStrategy");
            }

            return result;
        }
    }
}
