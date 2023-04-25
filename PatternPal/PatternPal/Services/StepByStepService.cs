namespace PatternPal.Services;

public class StepByStepService : Protos.StepByStepService.StepByStepServiceBase
{
    public override Task< GetInstructionSetsResponse > GetInstructionSets(
        GetInstructionSetsRequest request,
        ServerCallContext context)
    {
        GetInstructionSetsResponse response = new();
        foreach ((string name, IInstructionSet set) in InstructionSetsCreator.InstructionSets)
        {
            response.InstructionSets.Add(
                new InstructionSet
                {
                    Name = name,
                    NumberOfInstructions = (uint)set.Instructions.Count(),
                });
        }

        return Task.FromResult(response);
    }

    public override Task< GetSelectableClassesResponse > GetSelectableClasses(
        GetSelectableClassesRequest request,
        ServerCallContext context)
    {
        SyntaxGraph graph = new();

        foreach (string document in request.Documents)
        {
            graph.AddFile(
                FileManager.MakeStringFromFile(document),
                document);
        }

        graph.CreateGraph();

        GetSelectableClassesResponse selectableClasses = new();
        foreach ((string source, _) in graph.GetAll().OrderByDescending(p => File.GetLastWriteTime(p.Value.GetRoot().GetSource())))
        {
            selectableClasses.SelectableClasses.Add(source);
        }

        return Task.FromResult(selectableClasses);
    }

    public override Task< GetInstructionByIdResponse > GetInstructionById(
        GetInstructionByIdRequest request,
        ServerCallContext context)
    {
        IInstructionSet set = InstructionSetsCreator.InstructionSets[ request.InstructionSetName ];
        IInstruction instruction = set.Instructions.ToList()[ request.InstructionId ];

        GetInstructionByIdResponse response = new()
                                              {
                                                  Instruction = new Instruction
                                                                {
                                                                    Title = instruction.Title,
                                                                    Description = instruction.Description,
                                                                    ShowFileSelector = instruction is IFileSelector,
                                                                    FileId = instruction is IFileSelector fileSelector
                                                                        ? fileSelector.FileId
                                                                        : string.Empty,
                                                                }
                                              };

        return Task.FromResult(response);
    }

    public override Task< CheckInstructionResponse > CheckInstruction(
        CheckInstructionRequest request,
        ServerCallContext context)
    {
        IInstructionSet set = InstructionSetsCreator.InstructionSets[ request.InstructionSetName ];
        IInstruction instruction = set.Instructions.ToList()[ request.InstructionId ];

        SyntaxGraph graph = new();

        foreach (string document in request.Documents)
        {
            graph.AddFile(
                FileManager.MakeStringFromFile(document),
                document);
        }

        graph.CreateGraph();

        if (instruction is IFileSelector fileSelector)
        {
            State.StateKeyed[ fileSelector.FileId ] = request.SelectedItem;
        }

        IInstructionState state = new InstructionState();
        foreach (KeyValuePair< string, string > pair in State.StateKeyed)
        {
            state[ pair.Key ] = graph.GetAll()[ pair.Value ];
        }

        RecognizeResult res = new()
                              {
                                  //Recognizer = 0,
                                  //ClassName = result.EntityNode.GetFullName(),
                              };

        throw new NotImplementedException();
        //foreach (ICheckResult checkResult in instruction.Checks.Select(check => check.Correct(state)))
        //{
        //    res.Results.Add(CreateCheckResult(checkResult));
        //}

        bool correct = res.Results.All(c => c.FeedbackType == CheckResult.Types.FeedbackType.FeedbackCorrect);

        if (correct)
        {
            //Save all changed state to the state between instructions, only when all is successful
            foreach (KeyValuePair< string, IEntity > pair in state)
            {
                State.StateKeyed[ pair.Key ] = pair.Value.GetFullName();
            }
        }

        CheckInstructionResponse response = new()
                                            {
                                                Result = res,
                                            };
        return Task.FromResult(response);
    }

    //private CheckResult CreateCheckResult(
    //    ICheckResult checkResult)
    //{
    //    CheckResult newCheckResult = new()
    //                                 {
    //                                     FeedbackType = (CheckResult.Types.FeedbackType)((int)checkResult.GetFeedbackType() + 1),
    //                                     Hidden = checkResult.IsHidden,
    //                                     Score = checkResult.GetScore(),
    //                                     FeedbackMessage = ResourceUtils.ResultToString(checkResult),
    //                                 };
    //    foreach (ICheckResult childCheckResult in checkResult.GetChildFeedback())
    //    {
    //        newCheckResult.SubCheckResults.Add(CreateCheckResult(childCheckResult));
    //    }
    //    return newCheckResult;
    //}

    private static class State
    {
        internal static Dictionary< string, string > StateKeyed = new();
    }
}
