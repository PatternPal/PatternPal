﻿#region

using PatternPal.CommonResources;
using PatternPal.StepByStep;
using PatternPal.StepByStep.Abstractions;

using SyntaxTree;
using SyntaxTree.Abstractions.Entities;

#endregion

namespace PatternPal.Services;

public class PatternPalService : Protos.PatternPal.PatternPalBase
{
    private Dictionary< string, string > _keyed = new();

    public override Task Recognize(
        RecognizeRequest request,
        IServerStreamWriter< RecognizerResult > responseStream,
        ServerCallContext context)
    {
        string pathFile = request.File;
        string pathProject = request.Project;

        // TODO CV: Handle error cases
        if (string.IsNullOrWhiteSpace(pathFile)
            && string.IsNullOrWhiteSpace(pathProject))
        {
            return Task.CompletedTask;
            //TODO feedback message that no file was selected 
        }

        // Discriminate files based on file name extension
        if (!pathFile.EndsWith(".cs"))
        {
            //TODO programming language is not compatible add a feedback message for project check for .csproj?
        }

        List< string > files;
        // Return all the .cs files (and in all subdirectories)
        // Does not include files part of project but not in directory
        if (!string.IsNullOrEmpty(pathProject))
        {
            files = Directory.GetFiles(
                Path.GetDirectoryName(pathProject),
                "*.cs",
                SearchOption.AllDirectories).ToList();
        }
        else
        {
            files = new List< string >
                    {
                        request.File
                    };
        }

        RecognizerRunner runner = new();
        runner.CreateGraph(files);

        List< DesignPattern > patterns = new();
        foreach (Recognizer recognizer in request.Recognizers)
        {
            if (recognizer == Recognizer.Unknown)
            {
                continue;
            }

            patterns.Add(RecognizerRunner.GetDesignPattern(recognizer));
        }

        foreach (RecognitionResult result in runner.Run(patterns))
        {
            Result res = new()
                         {
                             Score = result.Result.GetScore()
                         };
            foreach (ICheckResult checkResult in result.Result.GetResults())
            {
                res.Results.Add(CreateCheckResult(checkResult));
            }

            RecognizerResult r = new RecognizerResult
                                 {
                                     DetectedPattern = result.Pattern.Name,
                                     ClassName = result.EntityNode.GetFullName(),
                                     Result = res,
                                 };
            responseStream.WriteAsync(r);
        }

        return Task.CompletedTask;
    }

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

    public override Task< RecognizerResult > CheckInstruction(
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
            _keyed[ fileSelector.FileId ] = request.SelectedItem;
        }

        IInstructionState state = new InstructionState();
        foreach (KeyValuePair< string, string > pair in _keyed)
        {
            state[ pair.Key ] = graph.GetAll()[ pair.Value ];
        }

        Result res = new();
        foreach (ICheckResult checkResult in instruction.Checks.Select(check => check.Correct(state)))
        {
            res.Results.Add(CreateCheckResult(checkResult));
        }

        RecognizerResult r = new()
                             {
                                 DetectedPattern = set.Name,
                                 //ClassName = result.EntityNode.GetFullName(),
                                 Result = res,
                             };

        bool correct = r.Result.Results.All(c => c.FeedbackType == FeedbackType.FeedbackCorrect);

        if (correct)
        {
            //Save all changed state to the state between instructions, only when all is successful
            foreach (KeyValuePair< string, IEntity > pair in state)
            {
                _keyed[ pair.Key ] = pair.Value.GetFullName();
            }
        }

        return Task.FromResult(r);
    }

    private CheckResult CreateCheckResult(
        ICheckResult checkResult)
    {
        CheckResult newCheckResult = new()
                                     {
                                         FeedbackType = (FeedbackType)((int)checkResult.GetFeedbackType() + 1),
                                         Hidden = checkResult.IsHidden,
                                         Score = checkResult.GetScore(),
                                         FeedbackMessage = ResourceUtils.ResultToString(checkResult),
                                     };
        foreach (ICheckResult childCheckResult in checkResult.GetChildFeedback())
        {
            newCheckResult.ChildFeedback.Add(CreateCheckResult(childCheckResult));
        }
        return newCheckResult;
    }
}
