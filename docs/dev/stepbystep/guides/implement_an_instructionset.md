# Implement an InstructionSet

To support a design pattern in PatternPal one must implement the `IStepByStepRecognizer` interface. 
This interface defines a `string` `Name`, a `RecognizerType` as defined in the protocol buffer and 
a method `GenerateStepsList`, which will return the object required of the Step-By-Step module.

In order to make effective use of existing `Recognizers` for bot detecting patterns and Step-By-Step 
it is recommended to encase the `IChecks` created in the `Recognizer's` `Create` method 
([here](~/dev/recognizers/guides/implement_recognizer.md)) into separate methods. This way the checks can
be declared once and can be called in both `Create` in `IRecognizer` and in `GenerateStepsList` in 
`IStepByStepRecognizer`. 
 

## Prerequisites 

First, it is of utmost importance that one has a clear idea of how much is asked of the user per step. It 
is  preferred to make the steps small and that they are supported by the available `ICheck`s 
([here](~/dev/recognizers/design/checks.md)). Second, it is important to have an explanation to 
communicate to the user what is required to pass the step and optionally and explanation why the 
requirement is the way it is. 

## Implementing the GenerateStepsList method

To support a Pattern for Step-By-Step one must add the following `using` statement to the top of 
file and implement `IStepByStepRecognizer`:

```csharp
using static PatternPal.Core.StepByStep;
using static PatternPal.Core.StepByStep.Resources.Instructions;
```

The result class should look something like this:

```csharp
using static PatternPal.Core.StepByStep;
using static PatternPal.Core.StepByStep.Resources.Instructions;

namespace MyStepBySteps;

internal class MyStepBySteps : IStepByStepRecognizer
{
	public string Name => "Singleton";
	
	public Recognizer RecognizerType => Recognizer.Singleton;
	
	List< IInstruction > IStepByStepRecognizer.GenerateStepsList()
	{
		throw new NotImplementedException();
	}
}
```

## Implementing the method
