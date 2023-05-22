#region

using System.Linq;
using Google.Protobuf.WellKnownTypes;
using PatternPal.SyntaxTree.Models;
using static PatternPal.Core.Checks.CheckBuilder;
#endregion

namespace PatternPal.Core.Recognizers;

/// <summary>
/// A <see cref="IRecognizer"/> that is used to determine if the provided file is an implementation
/// of the singleton pattern.
/// </summary>
/// <remarks>
/// Requirements to fulfill the pattern:<br/>
/// a) has no public/internal constructor<br/>
/// b) has at least one private/protected constructor<br/>
/// c) has a static, private field with the same type as the class<br/>
/// d0) has a static, public/internal method that acts as a constructor in the following way<br/>
///     d1) if called and there is no instance saved in the private field, then it calls the private constructor<br/>
///     d2) if called and there is an instance saved in the private field it returns this instance<br/>
/// <br/>
/// Optional requirement client:<br/>
/// a) calls the method that acts as a constructor of the singleton class<br/>
/// </remarks>
internal class SingletonRecognizer : IRecognizer
{
    /// <summary>
    /// A method which creates a lot of <see cref="ICheck"/>s that each adheres to the requirements a singleton pattern needs to have implemented.
    /// It returns the requirements in a tree structure stated per class.
    /// </summary>
    public IEnumerable<ICheck> Create()
    {
        // Step 1: Checks for requirements Singleton a & b
        ICheck onlyPrivateConstructor =
            OnlyPrivateConstructor(out ConstructorCheck privateConstructorCheck);

        // Step 2: Check for requirement Singleton c
        FieldCheck staticPrivateFieldOfTypeClass =
            StaticPrivateFieldOfTypeClass();

        // Step 3: Check for requirement Singleton d0-d2
        ICheck checkMethodActsAsConstructorBehaviour = CheckMethodAcsAsConstructorBehaviour(
            privateConstructorCheck,
            staticPrivateFieldOfTypeClass,
            out ICheck[] hasStaticPublicInternalMethod);

        // Step 4: Check for requirement Client a
        ClassCheck checkClientA = ClientCallsMethodActsAsConstructor(
            Method(
                Priority.Low,
                hasStaticPublicInternalMethod
            )
        );

        yield return Class(
            Priority.Low,
            onlyPrivateConstructor,
            staticPrivateFieldOfTypeClass,
            checkMethodActsAsConstructorBehaviour
        );

        yield return Class(
            Priority.Low,
            checkClientA
        );
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that together determine a constructor is only.
    /// <see langword="private"/>.
    /// </summary>
    internal ICheck OnlyPrivateConstructor(out ConstructorCheck privateConstructorCheck)
    {
        privateConstructorCheck = Constructor(
            Priority.Knockout,
            Modifiers(
                Priority.Knockout,
                Modifier.Private
            )
        );

        NotCheck noPuclicConstructorCheck = Not(
            Priority.Knockout,
            Constructor(
                Priority.Knockout,
                Any(
                    Priority.Knockout,
                    Modifiers(
                        Priority.Knockout,
                        Modifier.Public),
                    Modifiers(
                        Priority.Knockout,
                        Modifier.Internal),
                    Modifiers(
                        Priority.Knockout,
                        Modifier.Protected
                    )
                )
            )
        );

        return All(Priority.Low,
            privateConstructorCheck,
            noPuclicConstructorCheck);
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that together determine that there is a field which
    /// is private and static in a given class.
    /// </summary>
    internal FieldCheck StaticPrivateFieldOfTypeClass()
    {
        return Field(
            Priority.Knockout,
            Modifiers(
                Priority.Knockout,
                Modifier.Static,
                Modifier.Private
            ),
            Type(
                Priority.Knockout,
                ICheck.GetCurrentEntity
            )
        );
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that together determine that the class has a static and public/internal method
    /// </summary>
    internal ICheck[] HasStaticPublicInternalMethod()
    {
        return new ICheck[]
        {
            Modifiers(
                Priority.Knockout,
                Modifier.Static
            ),
            Any(
                Priority.Knockout,
                Modifiers(
                    Priority.Knockout,
                    Modifier.Public
                ),
                Modifiers(
                    Priority.Knockout,
                    Modifier.Internal
                )
            )
        };
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that together determine that there exists a method which adheres to the requirement of Singleton d1
    /// </summary>
    internal RelationCheck CallsPrivateConstructor(ConstructorCheck constructor)
    {
        //Right now it only checks if the constructor is called somewhere in a method, not at which conditions
        return Uses(
            Priority.Mid,
            constructor.Result
        );

        /*TODO: fix and use 'correct' implementation below
        //return Method(
        //    Priority.Mid,
        //    new IfThenOperatorCheck(
        //        Priority.Mid,
        //        new List<ICheck>(),
        //        new List<ICheck> {
        //            Uses(
        //                Priority.Mid,
        //                constructor.Result
        //            )
        //        }
        //    )
        //);*/
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that together determine that there exists a method which adheres to the requirement of Singleton d2
    /// </summary>
    internal ICheck[] ReturnsPrivateField(FieldCheck checkSingletonC)
    {
        //Right now it only checks if the field is called somewhere in a method and if the return type is the same as the class, not at which conditions
        return new ICheck[]
        {
            Uses(
                Priority.Mid,
                checkSingletonC.Result
            ),
            Type(
                Priority.Knockout,
                ICheck.GetCurrentEntity
            )
        };


        /*TODO: fix and use 'correct' implementation below
        //return Method(
        //    Priority.Mid,
        //    new IfThenOperatorCheck(
        //        Priority.Mid,
        //        new List<ICheck>(),
        //        new List<ICheck> {
        //            Uses(
        //                Priority.Mid,
        //                checkSingletonC.Result
        //            )
        //        }
        //    )
        //);*/
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that checks if the behaviour of an method in the singleton class adheres to requirements Singleton d0, d1 and d2
    /// </summary>
    internal ClassCheck CheckMethodAcsAsConstructorBehaviour(
        ConstructorCheck privateConstructorCheck,
        FieldCheck staticPrivateFieldOfTypeClass,
        out ICheck[] hasStaticPublicInternalMethod)
    {
        // check d0
        hasStaticPublicInternalMethod = HasStaticPublicInternalMethod();

        // check d1
        RelationCheck checkNoInstanceConstructor = CallsPrivateConstructor(privateConstructorCheck);

        // check d2
        ICheck[] checkInstanceConstructor = ReturnsPrivateField(staticPrivateFieldOfTypeClass);

        return Class(
            Priority.Low,
            privateConstructorCheck,
            staticPrivateFieldOfTypeClass,
            Method(
                Priority.High,
                hasStaticPublicInternalMethod.Append(
                    checkNoInstanceConstructor).Concat(
                        checkInstanceConstructor).ToArray()
            )
        );
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that together determine if the client class calls the method which acts as a constructor in Singleton
    /// </summary>
    internal ClassCheck ClientCallsMethodActsAsConstructor(MethodCheck getInstanceMethod)
    {
        return Class(
            Priority.Low,
            Any(
                Priority.Low,
                Constructor(
                    Priority.High,
                    Uses(
                        Priority.Mid,
                        getInstanceMethod.Result
                    )
                ),
                Method(
                    Priority.High,
                    Uses(
                        Priority.Mid,
                        getInstanceMethod.Result
                    )
                )
            )
        );
    }
}
