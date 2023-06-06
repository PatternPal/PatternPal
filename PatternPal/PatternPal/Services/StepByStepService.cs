#region

using System.Collections.Generic;
using System.Reflection;
using PatternPal.Core.Checks;
using PatternPal.Core.Recognizers;
using PatternPal.Core.StepByStep;
using PatternPal.SyntaxTree;
using PatternPal.SyntaxTree.Abstractions.Entities;

using InstructionSet = PatternPal.Core.StepByStep.InstructionSet;

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
        
        IDictionary<Recognizer, IRecognizer> SBSDictionary = 
            RecognizerRunner.SupportedRecognizers;

        // Go over all the recognizers supported by the runner and remove those that do not implement
        // the GenerateStepsList function.
        foreach (KeyValuePair<Recognizer, IRecognizer> entry in SBSDictionary)
        {
            try
            {
                entry.Value.GenerateStepsList();
            }
            catch
            {
                SBSDictionary.Remove(entry.Key);
            }
        }

        response.Recognizers.Add(SBSDictionary.Keys);

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
        IRecognizer recognizer = RecognizerRunner.SupportedRecognizers[providedRecognizer];
        List< IInstruction > instructions = recognizer.GenerateStepsList();

        Protos.InstructionSet res =
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
        IRecognizer recognizer = RecognizerRunner.SupportedRecognizers[protorecognizer];
        IInstruction instruction = recognizer.GenerateStepsList()[(int)request.InstructionNumber];
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
        Recognizer protorecognizer = request.Recognizer;
        IRecognizer recognizer = RecognizerRunner.SupportedRecognizers[protorecognizer];
        IInstruction instruction = recognizer.GenerateStepsList()[(int)request.InstructionNumber];
        
        // Run the checks on the provided files.
        RecognizerRunner runner = new(
            request.Documents,
            instruction);
        IList<ICheckResult> res = runner.Run(pruneAll: true);
        // Check whether there is a single file with results.
        if (res.Any())
        {
            // Assume the implementation is wrong only when there is evidence in one of the
            // results that prove otherwise.
            bool assumption = false;
            ICheckResult correctCheckResult = null;
            foreach (ICheckResult currentRes in res)
            {
                NodeCheckResult checkRes = (NodeCheckResult)currentRes;
                if (checkRes.ChildrenCheckResults.Count != 0)
                {
                    assumption = true;
                    correctCheckResult = checkRes;
                }
            }

            return Task.FromResult(new CheckInstructionResponse
            {
                Result = assumption,
                RecognizeResult = new RecognizeResult
                {
                    ClassName = correctCheckResult.MatchedNode.ToString(),
                    Recognizer = request.Recognizer
                }
            });
        }
        // There was no content in the file that was able to run in the runner.
        else
        {
            return Task.FromResult(new CheckInstructionResponse
            {
                Result = false,
                RecognizeResult = new RecognizeResult
                {
                    ClassName = "There is no valid class",
                    Recognizer = request.Recognizer
                }
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
