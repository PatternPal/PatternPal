﻿#region

using PatternPal.Core.StepByStep;
using PatternPal.Core.StepByStep.Resources.Instructions;
using PatternPal.SyntaxTree.Models;

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers;

/// <summary>
/// A <see cref="IRecognizer"/> that is used to determine if the provided files or project implement the singleton pattern
/// </summary>
/// <remarks>
/// Requirements for the Singleton class:<br/>
///     a) has no public/internal constructor<br/>
///     b) has at least one private/protected constructor<br/>
///     c) has a static, private field with the same type as the class<br/>
///     d0) has a static, public/internal method that acts as a constructor in the following way<br/>
///         d1) if called and there is no instance saved in the private field, then it calls the private constructor<br/>
///         d2) if called and there is an instance saved in the private field it returns this instance<br/>
/// <br/>
/// Requirement for the Client class:<br/>
///     a) calls the method that acts as a constructor of the singleton class<br/>
/// </remarks>
internal class SingletonRecognizer : IRecognizer,
                                     IStepByStepRecognizer
{
    /// <inheritdoc cref="IRecognizer" />
    public string Name => "Singleton";

    /// <inheritdoc cref="IRecognizer" />
    public Recognizer RecognizerType => Recognizer.Singleton;

    /// <inheritdoc />
    IEnumerable< ICheck > IRecognizer.Create()
    {
        // Step 1: Checks for requirements Singleton a & b
        ICheck onlyPrivateProtectedConstructor =
            OnlyPrivateProtectedConstructor(out ConstructorCheck privateProtectedConstructorCheck);

        // Step 2: Check for requirement Singleton c
        FieldCheck staticPrivateFieldOfTypeClass =
            StaticPrivateFieldOfTypeClass();

        // Step 3: Check for requirement Singleton d0-d2
        MethodCheck checkMethodActsAsConstructorBehaviour = CheckMethodActsAsConstructorBehaviour(
            privateProtectedConstructorCheck,
            staticPrivateFieldOfTypeClass);

        yield return Class(
            Priority.Knockout,
            "1. Singleton Class",
            onlyPrivateProtectedConstructor,
            staticPrivateFieldOfTypeClass,
            checkMethodActsAsConstructorBehaviour
        );

        // Step 4: Check for requirement Client a
        yield return ClientCallsMethodActsAsConstructor(
            checkMethodActsAsConstructorBehaviour
        );
    }

    /// <inheritdoc />
    List< IInstruction > IStepByStepRecognizer.GenerateStepsList()
    {
        List< IInstruction > generateStepsList = new();

        ICheck onlyPrivateProtectedConstructor =
            OnlyPrivateProtectedConstructor(out ConstructorCheck privateConstructorCheck);
        FieldCheck staticPrivateFieldOfTypeClass = StaticPrivateFieldOfTypeClass();
        MethodCheck checkMethodAsConstructorBehaviour =
            CheckMethodActsAsConstructorBehaviour(privateConstructorCheck, staticPrivateFieldOfTypeClass);

        // Step 1: The constructor is ONLY private
        generateStepsList.Add(
            new SimpleInstruction(
                SingletonInstructions.Step1,
                SingletonInstructions.Explanation1,
                new List< ICheck >
                        {
                            Class(
                                Priority.Knockout,
                                onlyPrivateProtectedConstructor
                            )
                        }
            ));

        
        // Step 2: There is a static private field with the same type as the class
        generateStepsList.Add(
            new SimpleInstruction(
                SingletonInstructions.Step2,
                SingletonInstructions.Explanation2,
                new List< ICheck >
                    {
                        Class(
                            Priority.Knockout,
                            All(
                                Priority.Low,
                                staticPrivateFieldOfTypeClass,
                                onlyPrivateProtectedConstructor
                            )
                        )
                    }
            ));
        

        // Step 3: There is a method that acts as the constructor
        generateStepsList.Add(
            new SimpleInstruction(
                SingletonInstructions.Step3,
                SingletonInstructions.Explanation3,
                new List< ICheck >
                    {
                        Class(
                            Priority.Knockout,
                            onlyPrivateProtectedConstructor,
                            staticPrivateFieldOfTypeClass,
                            checkMethodAsConstructorBehaviour
                        )
                    }));

        // Step 4: There is a client that calls the instance method
        generateStepsList.Add(
            new SimpleInstruction(
                SingletonInstructions.Step4,
                SingletonInstructions.Explanation4,
                new List< ICheck >
                {
                    Class(
                        Priority.Knockout,
                        onlyPrivateProtectedConstructor,
                        staticPrivateFieldOfTypeClass,
                        checkMethodAsConstructorBehaviour
                    ),
                    ClientCallsMethodActsAsConstructor(checkMethodAsConstructorBehaviour)
                }));

        return generateStepsList;
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that together determine a constructor is only.
    /// <see langword="private"/>.
    /// </summary>
    internal ICheck OnlyPrivateProtectedConstructor(
        out ConstructorCheck privateProtectedConstructorCheck)
    {
        privateProtectedConstructorCheck = Constructor(
            Priority.Knockout,
            "1b. Has at least one private/protected constructor.",
            Any(
                Priority.Knockout,
                Modifiers(
                    Priority.Knockout,
                    Modifier.Private
                ),
                Modifiers(
                    Priority.Knockout,
                    Modifier.Protected
                )
            )
        );

        NotCheck noPublicInternalConstructorCheck = Not(
            Priority.Knockout,
            "1a. Has no public/internal constructor.",
            Constructor(
                Priority.Knockout,
                Any(
                    Priority.Knockout,
                    Modifiers(
                        Priority.Knockout,
                        Modifier.Public),
                    Modifiers(
                        Priority.Knockout,
                        Modifier.Internal)
                )
            )
        );

        return All(
            Priority.Low,
            privateProtectedConstructorCheck,
            noPublicInternalConstructorCheck);
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that together determine that there is a field which
    /// is private and static.
    /// </summary>
    internal FieldCheck StaticPrivateFieldOfTypeClass()
    {
        return Field(
            Priority.Knockout,
            "1c. Has a static, private field with the same type as the class.",
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
    internal ICheck[ ] IsStaticPublicOrInternal()
    {
        return new ICheck[ ]
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
    internal RelationCheck CallsPrivateProtectedConstructor(
        ConstructorCheck constructor)
    {
        //TODO: Right now it only checks if the constructor is called somewhere in a method, not at which conditions
        return Uses(
            Priority.Mid,
            constructor
        );
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that together determine that there exists a method which adheres to the requirement of Singleton d2
    /// </summary>
    internal ICheck[ ] ReturnsPrivateField(
        FieldCheck checkSingletonC)
    {
        //TODO: Right now it only checks if the field is called somewhere in a method and if the return type is the same as the class, not at which conditions
        return new ICheck[ ]
        {
           Uses(
               Priority.Mid,
               checkSingletonC
           ),
           Type(
               Priority.Knockout,
               ICheck.GetCurrentEntity
           )
        };
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that checks if the behaviour of an method in the singleton class adheres to requirements Singleton d0, d1 and d2
    /// </summary>
    internal MethodCheck CheckMethodActsAsConstructorBehaviour(
        ConstructorCheck privateProtectedConstructorCheck,
        FieldCheck staticPrivateFieldOfTypeClass)
    {
        // check d0
        ICheck[ ] hasStaticPublicInternalMethod = IsStaticPublicOrInternal();

        // check d1
        RelationCheck checkNoInstanceConstructor = CallsPrivateProtectedConstructor(privateProtectedConstructorCheck);

        // check d2
        ICheck[ ] checkInstanceConstructor = ReturnsPrivateField(staticPrivateFieldOfTypeClass);

        return Method(
            Priority.High,
            "1d. Has a static, public/internal method that acts as a constructor in the following way: If called and there is an instance saved in the private field it returns this instance, otherwise it calls the private constructor.",
            hasStaticPublicInternalMethod.Append(checkNoInstanceConstructor).Concat(checkInstanceConstructor).ToArray()
        );
    }

    /// <summary>
    /// A collection of <see cref="ICheck"/>s that together determine if the client class calls the method which acts as a constructor in Singleton
    /// </summary>
    internal ClassCheck ClientCallsMethodActsAsConstructor(
        MethodCheck getInstanceMethod)
    {
        return Class(
            Priority.Low,
            "2. Client Class",
            Any(
                Priority.Low,
                "2a. Calls the method that acts as a constructor of the singleton class.",
                Constructor(
                    Priority.High,
                    Uses(
                        Priority.Mid,
                        getInstanceMethod
                    )
                ),
                Method(
                    Priority.High,
                    Uses(
                        Priority.Mid,
                        getInstanceMethod
                    )
                )
            )
        );
    }
}
