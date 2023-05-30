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
internal abstract class AdapterRecognizerAbstractClass : AdapterRecognizerParent
{
    public override ClassCheck IsInterfaceAbstractClassWithMethod(MethodCheck method)
    {
        return AbstractClass(
            Priority.Knockout,
            Modifiers(
                Priority.Knockout,
                Modifier.Abstract),
            method
        );
    }

    public override MethodCheck ContainsMaybeAbstractMethod()
    {
        return Method(
            Priority.High,
            Modifiers(
                Priority.High,
                Modifier.Abstract
            )
        );
    }

    public override RelationCheck DoesInheritFrom(ICheck possibleParent)
    {
        return new RelationCheck(
            Priority.Knockout,
            RelationType.Extends,
            possibleParent
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
                    (ClassCheck)service
                )
            );
        }
    }
}
