#region

using PatternPal.Core.StepByStep;
using PatternPal.Core.StepByStep.Resources.Instructions;
using PatternPal.SyntaxTree.Models;
using PatternPal.SyntaxTree.Models.Entities;
using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers;

/// <summary>
/// A <see cref="IRecognizer"/> that is used to determine if the provided files or project implements the state pattern
/// </summary>
/// <remarks>
/// Requirements for the Context class:<br/>
///       a) contains a field of type state<br/>
///       b) contains a changeState() method which changes the state field<br/>
///       c) contains a method which calls a method of the state field<br/>
///       d) has either:<br/>
///             1) a constructor with a state as parameter and sets the concretestrategy field<br/>
///             2) a constructor which creates and sets the concretestrategy field<br/>
/// Requirements for the State interface:<br/>
///       a) is an interface<br/>
///       b) gets inherited by one class<br/>
///       c) gets inherited by two classes<br/>                                                     --Todo: check this, not possible with current checkbase
///       d) contains a method<br/>
/// Requirements for the Concrete State classes:<br/>
///       a) inherits state<br/>
///       b) contain a field of type context<br/>
/// Requirements for the Client class:<br/>
///       a) creates a context object<br/>
///       b) change the state via the changeState() method<br/>
///       c) calls method of state via context<br/>
/// </remarks>
internal class StateRecognizer : IRecognizer
{
    /// <inheritdoc cref="IRecognizer" />
    public string Name => "State";

    /// <inheritdoc cref="IRecognizer" />
    public Recognizer RecognizerType => Recognizer.State;

    /// <inheritdoc />
    IEnumerable< ICheck > IRecognizer.Create()
    {

        // concrete state b
        //Not checkable


        // state d
        MethodCheck containsMethod = Method(Priority.Knockout);

        // state a
        InterfaceCheck stateInterface = Interface(
            Priority.Knockout,
            containsMethod);

        // concrete state a & state b
        RelationCheck inheritsState = Inherits(
            Priority.Knockout,
            state
        );

        ClassCheck concreteState = Class(
            Priority.Low,
            inheritsState
        );

        // context a
        FieldCheck fieldOfTypeState = Field(
            Priority.Knockout,
            state
        );

        // context b
        MethodCheck changeState = Method(
            Priority.Knockout,
            Uses(
                Priority.Knockout,
                fieldOfTypeState
            )
        );

        // context c
        MethodCheck callsStateField = Method(
            Priority.Mid,
            Uses(
                Priority.Mid,
                fieldOfTypeState));

        // context d
        ICheck contextConstructor = Any(
            Priority.Mid,
            Constructor(
                Priority.Mid,
                Parameters(
                    Priority.Mid,
                    Type(
                        Priority.Mid,
                        state
                    )
                ),
                Uses(
                    Priority.Mid,
                    fieldOfTypeState
                )
            ),
            Constructor(
                Priority.Mid,
                Creates(
                    Priority.Mid,
                    fieldOfTypeState
                ),
                Uses(
                    Priority.Mid,
                    fieldOfTypeState
                )
            )
        );

        ClassCheck context = Class(
            Priority.Low,
            fieldOfTypeState,
            changeState,
            callsStateField,
            contextConstructor
        );

        // client a
        RelationCheck createsContext;

        // client b
        RelationCheck callsChangeStateMethod;

        // client c
        RelationCheck callsMethodOfStateViaContext;

        return new ICheck[] { };
    }

    InterfaceCheck state = Interface(
        Priority.Knockout,
        Method(
            Priority.Knockout
        )
        
    );

    
}
