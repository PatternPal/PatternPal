#region

using System.ComponentModel;
using PatternPal.SyntaxTree.Models;

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers.Helper_Classes;

/// <summary>
/// A subclass of <see cref="AdapterRecognizerParent"/> that is used to specify checks for the adapter implementation with a Client Interface with type <see langword="abstract class"/>.
/// </summary>
internal class AdapterRecognizerAbstractClass : AdapterRecognizerParent
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

    public override MethodCheck ContainsMaybeAbstractVirtualMethod()
    {
        return Method(
            Priority.High,
            Any(
                Priority.High,
                Modifiers(
                    Priority.High,
                    Modifier.Abstract
                ),
                Modifiers(
                    Priority.High,
                    Modifier.Virtual
                )
            )
        );
    }

    public override RelationCheck DoesInheritFrom(ICheck possibleParent)
    {
        return Inherits(
            Priority.Knockout,
            possibleParent
        );
    }
}
