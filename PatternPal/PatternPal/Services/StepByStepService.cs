#region

using PatternPal.Core.StepByStep;

using InstructionSet = PatternPal.Protos.InstructionSet;

#endregion

namespace PatternPal.Services;

public class StepByStepService : Protos.StepByStepService.StepByStepServiceBase
{
    /// <inheritdoc />
    public override Task< GetInstructionSetsResponse > GetInstructionSets(
        GetInstructionSetsRequest request,
        ServerCallContext context)
    {
        GetInstructionSetsResponse response = new();

        foreach (Recognizer recognizer in RecognizerRunner.SupportedStepByStepRecognizers.Keys)
        {
            response.Recognizers.Add(recognizer);
        }

        return Task.FromResult(response);
    }

    /// <inheritdoc />
    public override Task< GetInstructionSetResponse > GetInstructionSet(
        GetInstructionSetRequest request,
        ServerCallContext context)
    {
        // Provided with the recognizer return information on the instruction set required by the
        // InstructionsViewModel to navigate between steps.
        Recognizer providedRecognizer = request.Recognizer;
        IStepByStepRecognizer recognizer = RecognizerRunner.SupportedStepByStepRecognizers[ providedRecognizer ];
        List< IInstruction > instructions = recognizer.GenerateStepsList();

        InstructionSet res =
            new()
            {
                Name = recognizer.Name,
                NumberOfInstructions = (uint)instructions.Count
            };

        GetInstructionSetResponse response = new()
                                             {
                                                 SelectedInstructionset = res
                                             };

        return Task.FromResult(response);
    }

    /// <inheritdoc />
    public override Task< GetInstructionByIdResponse > GetInstructionById(
        GetInstructionByIdRequest request,
        ServerCallContext context)
    {
        // Provided with the recognizer and instruction number return the instruction detailed
        // information for display to the user.
        Recognizer protorecognizer = request.Recognizers;
        IStepByStepRecognizer recognizer = RecognizerRunner.SupportedStepByStepRecognizers[ protorecognizer ];
        IInstruction instruction = recognizer.GenerateStepsList()[ (int)request.InstructionNumber ];
        GetInstructionByIdResponse response = new()
                                              {
                                                  Instruction = new Instruction
                                                                {
                                                                    Title = instruction.Requirement,
                                                                    Description = instruction.Description,
                                                                    FileId = ""
                                                                }
                                              };

        return Task.FromResult(response);
    }

    /// <inheritdoc />
    public override Task< CheckInstructionResponse > CheckInstruction(
        CheckInstructionRequest request,
        ServerCallContext context)
    {
        // Retrieve the checks based on the instruction number and recognizer.
        Recognizer protoRecognizer = request.Recognizer;
        IStepByStepRecognizer recognizer = RecognizerRunner.SupportedStepByStepRecognizers[ protoRecognizer ];
        IInstruction instruction = recognizer.GenerateStepsList()[ request.InstructionNumber ];

        // Run the checks on the provided files.
        RecognizerRunner runner = new(
            request.Documents,
            instruction );
        IList< (Recognizer, ICheckResult) > res = runner.Run(pruneAll: true);
        // Check whether there is a single file with results.
        if (res.Any())
        {
            // Assume the implementation is wrong only when there is evidence in one of the
            // results that prove otherwise.
            bool assumption = false;
            foreach ((_, ICheckResult currentRes) in res)
            {
                NodeCheckResult checkRes = (NodeCheckResult)currentRes;
                if (checkRes.ChildrenCheckResults.Count != 0)
                {
                    assumption = true;
                }
            }

            return Task.FromResult(
                new CheckInstructionResponse
                {
                    Result = assumption
                });
        }
        // There was no content in the file that was able to run in the runner.
        else
        {
            return Task.FromResult(
                new CheckInstructionResponse
                {
                    Result = false
                });
        }
    }

    // TODO Method can still be useful, common.proto CheckResult is based on old codebase.
    /// <inheritdoc />
    private static CheckResult CreateCheckResult(
        ICheckResult checkResult)
    {
        throw new NotImplementedException();
        //CheckResult newCheckResult = new()
        //{
        //    FeedbackType = (CheckResult.Types.FeedbackType)((int)checkResult.GetFeedbackType() + 1),
        //    Hidden = checkResult.IsHidden,
        //    FeedbackMessage = ResourceUtils.ResultToString(checkResult),
        //};
        //foreach (Recognizers.Abstractions.ICheckResult childCheckResult in checkResult.GetChildFeedback())
        //{
        //    newCheckResult.SubCheckResults.Add(CreateCheckResult(childCheckResult));
        //}
        //return newCheckResult;
    }
}
