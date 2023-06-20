#region

using PatternPal.SyntaxTree.Models;
using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers.Helper_Classes;

/// <summary>
/// A subclass of <see cref="AdapterRecognizerParent"/> that is used to specify checks for the adapter implementation with a Client Interface with type <see langword="abstract class"/>.
/// </summary>
internal class AdapterRecognizerAbstractClass : AdapterRecognizerParent
{
    /// <inheritdoc />
    public override ClassCheck IsInterfaceOrAbstractClassWithMethod(MethodCheck method)
    {
        return AbstractClass(
            Priority.Knockout,
            Modifiers(
                Priority.Knockout,
                Modifier.Abstract),
            method
        );
    }

    /// <inheritdoc />
    public override MethodCheck ContainsOverridableMethod()
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

    /// <inheritdoc />
    public override RelationCheck DoesInheritFrom(ICheck possibleParent)
    {
        return Inherits(
            Priority.Knockout,
            possibleParent
        );
    }
}
