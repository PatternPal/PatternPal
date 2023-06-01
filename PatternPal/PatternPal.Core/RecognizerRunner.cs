#region

using System.Collections.Generic;
using System.IO.Pipelines;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternPal.Core.Models;
using PatternPal.Core.Recognizers;
using PatternPal.Protos;
using PatternPal.SyntaxTree;
using PatternPal.SyntaxTree.Abstractions.Root;

#endregion

namespace PatternPal.Core;

/// <summary>
/// This class is the driver which handles running the recognizers.
/// </summary>
public class RecognizerRunner
{
    // TODO: Refactor DesignPattern so its properties are defined directly on the IRecognizer implementation.
    private readonly IList< DesignPattern > _patterns;

    // The syntax graph of the code currently being recognized.
    private SyntaxGraph _graph;

    /// <summary>
    /// Create a new recognizer runner instance.
    /// </summary>
    /// <param name="files">The files to run the recognizers on.</param>
    /// <param name="recognizers">The recognizers to run.</param>
    public RecognizerRunner(
        IEnumerable< string > files,
        IEnumerable< Recognizer > recognizers)
    {
        CreateGraph(files);

        // Because we currently haven't implemented all recognizers, this will crash. To prevent
        // that, we skip this. When we have the recognizers reimplemented, this can be enabled
        // again.

        // Get the design patterns which correspond to the given recognizers.
        //_patterns = new List< DesignPattern >();
        //foreach (Recognizer recognizer in recognizers)
        //{
        //    // `Recognizer.Unknown` is the default value of the `Recognizer` enum, as required
        //    // by the Protocol Buffer spec. This value should never be used.
        //    if (recognizer == Recognizer.Unknown)
        //    {
        //        continue;
        //    }

        //    _patterns.Add(DesignPattern.SupportedPatterns[ ((int)recognizer) - 1 ]);
        //}
    }

    /// <summary>
    /// Create a new recognizer runner instance.
    /// </summary>
    /// <param name="files">The files to run the recognizers on.</param>
    /// <param name="patterns">The design patterns for which to run the recognizers.</param>
    public RecognizerRunner(
        IEnumerable< string > files,
        IList< DesignPattern > patterns)
    {
        CreateGraph(files);
        _patterns = patterns;
    }

    /// <summary>
    /// Creates a <see cref="SyntaxGraph"/> from the given files.
    /// </summary>
    /// <param name="files">The files from which to create a <see cref="SyntaxGraph"/></param>
    private void CreateGraph(
        IEnumerable< string > files)
    {
        _graph = new SyntaxGraph();
        foreach (string file in files)
        {
            string content = FileManager.MakeStringFromFile(file);
            _graph.AddFile(
                content,
                file);
        }
        _graph.CreateGraph();
    }

    /// <summary>
    /// Run the recognizers.
    /// </summary>
    /// <returns>A list of <see cref="RecognitionResult"/>, one per given design pattern.</returns>
    public IList< RecognitionResult > Run()
    {
        // If the graph is empty, we don't have to do any work.
        if (_graph.IsEmpty)
        {
            return new List< RecognitionResult >();
        }

        SingletonRecognizer recognizer = new();

        ICheck rootCheck = new NodeCheck< INode >(
            Priority.Knockout,
            recognizer.Create());

        IRecognizerContext ctx = new RecognizerContext
                                 {
                                     Graph = _graph,
                                     CurrentEntity = null!,
                                     ParentCheck = rootCheck,
                                     EntityCheck = rootCheck
                                 };

        rootCheck.Check(
            ctx,
            new RootNode());

        return new List< RecognitionResult >();
    }

    /// <summary>
    /// Runs the configured <see cref="IRecognizer"/>s.
    /// </summary>
    /// <returns>The result of the <see cref="IRecognizer"/>, or <see langword="null"/> if the <see cref="SyntaxGraph"/> is empty.</returns>
    public ICheckResult ? RunV2()
    {
        // If the graph is empty, we don't have to do any work.
        if (_graph.IsEmpty)
        {
            // TODO: Return empty result?
            return null;
        }

        IRecognizer recognizer = new SingletonRecognizer();

        ICheck rootCheck = recognizer.CreateRootCheck();

        IRecognizerContext ctx = new RecognizerContext
                                 {
                                     Graph = _graph,
                                     CurrentEntity = null!,
                                     ParentCheck = rootCheck,
                                     EntityCheck = rootCheck
                                 };

        NodeCheckResult rootResult = (NodeCheckResult)rootCheck.Check(
            ctx,
            new RootNode());

        // TODO: Define least and most dependable and reference that definition here.

        // Sort the check results from least to most dependable.
        SortCheckResults(rootResult);

        // Filter the results.
        Dictionary< INode, List< ICheckResult > > resultsByNode = new();
        PruneResults(
            resultsByNode,
            rootResult);

        PrioritySort(rootResult, resultsByNode);

        return rootResult;
    }

    /// <summary>
    /// Sorts the child <see cref="ICheckResult"/>s of a <see cref="NodeCheckResult"/> from least to most dependable, based on their <see cref="ICheckResult.DependencyCount"/>.
    /// </summary>
    /// <param name="result">The <see cref="NodeCheckResult"/> whose child <see cref="ICheckResult"/>s to sort.</param>
    internal static void SortCheckResults(
        NodeCheckResult result)
    {
        // Sort the child results of `result` recursively.
        foreach (NodeCheckResult nodeCheckResults in result.ChildrenCheckResults.OfType< NodeCheckResult >())
        {
            SortCheckResults(nodeCheckResults);
        }

        // Sort the child results of `result` itself.
        ((List< ICheckResult >)result.ChildrenCheckResults).Sort(
            (
                x,
                y) => x.DependencyCount.CompareTo(y.DependencyCount));
    }

    /// <summary>
    /// Filters the sub-<see cref="ICheckResult"/>s of <paramref name="parentCheckResult" /> by
    /// removing any results which can be safely pruned.
    /// </summary>
    /// <returns><see langword="true"/> if <paramref name="parentCheckResult"/> should also be pruned.</returns>
    /// <remarks>
    /// Results filtering is done in 2 passes.<br/>
    /// 1. Collect results to be pruned.<br/>
    /// 2. Prune results.<br/>
    /// <br/>
    /// Filtering is done recursively. Once we encounter a <see cref="NodeCheckResult"/>, we filter
    /// its sub-<see cref="ICheckResult"/>s recursively. If the <see cref="NodeCheckResult"/>
    /// becomes empty because all its sub-<see cref="ICheckResult"/>s are pruned, we can also prune
    /// the <see cref="NodeCheckResult"/> itself.
    /// </remarks>
    internal static bool PruneResults(
        Dictionary< INode, List< ICheckResult > > resultsByNode,
        NodeCheckResult parentCheckResult)
    {
        // TODO: Properly handle CheckCollectionKind.
        // TODO: Properly handle Priorities.

        // Pass 1: Collect results to be pruned.

        // The results which should be pruned.
        List< ICheckResult > resultsToBePruned = new();

        foreach (ICheckResult checkResult in parentCheckResult.ChildrenCheckResults)
        {
            if (checkResult.MatchedNode is not null)
            {
                if (!resultsByNode.TryGetValue(
                    checkResult.MatchedNode,
                    out List< ICheckResult > ? results))
                {
                    results = new List< ICheckResult >();
                    resultsByNode.Add(
                        checkResult.MatchedNode,
                        results);
                }

                results.Add(checkResult);
            }

            switch (checkResult)
            {
                case LeafCheckResult leafCheckResult:
                {
                    // If the leaf check is correct, it shouldn't be pruned.
                    if (leafCheckResult.Correct)
                    {
                        continue;
                    }

                    // Only prune the incorrect leaf check if its priority is Knockout.
                    if (leafCheckResult.Priority == Priority.Knockout)
                    {
                        resultsToBePruned.Add(leafCheckResult);
                        leafCheckResult.Pruned = true;
                    }
                    break;
                }
                case NodeCheckResult nodeCheckResult:
                {
                    // Filter the results recursively. If `PruneResults` returns true, the node
                    // check itself should also be pruned.
                    if (PruneResults(
                        resultsByNode,
                        nodeCheckResult))
                    {
                        resultsToBePruned.Add(nodeCheckResult);
                        nodeCheckResult.Pruned = true;
                    }
                    break;
                }
                case NotCheckResult notCheckResult:
                {
                    switch (notCheckResult.NestedResult)
                    {
                        case LeafCheckResult leafCheckResult:
                        {
                            // If the leaf check is incorrect, this means the not check is
                            // correct, so it shouldn't be pruned.
                            if (!leafCheckResult.Correct)
                            {
                                continue;
                            }

                            // If the leaf check is correct, the not check is incorrect. If the not
                            // check has priority Knockout, it should be pruned.
                            if (notCheckResult.Priority == Priority.Knockout)
                            {
                                resultsToBePruned.Add(notCheckResult);
                                notCheckResult.Pruned = true;
                            }
                            break;
                        }
                        case NodeCheckResult nodeCheckResult:
                        {
                            // TODO: Fix this using priorities (only works if all sub-checks are knockout).
                            // If `PruneResults` returns false, this means the node check shouldn't
                            // be pruned (because it has correct children). Because it's wrapped in
                            // a not check, this not check is incorrect and should be pruned if it
                            // has priority Knockout.
                            if (!PruneResults(
                                    resultsByNode,
                                    nodeCheckResult)
                                && notCheckResult.Priority == Priority.Knockout)
                            {
                                resultsToBePruned.Add(notCheckResult);
                                notCheckResult.Pruned = true;
                            }
                            break;
                        }
                        case NotCheckResult nestedNotCheckResult:
                        {
                            // TODO: Check this during check creation?
                            throw new ArgumentException("Nested not checks not supported");
                        }
                        default:
                            throw new ArgumentException(
                                $"Unknown check result '{notCheckResult.NestedResult}'",
                                nameof( notCheckResult.NestedResult ));
                    }
                    break;
                }
                default:
                    throw new ArgumentException(
                        $"Unknown check result '{checkResult}'",
                        nameof( checkResult ));
            }
        }

        // Pass 2: Prune results.

        // If parentCheck becomes empty => also prune parent. The parent is pruned by the caller if
        // we return `true`.
        if (resultsToBePruned.Count > 0 && (resultsToBePruned.Count == parentCheckResult.ChildrenCheckResults.Count
            || parentCheckResult.CollectionKind == CheckCollectionKind.All))
        {
            // Parent becomes empty.
            parentCheckResult.ChildrenCheckResults.Clear();
            return true;
        }

        if (parentCheckResult is
            {
                NodeCheckCollectionWrapper: true,
                Priority: Priority.Knockout,
                Check: RelationCheck or TypeCheck
            })
        {
            // We're currently processing the NodeCheckResult of a RelationCheck or TypeCheck. 

            foreach (ICheckResult dependentCheckResult in parentCheckResult.ChildrenCheckResults)
            {
                if (dependentCheckResult is not LeafCheckResult dependentResult)
                {
                    throw new ArgumentException($"Unexpected check type '{dependentCheckResult.GetType()}', expected '{typeof( LeafCheckResult )}'");
                }

                if (dependentResult.RelatedCheck is null)
                {
                    throw new ArgumentNullException(
                        nameof( dependentResult.RelatedCheck ),
                        $"RelatedCheck of CheckResult '{dependentResult}' is null, while it should reference the ICheck belonging to the related INode.");
                }

                // If the RelationCheck or TypeCheck did not match any nodes, it won't have any child results. As
                // such, we won't reach this code. If we do reach this code, but
                // `dependentResult.MatchedNode` is null, this is an error.
                if (dependentResult.MatchedNode is null)
                {
                    throw new ArgumentNullException(
                        nameof( dependentResult.MatchedNode ),
                        "Relation or Type check did not match any nodes");
                }

                // Get the CheckResults belonging to the related node.
                List< ICheckResult > resultsOfNode = resultsByNode[ dependentResult.MatchedNode ];

                // Find the CheckResult linked to the check which searched for the related node.
                ICheckResult ? relevantResult = resultsOfNode.FirstOrDefault(
                    result => result.Check == dependentResult.RelatedCheck);

                // This should not be possible for the same reason `dependentResult.MatchedNode`
                // should not be null.
                if (relevantResult is null)
                {
                    throw new ArgumentNullException(
                        nameof( relevantResult ),
                        "Relation or Type check did not match any nodes");
                }

                // If this node gets pruned, it does not have the required attributes to be the node belonging to the
                // check which searched for the related node. Therefore, even though the
                // relation might be found, it is not the relation directed to the node to which there should be one.
                // Therefore this relation is not the required one, and thus the dependentResult gets pruned.
                if (relevantResult.Pruned)
                {
                    resultsToBePruned.Add(dependentResult);
                    dependentResult.Pruned = true;
                }
            }
        }

        if (resultsToBePruned.Count > 0 && (resultsToBePruned.Count == parentCheckResult.ChildrenCheckResults.Count
                                            || parentCheckResult.CollectionKind == CheckCollectionKind.All))
        {
            // Parent becomes empty.
            parentCheckResult.ChildrenCheckResults.Clear();
            return true;
        }

        // Prune the results.
        foreach (ICheckResult checkResult in resultsToBePruned)
        {
            parentCheckResult.ChildrenCheckResults.Remove(checkResult);
        }

        // Parent doesn't become empty, so it shouldn't be pruned by the caller.
        return false;
    }

    /// <summary>
    /// Sorts a <see cref="NodeCheckResult"/> based on <see cref="Priority"/>s and whether the
    /// <see cref="LeafCheckResult"/>s are correct. It goes recursively through the tree of
    /// <see cref="ICheckResult"/>s, and gives a <see cref="Score"/> to each <see cref="ICheckResult"/>
    /// based their <see cref="Priority"/>s and the <see cref="Score"/>s and <see cref="Priority"/>s
    /// of their children.
    /// </summary>
    internal static void PrioritySort(NodeCheckResult result, Dictionary<INode, List<ICheckResult>> resultsByNode)
    {
        foreach (ICheckResult childResult in result.ChildrenCheckResults)
        {
            switch (childResult)
            {
                case LeafCheckResult leafResult:
                    leafResult.Score = Score.CreateScore(leafResult.Priority, leafResult.Correct);
                    //TODO add Relation and TypeCheck. Idea underneath. However not as straightforward since a RelationCheck and a TypeCheck are NodeCheckResults
                    /*if (leafResult is { Correct: true, Check: RelationCheck or TypeCheck })
                    {
                        // Get the CheckResults belonging to the related node.
                        List<ICheckResult> resultsOfNode = resultsByNode[leafResult.MatchedNode];

                        // Find the CheckResult linked to the check which searched for the related node.
                        ICheckResult? relevantResult = resultsOfNode.FirstOrDefault(
                            result => result.Check == leafResult.RelatedCheck);

                        leafResult.Score += relevantResult.Score;
                    }*/
                    break;
                case NodeCheckResult nodeResult:
                    PrioritySort(nodeResult, resultsByNode);
                    break;
                case NotCheckResult notResult:
                    switch (notResult.NestedResult)
                    {
                        case LeafCheckResult notLeafResult:
                            notLeafResult.Score = Score.CreateScore(notLeafResult.Priority, notLeafResult.Correct);
                            break;
                        case NodeCheckResult notNodeResult:
                            PrioritySort(notNodeResult, resultsByNode);
                            break;
                        default:
                            throw new ArgumentException($"{notResult.NestedResult} is not a supported ICheckResult");
                    }
                    notResult.Score = Score.GetNot(notResult.NestedResult.Priority, notResult.NestedResult.Score);
                    break;
                default:
                    throw new ArgumentException($"{childResult} is not a supported ICheckResult");
            }
            result.Score += childResult.Score;
        }

        ((List<ICheckResult>)result.ChildrenCheckResults).Sort((a, b) => a.Score.CompareTo(b.Score));
    }
}

/// <summary>
/// Contains state for the <see cref="ICheck"/> which is currently being processed.
/// </summary>
public interface IRecognizerContext
{
    /// <summary>
    /// The <see cref="SyntaxGraph"/> on which the <see cref="ICheck"/>s are being processed.
    /// </summary>
    internal SyntaxGraph Graph { get; }

    /// <summary>
    /// The current <see cref="IEntity"/> being processed.
    /// </summary>
    /// <remarks>
    /// Example: When the current <see cref="ICheck"/> is a <see cref="MethodCheck"/>,
    /// <see cref="CurrentEntity"/> contains the parent <see cref="IEntity"/> of the <see cref="IMethod"/>.
    /// </remarks>
    internal IEntity CurrentEntity { get; }

    /// <summary>
    /// The <see cref="ICheck"/> which is the direct parent of the current <see cref="ICheck"/>.
    /// </summary>
    internal ICheck ParentCheck { get; }

    /// <summary>
    /// The <see cref="ICheck"/> belonging to the <see cref="CurrentEntity"/>.
    /// </summary>
    internal ICheck EntityCheck { get; }

    /// <summary>
    /// Create a new <see cref="IRecognizerContext"/> instance from an existing one, overwriting the
    /// old properties with the new ones.
    /// </summary>
    /// <param name="oldCtx">The existing <see cref="IRecognizerContext"/> from which to copy properties.</param>
    /// <param name="currentNode">
    /// The current <see cref="INode"/> being processed. If <paramref name="currentNode"/> is an <see cref="IEntity"/>,
    /// it will be stored in <see cref="CurrentEntity"/>, otherwise the previous value of <see cref="CurrentEntity"/> is kept.
    /// </param>
    /// <param name="parentCheck">The current <see cref="ICheck"/>, of which the sub-<see cref="ICheck"/>s are going to be processed.</param>
    /// <returns>The created <see cref="IRecognizerContext"/>.</returns>
    internal static IRecognizerContext From(
        IRecognizerContext oldCtx,
        INode currentNode,
        ICheck parentCheck) => new RecognizerContext
                               {
                                   Graph = oldCtx.Graph,
                                   CurrentEntity = currentNode as IEntity ?? oldCtx.CurrentEntity,
                                   ParentCheck = parentCheck,
                                   EntityCheck = currentNode is IEntity ? parentCheck : oldCtx.EntityCheck
                               };
}

/// <summary>
/// Implementation of an <see cref="IRecognizerContext"/>. This type is <see langword="file"/>
/// scoped to ensure we keep control over when a new instance is created.
/// </summary>
file class RecognizerContext : IRecognizerContext
{
    /// <inheritdoc />
    public required SyntaxGraph Graph { get; init; }

    /// <inheritdoc />
    public required IEntity CurrentEntity { get; init; }

    /// <inheritdoc />
    public required ICheck ParentCheck { get; init; }

    /// <inheritdoc />
    public required ICheck EntityCheck { get; init; }
}

/// <summary>
/// This <see cref="INode"/> implementation serves as a sentinel value to be passed to the root <see
/// cref="ICheck"/>, without adding nullability into the APIs everywhere.
/// </summary>
file class RootNode : INode
{
    /// <inheritdoc />
    string INode.GetName() => throw new UnreachableException();

    /// <inheritdoc />
    SyntaxNode INode.GetSyntaxNode() => throw new UnreachableException();

    /// <inheritdoc />
    IRoot INode.GetRoot() => throw new UnreachableException();
}

/// <summary>
/// The Score of a <see cref="ICheckResult"/>, used to sort the final root <see cref="ICheckResult"/>.
/// </summary>
public struct Score : IComparable<Score>
{
    internal int High, Mid, Low;

    /// <summary>
    /// Adds up every component of the right <see cref="Score"/> to the left <see cref="Score"/>.
    /// </summary>
    public static Score operator +(Score a, Score b) => 
        new()
        {
            High = a.High + b.High,
            Mid = a.Mid + b.Mid,
            Low = a.Low + b.Low
        };

    /// <summary>
    /// Subtracts every component of the right <see cref="Score"/> to the left <see cref="Score"/>.
    /// </summary>
    public static Score operator -(Score a, Score b) => new Score
    {
        High = a.High - b.High,
        Mid = a.Mid - b.Mid,
        Low = a.Low - b.Low
    };

    /// <summary>
    /// Calculates and returns the <see cref="Score"/> property belonging to a <see cref="LeafCheckResult"/>.
    /// </summary>
    /// <param name="priority">The priority of the <see cref="LeafCheckResult"/></param>
    /// <param name="correct">Whether the <see cref="LeafCheckResult"/> was correct</param>
    internal static Score CreateScore(Priority priority, bool correct)
    {
        int score = correct ? 1 : 0;
        switch (priority)
        {
            case Priority.High:
                return new Score { High = score };
            case Priority.Mid:
                return new Score { Mid = score };
            case Priority.Low:
                return new Score { Low = score };
            default:
                throw new ArgumentException($"{priority} is an unhandled type of priority.");
        }
    }

    /// <summary>
    /// Calculates what the <see cref="Score"/> of the <see cref="NodeCheckResult"/> of a <see cref="NotCheck"/>,
    /// based on the <see cref="Score"/> of the <see cref="ICheckResult"/> of its <see cref="NotCheck.NestedCheck"/>.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of the parent <see cref="NotCheck"/></param>
    /// <param name="score">The <see cref="Score"/> of the computed <see cref="NotCheck.NestedCheck"/></param>
    /// <returns>The <see cref="Score"/> belonging to the parent <see cref="NotCheck"/></returns>
    internal static Score GetNot(Priority priority, Score score) =>
        new()
        {
            High = (priority == Priority.High && score.High == 0) ? 1 : 0,
            Mid = (priority == Priority.Mid && score.Mid == 0) ? 1 : 0,
            Low = (priority == Priority.Low && score.Low == 0) ? 1 : 0
        };

    /// <inheritdoc />>
    public int CompareTo(Score other)
    {
        if (High > other.High)
            return -1;
        if (High < other.High)
            return 1;
        if (Mid > other.Mid)
            return -1;
        if (Mid < other.Mid)
            return 1;
        if (Low > other.Low)
            return -1;
        if (Low < other.Low)
            return 1;
        return 0;
    }

    public override string ToString()
    {
        return $"High: {High}, Mid: {Mid}, Low: {Low}";
    }
}
