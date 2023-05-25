# Functioning of the recognizers

The [appendix](#appendix) can be used as a tool accompanying the reading of this section. For more
information on how the recognizers are run, see the recognizer runner
[documentation](recognizer_runner.md).

## What is a recognizer

A recognizer tries to detect a specific design pattern in the
programmer's code. Such a recognizer comprises a collection of checks
examining the static code and identifying the critical parts of the
detected design pattern. These recognizers are created by dynamically
composing modular checks using the fluent design pattern.

### Checks

A recognizer is build from multiple checks which are also build from other checks, each check
searches through the @PatternPal.SyntaxTree.SyntaxGraph to look for specific nodes or relations between nodes.
More information about the individual checks can be found [here](checks.md).

### Priority

Each check has a @PatternPal.Core.Checks.Priority. This indicates the importance of the
characteristic the check validates being present in the design pattern.
There are four levels of priority, `Knockout`, `High`, `Mid`, and `Low`. A check
with a `Knockout` priority must succeed for the design pattern to be
considered at all. A certain predetermined percentage of the `High` checks
must succeed as well for the pattern to be considered. If this
percentage is met, but not all `High` checks are met, then these
unfulfilled checks are either critical points of improvement or an
indication the pattern is not implemented after all. `Mid` and `Low` checks
do not serve as determinants for the consideration of a pattern but
rather function as qualifiers for assessing the level of proficiency in
the implementation of the design pattern. They are used as final pattern
qualifiers and serve as points for further improvement.

### CheckResult

A @PatternPal.Core.ICheckResult is the result of a check. Similar to how a check can be a
collection of checks, the result of a check can be a collection of
`CheckResult`s. Each `CheckResult` has a feedback message, which provides
information about what happened when the check was run. For example, it
shows for what reason a check did not succeed.\
A leaf check corresponds to a @PatternPal.Core.LeafCheckResult. Such a `CheckResult` either
did succeed or did not. Therefore, it has a Correctness property. A node
check corresponds to a @PatternPal.Core.NodeCheckResult. It has a list of
`NodeCheckResult`s and `LeafCheckResult`s.

### CheckBuilder

The @PatternPal.Core.Checks.CheckBuilder is a static class that makes it possible to make a
collection of checks for a recognizer. It contains a number of static
methods, each one for one specific check, like `Class()` which returns a
@PatternPal.Core.Checks.ClassCheck, and `Method()` which returns a @PatternPal.Core.Checks.MethodCheck. These methods can
be used by a recognizer to create checks in a fluid way.

### Example recognizer

This recognizer checks whether there is a class which has a static,
non-private method which is used by one of its other methods.

```csharp
internal class ExampleRecognizer : IRecognizer
{
  internal IEnumerable< ICheck > Create()
  {
     MethodCheck instanceMethod =
            //this starts a collection of checks for a method
            Method(
                Priority.Knockout,
                //the first check in the collection is a modifier check
                Modifiers(
                    Priority.Knockout,
                    //requiring the method to be static
                    Modifier.Static
                ),
                //the second check in the collection is a not check
                Not(
                    Priority.Mid,
                    Modifiers(
                        Priority.Low,
                        //requiring that this method is not private
                        Modifier.Private
                    )
                )
            );

    //this starts a collection of checks for a class
    yield return
        Class(
            Priority.Knockout,
            //the first check is the previously defined method check
            instanceMethod,
            //the second check in de collection is also a method check
            Method( //this starts a new collection of checks
                Priority.Low,
                //the first check in this collection is a uses check
                Uses(
                    Priority.Low,
                    //requiring the method to use the other method
                    instanceMethod.Result
                )
            )
        );
  }
}
```

## Appendix

![easter egg](images/recognizer_uml.png "A generalized UML diagram of the recognizers.")
