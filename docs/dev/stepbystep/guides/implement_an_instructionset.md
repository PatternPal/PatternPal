# Implement an `InstructionSet`

To support a design pattern in PatternPal one must implement the @PatternPal.Core.StepByStep.IStepByStepRecognizer interface. 
This interface defines a `string` `Name`, and a `RecognizerType` as defined 
in the protocol buffer. and a method `GenerateStepsList`, which will 
return the object required for the Step-By-Step module.

# Efficiently add to `Recognizers` functionality

To make effective use of existing @PatternPal.Core.Recognizers for both detecting patterns and Step-By-Step 
it is recommended to encase all the @PatternPal.Core.Checks.ICheck created in the @PatternPal.Core.Recognizers `Create` method 
(more on this method can be found [here](~/dev/recognizers/guides/implement_recognizer.md)) into 
separate methods. This way the checks can be declared once and can be called in both `Create` in 
@PatternPal.Core.Recognizers.IRecognizer and in `GenerateStepsList` in @PatternPal.Core.StepByStep.IStepByStepRecognizer. 
The Recognizers would then inherit from both recognizer interfaces. Namely the @PatternPal.Core.Recognizers.IRecognizer and 
@PatternPal.Core.StepByStep.IStepByStepRecognizer interfaces.

An example could be a @PatternPal.Core.Checks.FieldCheck that tests if there is a field of the same type as the class the field 
is declared in. This method can then be called in the `Create` and `GenerateStepsList` methods. An example
of such can be seen here:

```csharp

IEnumerable< ICheck > IRecognizer.Create()
{
	...

	// Requirement 2 Singleton: Check for field of type class
	FieldCheck staticPrivateFieldOfTypeClass =
		StaticPrivateFieldOfTypeClass();

	...

	yield return Class(
		Priority.Knockout,
		...
		StaticPrivateFieldOfTypeClass(),
		...
	);
}
	
List< IInstruction > IStepByStepRecognizer.GenerateStepsList()
{
	List< IInstruction > generateStepsList = new();

	... 
	
	// Step 2: There is a static private field with the same type as the class
	generateStepsList.Add(
		new SimpleInstruction(
			"Create a field of the same type as the class",
			"The field is static since the Singleton should be stateless",
			new List< ICheck >
				{
					Class(
						Priority.Knockout,
						All(
							Priority.Low,
							StaticPrivateFieldOfTypeClass()
						)
					)
				}
		));
	
	...

	return generateStepsList;
}

internal FieldCheck StaticPrivateFieldOfTypeClass()
{
	return Field(
		Priority.Knockout,
		"has a static, private field with the same type as the class",
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
```

## Prerequisites 

First, it is of utmost importance that one has a clear idea of how much is asked of the user per step. It 
is  preferred to make the steps small and that they are supported by the available @PatternPal.Core.Checks.ICheck implementations.
(available checks can be found [here](~/dev/recognizers/design/checks.md)). Second, it is important to have explicit instructions to 
communicate to the user what is required to pass the step and optionally an explanation of why the 
requirement is the way it is. 

### Requirement and explanation per step
The goal is to make a step as small as possible. Defining a single class/interface, a single field, 
or a single method is considered a step. Defining a method can be more detailed because the behavior 
of the method needs to be described (return types, modifiers, parameters) while defining a field is the 
smallest step (including modifiers). Anything defined in one step should be closed in such a way that you
don't have to go back to the method/interface/field in a later step. 

The entity to be implemented (e.g. fields/methods/classes/interfaces) requires an explicit `Requirement` that
states explicitly what needs to be implemented and what the underlying @PatternPal.Core.Checks.ICheck will test for. For example: 
"Implement a static private field of the same type as the class". 

For education purposes, a `Description` of the step should be provided. It should explain why the step should
be implemented as instructed for example: "The field is static as a way of accessing the single instance when it is created.".

### Design pattern needs to be added to the API definition of the service
Much like the @PatternPal.Core.Recognizers.IRecognizer interface one must add a design pattern to the API definition. 
See [here](~/dev/recognizers/guides/implement_recognizer) for a guide.


## The `IStepByStepRecognizer` interface and library requirements

To support a Pattern for Step-By-Step one must add the following `using` statement to the top of the
file and implement the @PatternPal.Core.StepByStep.IStepByStepRecognizer:

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

## The `SimpleInstruction` object
Everything that is required by the program to retrieve information for a step and display that to the user 
is encased in a @PatternPal.Core.StepByStep.SimpleInstruction. These are the `Requirement`, `Description`, and a list of type @PatternPal.Core.Checks.ICheck. An 
example of a @PatternPal.Core.StepByStep.SimpleInstruction would be: 

```csharp
new SimpleInstruction(
                "Implement a...",
                "You have to implement it this way because...",
                new List< ICheck >
				{
					Class(
						Priority.Knockout,
						Field(
							Priority.Knockout,
							"Feedback message",
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
            )
```

## Implementing the `GenerateStepsList` method
The return type is a list of type @PatternPal.Core.StepByStep.IInstruction, these instructions require a `string Requirement` 
(the previously mentioned explicit instruction), `string Description` (the previously mentioned explanation), 
and a list of type @PatternPal.Core.Checks.ICheck that reflect the requirement of the step. 

The `Requirement` and `Description` are retrieved from the resources (`.resx` files). These are to be added 
to the `PatternPal.Core` project under the folder `StepByStep\Resources\Instructions`. For example: 
"`SingletonInstructions.resx`". Per step, a `Requirement` string and `Description` string can be added to 
this resource.

Creating an @PatternPal.Core.StepByStep.IInstruction should lastly be supplied with a list of type @PatternPal.Core.Checks.ICheck 
that need to pass in a step. These can be added to a list of instructions in `GenerateStepsList`. An example would be :

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

When the @PatternPal.Core.StepByStep.IStepByStepRecognizer interface is implemented it is automatically added as an option to the 
extension. It can then be selected from the combo box in the `StepByStepListView`.

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