using PatternPal.Core.StepByStep;
using PatternPal.SyntaxTree;
using PatternPal.SyntaxTree.Abstractions.Entities;

namespace PatternPal.Services;

public class StepByStepService : Protos.StepByStepService.StepByStepServiceBase
{
    public override Task< GetInstructionSetsResponse > GetInstructionSets(
        GetInstructionSetsRequest request,
        ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    public override Task< GetSelectableClassesResponse > GetSelectableClasses(
        GetSelectableClassesRequest request,
        ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    public override Task< GetInstructionByIdResponse > GetInstructionById(
        GetInstructionByIdRequest request,
        ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    public override Task< CheckInstructionResponse > CheckInstruction(
        CheckInstructionRequest request,
        ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    private static CheckResult CreateCheckResult(
        Recognizers.Abstractions.ICheckResult checkResult)
    {
        throw new NotImplementedException();
    }

    private static class State
    {
        internal static readonly Dictionary< string, string > StateKeyed = new();
    }
}
