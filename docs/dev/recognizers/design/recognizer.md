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

A recognizer is built from multiple checks which are also built from other checks, each check
searches through the @PatternPal.SyntaxTree.SyntaxGraph to look for specific nodes or relations between nodes.
More information about the individual checks can be found [here](checks.md).

### Priority

Each check has a @PatternPal.Core.Checks.Priority. This indicates the importance of the
characteristic the check validates being present in the design pattern.
There are four levels of priority, `Knockout`, `High`, `Mid`, and `Low`. A check
with a `Knockout` priority must succeed for the design pattern to be
considered at all. A check with `High`, `Mid`, or `Low` priority is not required to succeed
for the design pattern to be considered. These priorities are instead used for two reasons. The first is to sort the remaining results
and find the most probable place the pattern could be implemented. The second is to assess the level of proficiency in the implementation of the pattern.
Logically, a check with a `High` priority is more important than a check with a `Mid` priority, which is more important than a check with a `Low` priority.
However, to make sure priority assessment is done in a similar fashion for all recognizers, a list of assessment criteria has been made:
`High` indicates a very important characteristic. This characteristic should be present, and it should be present in precisely this way. If, by means of checking the knock-out criteria, 
it is probable the user is implementing a specific pattern, there is no good reason these parts of the pattern are not present. Often, `High` priority checks are structural checks, 
i.e. checks for a private field or an abstract method. `Mid` indicates a less important characteristic. The core functionality of the pattern is still present if this part is not implemented, or implemented in a different way. Another example is the usage of the protected keyword instead of private. Another case in which this criterium could be given is when the specific part of the pattern is not essential for the pattern, yet it does adhere nicely to a design principle, for example, implementing an interface instead of an implementation.
`Low` indicates the least important characteristics. The parts of a pattern that could essentially be done in multiple different ways or by multiple different components. The pattern is essentially still implemented if this part is lacking, covered by another part of the code, or split over multiple different components. Client classes are often of `Low` priority.


### CheckResult

A @PatternPal.Core.ICheckResult is the result of a check. Similar to how a check can be a
collection of checks, the result of a check can be a collection of
`CheckResult`s. It has the priority of its check, the `Node` on which the check was checked, and the check that created it.\
A leaf check corresponds to a @PatternPal.Core.LeafCheckResult. Such a `CheckResult` either
did succeed or did not. For example, a @Patternpal.Core.Checks.ModifierCheck creates a `LeafCheckResult`. 
Therefore, it has a Correctness property. A node ccheck corresponds to a @PatternPal.Core.NodeCheckResult. It has a list of
`NodeCheckResult`s and `LeafCheckResult`s.

### CheckBuilder

The @PatternPal.Core.Checks.CheckBuilder is a static class that makes it possible to make a
collection of checks for a recognizer. It contains a number of static
methods, each one for one specific check, like `Class()` which returns a
@PatternPal.Core.Checks.ClassCheck, and `Method()` which returns a @PatternPal.Core.Checks.MethodCheck. These methods can
be used by a recognizer to create checks in a fluid way.

### Example recognizer

This recognizer checks whether there is a class that has a static,
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
