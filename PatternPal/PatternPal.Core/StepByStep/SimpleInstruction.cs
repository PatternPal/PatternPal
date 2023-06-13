namespace PatternPal.Core.StepByStep;

/// <summary>
/// Represents a single step in a <see cref="IStepByStepRecognizer"/> implementation.
/// </summary>
public class SimpleInstruction : IInstruction
{
    /// <inheritdoc />
    public string Requirement { get; }

    /// <inheritdoc />
    public string Description { get; }

    /// <inheritdoc />
    public List< ICheck > Checks { get; }

    /// <summary>
    /// Creates a new <see cref="SimpleInstruction"/> instance.
    /// </summary>
    /// <param name="requirement">The requirement for this <see cref="IInstruction"/>.</param>
    /// <param name="description">The description of this <see cref="IInstruction"/>.</param>
    /// <param name="checks">The <see cref="ICheck"/>s which make up this <see cref="IInstruction"/>.</param>
    public SimpleInstruction(
        string requirement,
        string description,
        List< ICheck > checks)
    {
        Requirement = requirement;
        Description = description;
        Checks = checks;
    }
}
