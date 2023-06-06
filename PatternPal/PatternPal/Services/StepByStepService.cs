#region

using System.Collections.Generic;
using System.Reflection;

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
        Recognizer protorecognizer = request.Recognizer;
        IRecognizer recognizer = RecognizerRunner.SupportedRecognizers[protorecognizer];
        IInstruction instruction = recognizer.GenerateStepsList()[(int)request.InstructionNumber];
        
        RecognizerRunner runner = new(
            request.Documents,
            instruction);
        IList<ICheckResult> res = runner.Run(pruneAll: true);
        if (res.Any())
        {
            bool assumption = false;
            foreach (ICheckResult currentRes in res)
            {
                NodeCheckResult checkRes = (NodeCheckResult)currentRes;
                if (checkRes.ChildrenCheckResults.Count != 0)
                {
                    assumption = true;
                }
            }
            return Task.FromResult(new CheckInstructionResponse
            {
                Result = assumption,
                RecognizeResult = new RecognizeResult
                {
                    ClassName = "",
                    Recognizer = request.Recognizer
                }
            });
        }
        else
        {
            return Task.FromResult(new CheckInstructionResponse
            {
                Result = false,
                RecognizeResult = new RecognizeResult
                {
                    ClassName = "",
                    Recognizer = request.Recognizer
                }
            });
        }
    }

    // TODO Method may still be useful depending on the presentation in the detector view
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
