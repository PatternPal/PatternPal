#region

using PatternPal.Core.StepByStep;
using PatternPal.Services.Helpers;

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
        Recognizer protoRecognizer = request.Recognizers;
        IStepByStepRecognizer recognizer = RecognizerRunner.SupportedStepByStepRecognizers[ protoRecognizer ];
        IInstruction instruction = recognizer.GenerateStepsList()[ (int)request.InstructionNumber - 1 ];
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
        // Due to some limitations in the score calculation, we unfortunately can't rely on that to
        // determine if the result is correct. Instead we fallback to the old way of determining
        // that, which is to run with prune all and check if any results remain. This does mean that
        // the recognizer technically runs twice, which is inefficient. However, because a
        // Step-by-Step implementation is usually quite small, this doesn't really matter.
        bool correct = GetCorrect(request);
        RecognizeResult ? recognizeResult = GetResult(request);

        CheckInstructionResponse response = new()
                                            {
                                                Result = correct,
                                                RecognizeResult = recognizeResult,
                                            };

        return Task.FromResult(response);
    }

    /// <summary>
    /// Checks whether the instruction has been correctly implemented.
    /// </summary>
    /// <param name="request">The <see cref="CheckInstructionRequest"/> to check.</param>
    /// <returns><see langword="true"/> if the instruction has been implemented correctly, <see langword="false"/> otherwise.</returns>
    private static bool GetCorrect(
        CheckInstructionRequest request)
    {
        // Retrieve the checks based on the instruction number and recognizer.
        Recognizer protoRecognizer = request.Recognizer;
        IStepByStepRecognizer recognizer = RecognizerRunner.SupportedStepByStepRecognizers[ protoRecognizer ];
        IInstruction instruction = recognizer.GenerateStepsList()[ request.InstructionNumber ];

        // Run the checks on the provided files.
        RecognizerRunner runner = new(
            protoRecognizer,
            request.Documents,
            instruction );
        IList< RecognizerRunner.RunResult > results = runner.Run(pruneAll: true);

        // Check whether there is a single file with results.
        if (results.Count == 0)
        {
            return false;
        }

        // Assume the implementation is wrong only when there is evidence in one of the
        // results that proves otherwise.
        bool assumption = false;
        foreach (RecognizerRunner.RunResult runResult in results)
        {
            NodeCheckResult checkRes = (NodeCheckResult)runResult.CheckResult;
            if (checkRes.ChildrenCheckResults.Count != 0)
            {
                assumption = true;
            }
        }

        return assumption;
    }

    /// <summary>
    /// Create a <see cref="RecognizeResult"/> from the instruction implementation.
    /// </summary>
    /// <param name="request">The <see cref="CheckInstructionRequest"/> to check.</param>
    /// <returns>The <see cref="RecognizeResult"/> if the <see cref="RecognizerRunner"/> produced a result, <see langword="null"/> otherwise.</returns>
    private static RecognizeResult ? GetResult(
        CheckInstructionRequest request)
    {
        // Retrieve the checks based on the instruction number and recognizer.
        Recognizer protoRecognizer = request.Recognizer;
        IStepByStepRecognizer recognizer = RecognizerRunner.SupportedStepByStepRecognizers[ protoRecognizer ];
        IInstruction instruction = recognizer.GenerateStepsList()[ request.InstructionNumber ];

        // Run the checks on the provided files.
        RecognizerRunner runner = new(
            recognizer.RecognizerType,
            request.Documents,
            instruction );
        IList< RecognizerRunner.RunResult > results = runner.Run();

        return results.Count == 0
            ? null
            : ResultsHelper.CreateRecognizeResult(results.First());
    }
}
