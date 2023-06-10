namespace PatternPal.Core.StepByStep;

/// <summary>
/// Represents a Step-by-Step implementation of a <see cref="Recognizers.IRecognizer"/> for a design pattern.
/// </summary>
public interface IStepByStepRecognizer
{
    /// <summary>
    /// The name of the design pattern which this <see cref="IStepByStepRecognizer"/> recognizes.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The type of the <see cref="IStepByStepRecognizer"/>, as defined in the Protocol Buffer.
    /// </summary>
    public Recognizer RecognizerType { get; }

    /// <summary>
    /// Returns a list of <see cref="IInstruction"/>s that contain everything for a single step
    /// in the <see cref="StepByStep"/> module.
    /// </summary>
    /// <returns></returns>
    List< IInstruction > GenerateStepsList();
}
