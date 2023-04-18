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
    public class StateRecognizer : IRecognizer
    {
        private Result result;

        public IResult Recognize(IEntity node)
        {
            result = new Result();
            var relations = node.GetRelations(RelationTargetKind.Entity);

            IEntity entityNode = null;

            var statePatternCheck = new GroupCheck<IEntity, IEntity>(
                new List<ICheck<IEntity>>
                {
                    //check if node is abstract class or interface
                    new ElementCheck<IEntity>(
                        x => x.CheckTypeDeclaration(EntityType.Interface) |
                             (x.CheckTypeDeclaration(EntityType.Class) &&
                              x.CheckModifier("abstract")), "NodeAbstractOrInterface", 2
                    ),

                    //check state node methods
                    new GroupCheck<IEntity, IMethod>(
                        new List<ICheck<IMethod>>
                        {
                            new ElementCheck<IMethod>(
                                x => x.CheckReturnType("void"),
                                new ResourceMessage("MethodReturnType", new[] { "void" }), 1
                            ),
                            new ElementCheck<IMethod>(x => x.GetBody() == null, "MethodBodyEmpty", 1)
                        }, x => x.GetAllMethods(), "StateNodeMethods"
                    ),

                    //check state node used by relations
                    new GroupCheck<IEntity, IEntity>(
                        new List<ICheck<IEntity>>
                        {
                            new ElementCheck<IEntity>(
                                x => x.CheckMinimalAmountOfRelationTypes(RelationType.UsedBy, 1),
                                "NodeUses1", 0.5f
                            ),

                            //check if field has state as type
                            new GroupCheck<IEntity, IEntity>(
                                new List<ICheck<IEntity>>
                                {
                                    new ElementCheck<IEntity>(
                                        x => x.CheckTypeDeclaration(EntityType.Interface) |
                                             (x.CheckTypeDeclaration(EntityType.Class) &&
                                              x.CheckModifier("abstract")), "NodeAbstractOrInterface", 2
                                    )
                                }, x => new List<IEntity> { node }, "StateFieldStateType"
                            ),

                            //check context class fields
                            new GroupCheck<IEntity, IField>(
                                new List<ICheck<IField>>
                                {
                                    //TO DO: check name

                                    new ElementCheck<IField>(
                                        x => x.CheckMemberModifier("private"), "FieldModifierPrivate",
                                        0.5f
                                    )
                                }, x => x.GetFields(), "StateContextField", GroupCheckType.All
                            )
                        },
                        x => x.GetRelations(RelationTargetKind.Entity).Where(y => y.GetRelationType().Equals(RelationType.UsedBy)).Select(y => y.Target.AsT0), "StateContext"
                    ),

                    //check inheritance
                    new GroupCheck<IEntity, IEntity>(
                        new List<ICheck<IEntity>>
                        {
                            new ElementCheck<IEntity>(
                                x =>
                                {
                                    entityNode = x;
                                    return x.GetAllMethods().Any();
                                }, "MethodAny"
                            ),
                            new GroupCheck<IEntity, IEntity>(
                                new List<ICheck<IEntity>>
                                {
                                    new GroupCheck<IEntity, IMethod>(
                                        new List<ICheck<IMethod>>
                                        {
                                            new ElementCheck<IMethod>(
                                                x => x.CheckReturnType("void"),
                                                new ResourceMessage("MethodReturnType", new[] { "void" }), 1
                                            ),
                                            new ElementCheck<IMethod>(
                                                x => x.CheckCreationType(entityNode.GetName()) &&
                                                     !x.CheckCreationType(node.GetName()),
                                                new ResourceMessage("MethodCreateSameInterface"), 2
                                            )
                                        }, x => x.GetAllMethods(), "StateClassChangeState"
                                    )
                                },
                                x => entityNode.GetRelations(RelationTargetKind.Entity).Where(y => y.GetRelationType().Equals(RelationType.Creates)).Select(y => y.Target.AsT0), "StateCreatesOtherState"
                            )
                        }, x => x.GetRelations(RelationTargetKind.Entity).Where(
                            y =>
                                y.GetRelationType().Equals(RelationType.ExtendedBy) ||
                                y.GetRelationType().Equals(RelationType.ImplementedBy)
                        ).Select(y => y.Target.AsT0), "StateConcrete", GroupCheckType.All
                    )
                }, x => new List<IEntity> { node }, "State"
            );

            result.Results.Add(statePatternCheck.Check(node));
            foreach (var concrete in node.GetRelations(RelationTargetKind.Entity).Where(x => x.GetRelationType().Equals(RelationType.ExtendedBy) || x.GetRelationType().Equals(RelationType.ImplementedBy)).Select(x => x.Target.AsT0))
            {
                result.RelatedSubTypes.Add(concrete, "ConcreteState");
            }

            return result;
        }
    }
}
