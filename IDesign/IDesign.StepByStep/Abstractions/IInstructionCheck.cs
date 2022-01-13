using IDesign.Recognizers.Abstractions;

namespace IDesign.StepByStep.Abstractions
{
    public interface IInstructionCheck
    {
        ICheckResult Correct(IInstructionState state);
    }
}
