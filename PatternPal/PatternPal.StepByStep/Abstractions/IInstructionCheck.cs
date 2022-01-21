using PatternPal.Recognizers.Abstractions;

namespace PatternPal.StepByStep.Abstractions
{
    public interface IInstructionCheck
    {
        ICheckResult Correct(IInstructionState state);
    }
}
