using PatternPal.Core.Checks;
using PatternPal.Core.Runner;
using PatternPal.SyntaxTree.Abstractions.Entities;
using PatternPal.SyntaxTree.Abstractions.Members;

using IMethod = PatternPal.SyntaxTree.Abstractions.Members.IMethod;

namespace PatternPal.Services;

/// <inheritdoc cref="Protos.RecognizerService"/>
public class RecognizerService : Protos.RecognizerService.RecognizerServiceBase
{
    /// <inheritdoc />
    public override Task< GetSupportedRecognizersResponse > GetSupportedRecognizers(
        GetSupportedRecognizersRequest request,
        ServerCallContext context)
    {
        GetSupportedRecognizersResponse response = new();

        foreach (Recognizer supportedRecognizer in RecognizerRunner.SupportedRecognizers.Keys)
        {
            response.Recognizers.Add(supportedRecognizer);
        }

        return Task.FromResult(response);
    }

    /// <inheritdoc />
    public override Task Recognize(
        RecognizeRequest request,
        IServerStreamWriter< RecognizeResponse > responseStream,
        ServerCallContext context)
    {
        // TODO CV: Handle error cases

        // Get the list of files on which to run the recognizers.
        IList< string > ? files = GetFiles(request);
        if (files is null)
        {
            return Task.CompletedTask;
        }

        // Run the recognizers. We need to cast the result to a `List` to access the `Sort` method,
        // which is defined on the class, not on the `IList` contract.
        RecognizerRunner runner = new(
            files,
            request.Recognizers );
        IList< RecognizerRunner.RunResult > results = runner.Run();

        // KNOWN: The root check result is always a NodeCheckResult.
        foreach (RecognizerRunner.RunResult runResult in results)
        {
            RecognizeResult rootResult = new()
                                         {
                                             Recognizer = runResult.RecognizerType!.Value,
                                             Feedback = "Goed gedoet"
                                         };

            Dictionary< string, (Result Result, Func< IDictionary< ICheck, ICheckResult >, bool >) > resultsByRequirement = new();

            // Get all requirements for which we have results.
            IDictionary< ICheck, ICheckResult > resultsByCheck = new Dictionary< ICheck, ICheckResult >();
            foreach ((Result Result, Func< IDictionary< ICheck, ICheckResult >, bool >) result in GetResults(
                resultsByCheck,
                runResult.CheckResult))
            {
                if (!resultsByRequirement.TryGetValue(
                        result.Result.Requirement,
                        out (Result Result, Func< IDictionary< ICheck, ICheckResult >, bool >) existingResult)
                    || (existingResult.Result.MatchedNode == null && result.Result.MatchedNode != null))
                {
                    resultsByRequirement[ result.Result.Requirement ] = result;
                }
            }

            // Check which requirements have no results (because the results were pruned).
            foreach (string requirement in runResult.Requirements!)
            {
                if (!resultsByRequirement.TryGetValue(
                    requirement,
                    out (Result Result, Func< IDictionary< ICheck, ICheckResult >, bool >) foundResult))
                {
                    resultsByRequirement[ requirement ] = (new Result
                                                           {
                                                               Requirement = requirement,
                                                               Correctness = Result.Types.Correctness.CIncorrect,
                                                           }, _ => false);
                    continue;
                }
            }

            // TODO: Generate feedback for incorrect/missing requirements.
            int oldEntityCheck = 0;
            EntityResult entityResult = new();

            foreach ((Result result, Func< IDictionary< ICheck, ICheckResult >, bool > calcCorrect) in resultsByRequirement.Values.OrderBy(r => r.Result.Requirement))
            {
                string[] splittedRequirement = result.Requirement.Split(". ");
                if (int.TryParse(splittedRequirement[0], out int newEntityCheck) && oldEntityCheck != newEntityCheck)
                {
                    if (oldEntityCheck > 0)
                    {
                        rootResult.EntityResults.Add(entityResult);
                    }
                    oldEntityCheck = newEntityCheck;

                    entityResult = new EntityResult
                    {
                        Name = splittedRequirement[1],
                        MatchedNode = result.MatchedNode
                    };

                    if (!calcCorrect(resultsByCheck))
                    {
                        entityResult.MatchedNode = null;
                    }
                }
                else
                {
                    if (!calcCorrect(resultsByCheck))
                    {
                        result.MatchedNode = null;
                    }

                    int position = 0;
                    string isThisInt = splittedRequirement[0];
                    while (char.IsDigit(isThisInt[position]))
                    {
                        position++;
                    }

                    result.Requirement = splittedRequirement[0][position..] + ") " + splittedRequirement[1];
                    entityResult.Requirements.Add(result);
                }
            }

            if (entityResult.Requirements.Count > 0)
            {
                rootResult.EntityResults.Add(entityResult);
            }

            RecognizeResponse response = new()
                                         {
                                             Result = rootResult
                                         };
            responseStream.WriteAsync(response);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// This method transforms a <see cref="ICheckResult"/> to a <see cref="Result"/>
    /// </summary>
    /// <param name="resultsByCheck"></param>
    /// <param name="rootCheckResult"></param>
    /// <param name="resultsToProcess"></param>
    /// <param name="checkResult"> The check to transform.</param>
    /// <param name="usedNodes"></param>
    /// <returns></returns>
    private static IEnumerable< (Result, Func< IDictionary< ICheck, ICheckResult >, bool >) > GetResults(
        IDictionary< ICheck, ICheckResult > resultsByCheck,
        ICheckResult rootCheckResult)
    {
        Queue< ICheckResult > resultsToProcess = new();
        resultsToProcess.Enqueue(rootCheckResult);
        while (resultsToProcess.Count != 0)
        {
            ICheckResult resultToProcess = resultsToProcess.Dequeue();

            if (!string.IsNullOrWhiteSpace(resultToProcess.Check.Requirement))
            {

                Result result = new()
                                {
                                    Requirement = resultToProcess.Check.Requirement,
                                    Correctness = Result.Types.Correctness.CCorrect,
                                };

                if (resultToProcess.MatchedNode != null)
                {
                    INode matchedNode = resultToProcess.MatchedNode;
                    TextSpan sourceLocation = matchedNode.GetSourceLocation;
                    result.MatchedNode = new MatchedNode
                                         {
                                             Name = matchedNode.GetName(),
                                             Path = matchedNode.GetRoot().GetSource(),
                                             Start = sourceLocation.Start,
                                             Length = sourceLocation.Length,
                                             Kind = resultToProcess.MatchedNode switch
                                             {
                                                 IClass => MatchedNode.Types.MatchedNodeKind.MnkClass,
                                                 IInterface => MatchedNode.Types.MatchedNodeKind.MnkInterface,
                                                 IConstructor => MatchedNode.Types.MatchedNodeKind.MnkConstructor,
                                                 IMethod => MatchedNode.Types.MatchedNodeKind.MnkMethod,
                                                 IField => MatchedNode.Types.MatchedNodeKind.MnkField,
                                                 IProperty => MatchedNode.Types.MatchedNodeKind.MnkProperty,
                                                 _ => MatchedNode.Types.MatchedNodeKind.MnkUnknown,
                                             }
                                         };
                }

                result.Correctness = resultToProcess.Score.PercentageTo(resultToProcess.PerfectScore) switch
                {
                    100 => Result.Types.Correctness.CCorrect,
                    > 50 => Result.Types.Correctness.CPartiallyCorrect,
                    _ => Result.Types.Correctness.CIncorrect,
                };
                Func< IDictionary< ICheck, ICheckResult >, bool > calcCorrect = check2Result =>
                                                                                {
                                                                                    //Score perfectScore = resultToProcess.Check.PerfectScore(
                                                                                    //    check2Result,
                                                                                    //    resultToProcess);
                                                                                    //Score actualScore = resultToProcess.Score;

                                                                                    return resultToProcess.Score.Equals(resultToProcess.PerfectScore);
                                                                                    //return !perfectScore.Equals(default)
                                                                                    //       && perfectScore.Equals(actualScore);
                                                                                };
                yield return (result, calcCorrect);
            }

            switch (resultToProcess)
            {
                case LeafCheckResult:
                    continue;
                case NotCheckResult notCheckResult:
                    resultsToProcess.Enqueue(notCheckResult.NestedResult);
                    continue;
                case NodeCheckResult nodeCheckResult:
                    //bool first = true;
                    foreach (ICheckResult childCheckResult in nodeCheckResult.ChildrenCheckResults)
                    {
                        if (childCheckResult is NodeCheckResult {NodeCheckCollectionWrapper: true} childNodeCheckResult)
                        {
                            if (childNodeCheckResult.ChildrenCheckResults.FirstOrDefault() is NodeCheckResult matchedNodeCheckResult)
                            {
                                resultsByCheck[ matchedNodeCheckResult.Check ] = matchedNodeCheckResult;
                            }
                        }
                        //if (!((Dictionary< ICheck, ICheckResult >)resultsByCheck).TryAdd(
                        //    childCheckResult.Check,
                        //    childCheckResult))
                        //{
                        //    if (nodeCheckResult.NodeCheckCollectionWrapper && first)
                        //    {
                        //        resultsByCheck[ childCheckResult.Check ] = childCheckResult;
                        //        first = false;
                        //    }
                        //}
                        resultsToProcess.Enqueue(childCheckResult);
                    }
                    continue;
            }
        }
    }

    /// <summary>
    /// Gets the list of files on which to run the recognizers.
    /// </summary>
    /// <returns><see langword="null"/> if the request contains no valid file or project directory,
    /// or a list of files otherwise.</returns>
    private static IList< string > ? GetFiles(
        RecognizeRequest request)
    {
        switch (request.FileOrProjectCase)
        {
            case RecognizeRequest.FileOrProjectOneofCase.File:
            {
                if (string.IsNullOrWhiteSpace(request.File)
                    || Path.GetExtension(request.File) != ".cs")
                {
                    return null;
                }

                return new List< string >
                       {
                           request.File
                       };
            }
            case RecognizeRequest.FileOrProjectOneofCase.Project:
            {
                if (string.IsNullOrWhiteSpace(request.Project))
                {
                    return null;
                }

                string ? projectDirectory = Path.GetDirectoryName(request.Project);
                if (string.IsNullOrWhiteSpace(projectDirectory))
                {
                    return null;
                }

                return Directory.GetFiles(
                    projectDirectory,
                    "*.cs",
                    SearchOption.AllDirectories).ToList();
            }
            case RecognizeRequest.FileOrProjectOneofCase.None:
            default:
            {
                return null;
            }
        }
    }
}
