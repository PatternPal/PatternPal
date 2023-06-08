#region
using static PatternPal.Core.Checks.CheckBuilder;
#endregion

namespace PatternPal.Core.Recognizers.Helper_Classes;

/// <summary>
/// A subclass of <see cref="AdapterRecognizerParent"/> that is used to specify checks for the adapter implementation with a Client Interface with type <see langword="interface"/>.
/// </summary>
internal class AdapterRecognizerInterface : AdapterRecognizerParent
{
    /// <inheritdoc />
    public override InterfaceCheck IsInterfaceOrAbstractClassWithMethod(MethodCheck method)
    {
        return Interface(
            Priority.Knockout,
            method
        );
    }

    /// <inheritdoc />
    public override MethodCheck ContainsOverridableMethod()
    {
        return Method(Priority.High);
    }

    /// <inheritdoc />
    public override RelationCheck DoesInheritFrom(ICheck parent)
    {
        return Implements(
                Priority.Knockout,
                parent
        );
    }
}
