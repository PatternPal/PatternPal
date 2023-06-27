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

        if (results.Count == 0)
        {
            return Task.FromResult(
                new CheckInstructionResponse
                {
                    Result = false
                });
        }

        RecognizeResult result = ResultsHelper.CreateRecognizeResult(results.First());

        return Task.FromResult(
            new CheckInstructionResponse
            {
                Result = result.PercentageCorrectResults == 100,
                RecognizeResult = result
            });
    }
}
