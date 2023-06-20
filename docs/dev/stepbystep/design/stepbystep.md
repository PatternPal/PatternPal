# StepByStep
StepByStep allows a user to implement a pattern step by step. When all steps are completed, the user has successfully implemented the desired pattern.

## IStepByStepRecognizer interface
TODO (technical explanation, not usage as done in guide)

## SimpleInstruction
TODO (technical explanation, not usage as done in guide)

## Usage of the `RecognizerRunner`
As the steps of a StepByStep implementation are made of the same checks a recognizer is made of, the @PatternPal.Core.Runner.RecognizerRunner can be used to run the @PatternPal.Core.Checks.IChecks on the user's code.
However, since all checks of a step need to be perfectly implemented for a step to be completed, we do not care about check priorities. One could also say that all checks have `Knockout` @PatternPal.Core.Checks.Priority.
For this purpose, the `RecognizerRunner` can be run with `pruneAll` set to `true`. This lets the `RecognizerRunner` prune results regardless of the priority of their check.

TODO further useful information
