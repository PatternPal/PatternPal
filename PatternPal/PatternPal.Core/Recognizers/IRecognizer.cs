namespace PatternPal.Core.Recognizers;

/// <summary>
/// Represents a recognizer for a design pattern.
/// </summary>
internal interface IRecognizer
{
    /// <summary>
    /// Creates the <see cref="ICheck"/>s which are used to recognize a design pattern.
    /// </summary>
    /// <returns>The created <see cref="ICheck"/>s.</returns>
    protected IEnumerable< ICheck > Create();

    /// <summary>
    /// Creates the root <see cref="ICheck"/> in which all <see cref="ICheck"/>s are contained.
    /// </summary>
    /// <returns>The root <see cref="ICheck"/>.</returns>
    internal ICheck CreateRootCheck() => new NodeCheck< INode >(
        Priority.Knockout,
        Create());
}
