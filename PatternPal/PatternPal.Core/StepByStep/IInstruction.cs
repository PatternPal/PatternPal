namespace PatternPal.Core.StepByStep;

public interface IInstruction
{
    /// <summary>
    /// Requirement the step needs to fulfill.
    /// </summary>
    string Requirement { get; }

    /// <summary>
    /// Text that explains why the requirement needs to be fulfilled.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Checks that need to be passed to continue.
    /// </summary>
    List< ICheck > Checks { get; }

    /// <summary>
    /// The file for which the <see cref="Checks"/> need to run on.
    /// </summary>
    string FileId { get; set; }
}
