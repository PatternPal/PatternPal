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
    public class DecoratorRecognizer : IRecognizer
    {
        /// <summary>
        ///     Function thats checks the returntype of a method.
        /// </summary>
        /// <param name="entityNode">The method witch it should check</param>
        /// <returns></returns>
        public IResult Recognize(IEntity entityNode)
        {
            var result = new Result();

            IEntity currentComponent = null;
            var decoratorCheck = new GroupCheck<IEntity, IEntity>(new List<ICheck<IEntity>>
            {
                new ElementCheck<IEntity>(x => x.CheckModifier("Abstract"), "NodeModifierAbstract", 0.5f),
                new ElementCheck<IEntity>(
                    x => x.CheckMinimalAmountOfRelationTypes(RelationType.Extends, 1) ||
                         x.CheckMinimalAmountOfRelationTypes(RelationType.Implements, 1), "Parent", 1),
                new ElementCheck<IEntity>(x => x.CheckMinimalAmountOfRelationTypes(RelationType.ExtendedBy, 1), "Child",
                    1),

                //Component checks
                new GroupCheck<IEntity, IEntity>(new List<ICheck<IEntity>>
                    {
                        new ElementCheck<IEntity>(x =>
                        {
                            currentComponent = x;
                            return x.GetAllMethods().Any();
                        }, "MethodAny", 1),
                        new ElementCheck<IEntity>(
                            x => x.CheckMinimalAmountOfRelationTypes(RelationType.ExtendedBy, 2) ||
                                 x.CheckMinimalAmountOfRelationTypes(RelationType.ImplementedBy, 2),
                            "DecoratorComponentChild", 1),
                        new GroupCheck<IEntity, IMethod>(
                            new List<ICheck<IMethod>>
                            {
                                new ElementCheck<IMethod>(
                                    x => x.CheckModifier("public") || x.CheckModifier("protected"),
                                    "MethodModifierNotPrivate", 1),
                                new ElementCheck<IMethod>(
                                    x => x.CheckParameters(new List<string> {currentComponent.GetName()}),
                                    "DecoratorComponentMethodParameters", 1)
                            }, x => entityNode.GetConstructors(), "DecoratorComponentConstructor"),
                        new GroupCheck<IEntity, IField>(
                            new List<ICheck<IField>>
                            {
                                new ElementCheck<IField>(
                                    x => x.CheckFieldType(new List<string> {currentComponent.GetName()}),
                                    "DecoratorComponentFieldType", 2)
                            }, x => entityNode.GetFields(), "DecoratorComponentField"),

                        //Concrete decorator
                        new GroupCheck<IEntity, IEntity>(
                            new List<ICheck<IEntity>>
                            {
                                new GroupCheck<IEntity, IMethod>(
                                    new List<ICheck<IMethod>>
                                    {
                                        new ElementCheck<IMethod>(x => x.CheckModifier("public"),
                                            "MethodModifierPublic", 1),
                                        new ElementCheck<IMethod>(
                                            x => x.CheckParameters(new List<string> {currentComponent.GetName()}),
                                            "ConcreteDecoratorMethodParameters", 1),
                                        new ElementCheck<IMethod>(
                                            x => x.CheckIfArgumentsExists(currentComponent.GetName()),
                                            "DecoratorConcreteMethodArguments", 1)
                                    }, x => x.GetConstructors(), "ConcreteDecoratorConstructor"),
                                new GroupCheck<IEntity, IMethod>(
                                    new List<ICheck<IMethod>>
                                    {
                                        new ElementCheck<IMethod>(
                                            x => x.CheckIfNameExists(currentComponent.GetAllMethods()),
                                            "MethodNameOverride"),
                                        new ElementCheck<IMethod>(x => x.CheckIfMethodCallsBase(), "MethodBaseCall")
                                    }, x => x.GetAllMethods(), "ConcreteDecoratorMethod")
                            },
                            x => entityNode.GetRelations(Relationable.Entity)
                                .Where(y => y.GetRelationType().Equals(RelationType.ExtendedBy))
                                .Select(y => y.Node2Entity), "DecoratorConcrete", GroupCheckType.Median)
                    },
                    x => x.GetRelations(Relationable.Entity)
                        .Where(y => y.GetRelationType().Equals(RelationType.Implements) ||
                                    y.GetRelationType().Equals(RelationType.Extends))
                        .Select(y => y.Node2Entity),
                    "DecoratorComponent")
            }, x => new List<IEntity> {entityNode}, "Decorator");
            var checkResult = decoratorCheck.Check(entityNode);

            result.Results = checkResult.GetChildFeedback().ToList();
            return result;
        }
    }
}
