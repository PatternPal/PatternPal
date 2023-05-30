namespace PatternPal.Core.Recognizers;

/// <summary>
/// Represents a recognizer for a design pattern.
/// </summary>
public interface IRecognizer
{
    /// <summary>
    /// The name of the design pattern which this <see cref="IRecognizer"/> recognizes.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The type of the <see cref="IRecognizer"/>, as defined in the Protocol Buffer.
    /// </summary>
    public Recognizer RecognizerType { get; }

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
