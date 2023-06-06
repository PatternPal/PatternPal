#region

using System.ComponentModel;
using PatternPal.SyntaxTree.Models;

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers.Helper_Classes;

/// <summary>
/// A subclass of <see cref="AdapterRecognizerParent"/> that is used to specify checks for the adapter implementation with a Client Interface with type <see langword="interface"/>.
/// </summary>
internal class AdapterRecognizerInterface : AdapterRecognizerParent
{
    public override InterfaceCheck IsInterfaceAbstractClassWithMethod(MethodCheck method)
    {
        return Interface(
            Priority.Knockout,
            method
        );
    }

    public override MethodCheck ContainsMaybeAbstractVirtualMethod()
    {
        return Method(Priority.High);
    }

    public override RelationCheck DoesInheritFrom(ICheck parent)
    {
        return Implements(
                Priority.Knockout,
                parent
        );
    }
}
