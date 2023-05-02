#region

using static PatternPal.Core.Checks.CheckBuilder;
using Type = System.Type;

#endregion

namespace PatternPal.Core.Recognizers;

internal class SingletonRecognizer : IRecognizer
{
    /* Requirements to fullfill the pattern:
     *         Singleton
     *               a) has no public/internal constructor
     *               b) has at least one private/protected constructor
     *               c) has a static, private field with the same type as the class
     *               d) has a static, public/internal method that acts as a constructor in the following way
     *                     1) if called and there is no instance saved in the private field, then it calls the private constructor
     *                     2) if called and there is an instance saved in the private field it returns this instance
     *         Client
     *               a) calls the getInstance() method of the singleton class
     */

    internal IEnumerable< ICheck > Create()
    {
        //Singleton a) has no public/internal constructor
        NotCheck checkSingletonA = Not(
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
                        Modifier.Internal
                    )
                )
            )
        );

        //Singleton b) has at least one private/protected constructor
        ConstructorCheck checkSingletonB = Constructor(
            Priority.Knockout,
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

        //Singleton c) has a static, private field with the same type as the class
        FieldCheck checkSingletonC = Field(
            Priority.Knockout,
            Modifiers(
                priority.knockout,
                Modifier.Static,
                Modifier.Private
            ),
            Type(
                Priority.Knockout,
               //TODO GEEN IDEE OF DIT KLOPT!
               ICheck.GetCurrentEntity
            )
        );

        //Singleton d0) has a static, public/internal method
        MethodCheck getInstanceMethod = Method(
            Priority.Knockout,
            Modifiers(
                Priority.Low,
                Modifier.Static
            ),
            Any(
                Priority.Low,
                Modifiers(
                    Priority.Low,
                    Modifier.Public
                ),
                Modifiers(
                    Priority.Mid,
                    Modifier.Internal
                )
            )
        );

        //Singleton d1) if called and there is no instance saved in the private field, then it calls the private constructor
        //TODO: implement

        //Singleton d2) if called and there is an instance saved in the private field it returns this instance
        //TODO: implement

        //Client a) calls the getInstance() method of the singleton class
        MethodCheck checkClientA = Method(
            Priority.Mid,
            Uses(
                Priority.Mid,
                getInstanceMethod.Result
            )
        );

        yield return Class(
            Priority.Low,
            checkSingletonA,
            checkSingletonB,
            checkSingletonC,
            getInstanceMethod
        );

        yield return Class(
            Priority.Low,
            checkClientA
        );

    }
}
