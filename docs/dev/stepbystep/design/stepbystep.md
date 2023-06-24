# Step-By-Step
Step-By-Step allows a user to implement a pattern step by step. From a dropdown menu, the user selects a design pattern.
When all steps are completed, the user has successfully implemented the desired pattern.

## `IStepByStepRecognizer` interface
The implementation of the Step-By-Step module is decoupled from the @PatternPal.Core.Recognizers.IRecognizer interface. This is done
by defining the @PatternPal.Core.StepByStep.IStepByStepRecognizer interface. The @PatternPal.Core.StepByStep.IStepByStepRecognizer is made up of a list of type @PatternPal.Core.StepByStep.IInstruction. These
serve as instructions to implement a design pattern. It has similarities to the @PatternPal.Core.Recognizers.IRecognizer but instead of
returning a list of type @PatternPal.Core.Checks.ICheck it returns a list of type @PatternPal.Core.StepByStep.IInstruction. Implementing
the interface adds it as supported Step-By-Step design patterns in the @PatternPal.Core.Runner.RecognizerRunner.  

An example of the implemented interface can be found [here](~/dev/stepbystep/guides/implement_an_instructionset.md).

## `SimpleInstruction`
The @PatternPal.Core.StepByStep.SimpleInstruction is an instruction used in the Step-By-Step module and it includes a list of type 
@PatternPal.Core.Checks.ICheck. These checks are the requirements that need to be fulfilled in the step. The additional `string`s are 
a textual representation of these `check`s. 

## Usage of the `RecognizerRunner` by Step-By-Step
Because the @PatternPal.Core.Checks.ICheck in a @PatternPal.Core.StepByStep.SimpleInstruction for Step-By-Step are the same checks as in 
the PatternPal.Core.Recognizers.IRecognizer `Create` method, the @PatternPal.Core.Runner.RecognizerRunner can be used. However, because
every @PatternPal.Core.Checks.ICheck is required to have a priority the @PatternPal.Core.Runner.RecognizerRunner is used differently. 
Because all the checks in a step need to be perfectly implemented for the step to be 'correct', we do not care about the check priorities.
One could also say that all checks have `Knockout` @PatternPal.Core.Checks.Priority. For this purpose, the @PatternPal.Core.Runner.RecognizerRunner 
can be run with an extra parameter `pruneAll` set to `true`. This lets the @PatternPal.Core.Runner.RecognizerRunner prune results regardless of the 
priority of their check. Treating every check as such results in no results from the @PatternPal.Core.Runner.RecognizerRunner and this is equal
to an incorrect implementation. 
