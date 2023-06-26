﻿#region

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
        Recognizer protoRecognizer = request.Recognizers;
        IStepByStepRecognizer recognizer = RecognizerRunner.SupportedStepByStepRecognizers[ protoRecognizer ];
        IInstruction instruction = recognizer.GenerateStepsList()[ (int)request.InstructionNumber -1];
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
        IList< RecognizerRunner.RunResult > res = runner.Run(pruneAll: true);
        // Check whether there is a single file with results.
        if (res.Any())
        {
            // Assume the implementation is wrong only when there is evidence in one of the
            // results that prove otherwise.
            bool assumption = false;
            foreach (RecognizerRunner.RunResult runResult in res)
            {
                NodeCheckResult checkRes = (NodeCheckResult)runResult.CheckResult;
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
}
