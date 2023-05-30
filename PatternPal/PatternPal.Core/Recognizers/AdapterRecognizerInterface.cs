#region

using System.ComponentModel;
using PatternPal.SyntaxTree.Models;

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers;

/// <summary>
/// A <see cref="IRecognizer"/> that is used to determine if the provided files or project implements the adapter pattern
/// </summary>
/// <remarks>
/// Requirements to fulfill the pattern:<br/>
/// </remarks>
internal class AdapterRecognizerInterface : AdapterRecognizerParent
{
    public override InterfaceCheck IsInterfaceAbstractClassWithMethod(MethodCheck method)
    {
        return Interface(
            Priority.Knockout,
            method
        );
    }

    public override MethodCheck ContainsMaybeAbstractMethod()
    {
        return Method(Priority.High);
    }

    public override RelationCheck DoesInheritFrom(ICheck parent)
    {
        return new RelationCheck(
                Priority.Knockout,
                RelationType.Implements,
                parent
            
        );
    }

    public override FieldCheck ContainsServiceField(ICheck service)
    {
        {
            return Field(
                Priority.Knockout,
                Modifiers(
                    Priority.Knockout,
                    Modifier.Private
                ),
                Type(
                    Priority.Knockout,
                    (InterfaceCheck)service
                )
            );
        }
    }
}
