using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;

namespace IDesign.Recognizers
{
    public class StateRecognizer : IRecognizer
    {
        private Result result;

        public IResult Recognize(IEntityNode node)
        {
            result = new Result();
            var relations = node.GetRelations();

            IEntityNode entityNode = null;

            var statePatternCheck = new GroupCheck<IEntityNode, IEntityNode>(
                new List<ICheck<IEntityNode>>
                {
                    //check if node is abstract class or interface
                    new ElementCheck<IEntityNode>(
                        x => x.CheckTypeDeclaration(EntityNodeType.Interface) |
                             (x.CheckTypeDeclaration(EntityNodeType.Class) && x.CheckModifier("abstract")),
                        "NodeAbstractOrInterface", 2
                    ),

                    //check state node methods
                    new GroupCheck<IEntityNode, IMethod>(
                        new List<ICheck<IMethod>>
                        {
                            new ElementCheck<IMethod>(
                                x => x.CheckReturnType("void"),
                                new ResourceMessage("MethodReturnType", new[] { "void" }), 1
                            ),
                            new ElementCheck<IMethod>(x => x.GetBody() == null, "MethodBodyEmpty", 1)
                        }, x => x.GetMethodsAndProperties(), "StateNodeMethods"
                    ),

                    //check state node used by relations
                    new GroupCheck<IEntityNode, IEntityNode>(
                        new List<ICheck<IEntityNode>>
                        {
                            new ElementCheck<IEntityNode>(
                                x => x.CheckMinimalAmountOfRelationTypes(RelationType.UsedBy, 1), "NodeUses1", 0.5f
                            ),

                            //check if field has state as type
                            new GroupCheck<IEntityNode, IEntityNode>(
                                new List<ICheck<IEntityNode>>
                                {
                                    new ElementCheck<IEntityNode>(
                                        x => x.CheckTypeDeclaration(EntityNodeType.Interface) |
                                             (x.CheckTypeDeclaration(EntityNodeType.Class) &&
                                              x.CheckModifier("abstract")), "NodeAbstractOrInterface", 2
                                    )
                                }, x => new List<IEntityNode> { node }, "StateFieldStateType"
                            ),

                            //check context class fields
                            new GroupCheck<IEntityNode, IField>(
                                new List<ICheck<IField>>
                                {
                                    //TO DO: check name

                                    new ElementCheck<IField>(
                                        x => x.CheckMemberModifier("private"), "FieldModifierPrivate", 0.5f
                                    )
                                }, x => x.GetFields(), "StateContextField", GroupCheckType.All
                            )
                        },
                        x => x.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.UsedBy))
                            .Select(y => y.GetDestination()), "StateContext"
                    ),

                    //check inheritance
                    new GroupCheck<IEntityNode, IEntityNode>(
                        new List<ICheck<IEntityNode>>
                        {
                            new ElementCheck<IEntityNode>(
                                x =>
                                {
                                    entityNode = x;
                                    return x.GetMethodsAndProperties().Any();
                                }, "MethodAny"
                            ),
                            new GroupCheck<IEntityNode, IEntityNode>(
                                new List<ICheck<IEntityNode>>
                                {
                                    new GroupCheck<IEntityNode, IMethod>(
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
                                        }, x => x.GetMethodsAndProperties(), "StateClassChangeState"
                                    )
                                },
                                x => entityNode.GetRelations()
                                    .Where(y => y.GetRelationType().Equals(RelationType.Creates))
                                    .Select(y => y.GetDestination()), "StateCreatesOtherState"
                            )
                        }, x => x.GetRelations().Where(
                            y => y.GetRelationType().Equals(RelationType.ExtendedBy) ||
                                 y.GetRelationType().Equals(RelationType.ImplementedBy)
                        ).Select(y => y.GetDestination()), "StateConcrete", GroupCheckType.All
                    )
                }, x => new List<IEntityNode> { node }, "State"
            );

            result.Results.Add(statePatternCheck.Check(node));
            foreach (var concrete in node.GetRelations().Where(
                x =>
                    x.GetRelationType().Equals(RelationType.ExtendedBy) ||
                    x.GetRelationType().Equals(RelationType.ImplementedBy)
            ).Select(x => x.GetDestination()))
            {
                result.RelatedSubTypes.Add(concrete, "ConcreteState");
            }

            return result;
        }
    }
}
