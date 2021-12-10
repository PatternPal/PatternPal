using System;
using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers.Recognizers {
    public class SingletonRecognizer : IRecognizer {
        public IResult Recognize(IEntity entityNode) {
            var result = new Result();

            var methodChecks = new List<ICheck<IMethod>> {
                new ElementCheck<IMethod>(
                    x => x.CheckReturnType(entityNode.GetName()),
                    new ResourceMessage("MethodReturnType", new[] { entityNode.GetName() }), 1
                ),
                new ElementCheck<IMethod>(x => x.CheckModifier("static"), "MethodModifierStatic", 1),
                new ElementCheck<IMethod>(
                    x => x.CheckReturnTypeSameAsCreation(),
                    new ResourceMessage("SingletonMethodReturnCreationType")
                )
            };

            var propertyChecks = new List<ICheck<IField>> {
                new ElementCheck<IField>(
                    x => x.CheckFieldTypeGeneric(new List<string>() { entityNode.GetName() }),
                    new ResourceMessage("FieldType", new[] { entityNode.GetName() }), 1
                ),
                new ElementCheck<IField>(x => x.CheckMemberModifier("static"), "FieldModifierStatic", 1),
                new ElementCheck<IField>(x => !x.CheckMemberModifier("public"), "FieldModifierNotPublic", 1)
            };

            var constructorChecks = new List<ICheck<IConstructor>> {
                new ElementCheck<IConstructor>(x => !x.CheckModifier("public"), "ConstructorModifierNotPublic", 0.5F)
            };


            var singletonCheck = new GroupCheck<IEntity, IEntity>(
                new List<ICheck<IEntity>> {
                    new GroupCheck<IEntity, IMethod>(methodChecks, x => x.GetAllMethods(), "SingletonMethod"),
                    new GroupCheck<IEntity, IField>(
                        propertyChecks, x => (x is IClass cls) ? cls.GetFields() : Array.Empty<IField>(),
                        "SingletonField"
                    ),
                    new GroupCheck<IEntity, IConstructor>(
                        constructorChecks, x => (x is IClass cls) ? cls.GetConstructors() : Array.Empty<IConstructor>(),
                        "SingletonConstructor"
                    )
                }, x => new List<IEntity> { entityNode }, "Singleton", GroupCheckType.All
            );

            var r = singletonCheck.Check(entityNode);
            result.Results = r.GetChildFeedback().ToList();


            result.RelatedSubTypes.Add(entityNode, "Singleton");
            return result;
        }
    }
}
