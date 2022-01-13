namespace IDesign.StepByStep.Abstractions
{
    public interface IInstructionCheck
    {
        bool Correct(IInstructionState state);
    }
}
