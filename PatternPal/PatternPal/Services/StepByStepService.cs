#region

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

        // Obtain all the enum values in the proto.
        bool initialValue = true;
        foreach (Recognizer test in Enum.GetValues< Recognizer >())
        {
            if (initialValue)
            {
                initialValue = false;
                continue;
            }
            response.Recognizers.Add(test);
        }

        return Task.FromResult(response);
    }

    /// <inheritdoc />
    public override Task< GetInstructionSetResponse > GetInstructionSet(
        GetInstructionSetRequest request,
        ServerCallContext context)
    {
        IRecognizer recognizer = GetRecognizer(request.SelectedCombobox);
        List< IInstruction > instructions = recognizer.GenerateStepsList();

        InstructionSetHolder.instructionSet =
            new InstructionSet(
                recognizer,
                instructions);

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

        // Add not only the name of class but also the filepath
        GetSelectableClassesResponse selectableClasses = new();
        foreach ((string source, _) in graph.GetAll().OrderByDescending(p => File.GetLastWriteTime(p.Value.GetRoot().GetSource())))
        {
            selectableClasses.SelectableClasses.Add(source);
        }

        return Task.FromResult(selectableClasses);
    }

    /// <inheritdoc />
    public override Task< GetInstructionByIdResponse > GetInstructionById(
        GetInstructionByIdRequest request,
        ServerCallContext context)
    {
        IInstruction instructionSet = InstructionSetHolder.instructionSet.Steps[ (int)request.InstructionNumber ];
        GetInstructionByIdResponse response = new()
                                              {
                                                  Instruction = new Instruction
                                                                {
                                                                    Title = instructionSet.Requirement,
                                                                    Description = instructionSet.Description,
                                                                    FileId = ""
                                                                }
                                              };

        return Task.FromResult(response);
    }

    // TODO 
    /// <inheritdoc />
    public override Task< CheckInstructionResponse > CheckInstruction(
        CheckInstructionRequest request,
        ServerCallContext context)
    {
        // Make the graph based on the users opened solution or files.
        SyntaxGraph graph = new();
        foreach (string document in request.Documents)
        {
            graph.AddFile(
                FileManager.MakeStringFromFile(document),
                document);
        }
        graph.CreateGraph();

        Dictionary< string, IEntity > allEntities = graph.GetAll();
        string selectedNodePath = allEntities[ request.SelectedItem ].GetRoot().GetSource();

        RecognizerRunner runner = new(
            selectedNodePath,
            InstructionSetHolder.instructionSet.Steps[ request.InstructionId ] );
        IList< ICheckResult > res = runner.Run(pruneAll: true);

        throw new NotImplementedException();
        // Run runner with IChecks wrapped in something and use the "prune all" option that behaves 
        // the same as a list of IChecks with all priorities = knockout. The result should be "empty"
        // /no roots because of a wrong implementation if not it's not empty and the next step can be
        // selected.

        //NodeCheckResult rootCheckResult = (NodeCheckResult)rootcheck.Check(); 
        //MakeAllKnockoutPriority(rootCheckResult);
    }

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

    private static class State
    {
        internal static readonly Dictionary< string, string > StateKeyed = new();
    }

    /// <inheritdoc />
    public override Task< SetFilePathResponse > SetNewFilePath(
        SetFilePathRequest request,
        ServerCallContext context)
    {
        //throw new NotImplementedException();
        string filepath = request.FilePath;
        foreach (IInstruction current in InstructionSetHolder.instructionSet.Steps)
        {
            current.FileId = filepath;
        }

        var test = InstructionSetHolder.instructionSet;

        return Task.FromResult(new SetFilePathResponse());
    }

    public IRecognizer GetRecognizer(
        Recognizer recognizer)
    {
        IDictionary< Recognizer, IRecognizer > supportedRecognizers =
            new Dictionary< Recognizer, IRecognizer >();

        Type recognizerType = typeof( IRecognizer );

        // Find all types which derive from `IRecognizer`.
        foreach (Type type in recognizerType.Assembly.GetTypes().Where(ty => ty != recognizerType && recognizerType.IsAssignableFrom(ty)))
        {
            IRecognizer instance = (IRecognizer)Activator.CreateInstance(type)!;

            // Get the mapping from `Recognizer` to `IRecognizer` by invoking the property on the
            // recognizer which specifies it.
            supportedRecognizers.Add(
                (Recognizer)type.GetRuntimeProperty(nameof( IRecognizer.RecognizerType ))!.GetValue(instance)!,
                instance);
        }
        return supportedRecognizers[ recognizer ];
    }
}

public static class InstructionSetHolder
{
    public static InstructionSet instructionSet { get; set; }
}
