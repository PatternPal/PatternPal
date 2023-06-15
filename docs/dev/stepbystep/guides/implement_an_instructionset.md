# Implement an InstructionSet

To support a design pattern in PatternPal one must implement the `IStepByStepRecognizer` interface. 
This interface defines a `string` `Name`, a `RecognizerType` as defined in the protocol buffer and 
a method `GenerateStepsList`, which will return the object required of the Step-By-Step module.

# Efficiently add to `Recognizer` functionality

In order to make effective use of existing `Recognizers` for both detecting patterns and Step-By-Step 
it is recommended to encase the `IChecks` created in the `Recognizer's` `Create` method 
(more on this method can be found [here](~/dev/recognizers/guides/implement_recognizer.md)) into separate methods. This way the checks can
be declared once and can be called in both `Create` in `IRecognizer` and in `GenerateStepsList` in 
`IStepByStepRecognizer`. The Recognizers would then inherit from both recognizer interfaces (`IRecognizer` 
and `IStepByStepRecognizer`).

## Prerequisites 

First, it is of utmost importance that one has a clear idea of how much is asked of the user per step. It 
is  preferred to make the steps small and that they are supported by the available `ICheck`s 
(available checks can be found [here](~/dev/recognizers/design/checks.md)). Second, it is important to have an explicit instruction to 
communicate to the user what is required to pass the step and optionally an explanation why the 
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
The return type is a list of `IInstruction`s. These instructions require a `string Requirement` 
(the previously mentioned explicit instruction), `string Description` (the previously mentioned explanation) 
and a list of `ICheck`s that reflect the requirement of the step. 

The `Requirement` and `Description` are retrieved from the resources (`.resx` files). These are to be added 
to the `PatternPal.Core` project under the folder `StepByStep\Resources\Instructions`. For example: 
"`SingletonInstructions.resx`". Per step a `Requirement` string and `Description` string can be added to 
this resource.

Creating an `IInstruction` should lastly be supplied with a list of `ICheck`s that need to pass in a step. 
These can be added to a list of instruction in `GenerateStepsList`. An example would be :

```csharp

List< IInstruction > IStepByStepRecognizer.GenerateStepsList()
{
	List< IInstruction > generateStepsList = new();
	
	// Step 1: There is a static private field with the same type as the class
	generateStepsList.Add(
            new SimpleInstruction(
                SingletonInstructions.Step1,
                SingletonInstructions.Explanation1,
                new List< ICheck >
				{
					Class(
						Priority.Knockout,
						Field(
							Priority.Knockout,
							"Has a static, private field with the same type as the class",
							Modifiers(
								Priority.Knockout,
								Modifier.Static,
								Modifier.Private
							),
							Type(
								Priority.Knockout,
								ICheck.GetCurrentEntity
							)
						)
					)
				}
            ));
	
	// Step 2: ...
	...
	
	return generateStepsList;
}
```

When the `IStepByStepRecognizer` interface is implemented it is automatically added as an option to the 
extension. It can then be selected from the combobox in the `StepByStepListView`.

## Final Code

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
		List< IInstruction > generateStepsList = new();
		
		// Step 1: There is a static private field with the same type as the class
		generateStepsList.Add(
				new SimpleInstruction(
					SingletonInstructions.Step1,
					SingletonInstructions.Explanation1,
					new List< ICheck >
					{
						Class(
							Priority.Knockout,
							Field(
								Priority.Knockout,
								"Has a static, private field with the same type as the class",
								Modifiers(
									Priority.Knockout,
									Modifier.Static,
									Modifier.Private
								),
								Type(
									Priority.Knockout,
									ICheck.GetCurrentEntity
								)
							)
						)
					}
				));
		
		return generateStepsList;
	}
}
```