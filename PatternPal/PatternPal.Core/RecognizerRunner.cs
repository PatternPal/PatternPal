#region

using Microsoft.CodeAnalysis;

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
                                 };

        NodeCheckResult rootResult = (NodeCheckResult)rootCheck.Check(
            ctx,
            new RootNode());

        // TODO: Define least and most dependable and reference that definition here.

        // Sort the check results from least to most dependable.
        SortCheckResults(rootResult);

        // Filter the results.
        Dictionary< INode, List< ICheckResult > > resultsByNode = new();
        FilterResults(
            resultsByNode,
            rootResult);

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
    internal static bool FilterResults(
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
                    }
                    break;
                }
                case NodeCheckResult nodeCheckResult:
                {
                    // Filter the results recursively. If `FilterResults` returns true, the node
                    // check itself should also be pruned.
                    if (FilterResults(
                        resultsByNode,
                        nodeCheckResult))
                    {
                        resultsToBePruned.Add(nodeCheckResult);
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
                            }
                            break;
                        }
                        case NodeCheckResult nodeCheckResult:
                        {
                            // If `FilterResults` returns false, this means the node check shouldn't
                            // be pruned (because it has correct children). Because it's wrapped in
                            // a not check, this not check is incorrect and should be pruned if it
                            // has priority Knockout.
                            if (!FilterResults(
                                    resultsByNode,
                                    nodeCheckResult)
                                && notCheckResult.Priority == Priority.Knockout)
                            {
                                resultsToBePruned.Add(notCheckResult);
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
        if (resultsToBePruned.Count == parentCheckResult.ChildrenCheckResults.Count)
        {
            // Parent becomes empty.
            return true;
        }

        // Prune the results.
        foreach (ICheckResult checkResult in resultsToBePruned)
        {
            parentCheckResult.ChildrenCheckResults.Remove(checkResult);
        }

        if (parentCheckResult is
            {
                NodeCheckCollectionWrapper: true,
                Priority: Priority.Knockout,
                Check: RelationCheck relationCheck
            })
        {
            // We're currently processing the NodeCheckResult of a RelationCheck.

            foreach (ICheckResult relationCheckResult in parentCheckResult.ChildrenCheckResults)
            {
                if (relationCheckResult is not LeafCheckResult relationResult)
                {
                    throw new ArgumentException($"Unexpected check type '{relationCheckResult.GetType()}', expected '{typeof( LeafCheckResult )}'");
                }

            }
        }

        // Parent doesn't become empty, so it shouldn't be pruned by the caller.
        return false;
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
                                   ParentCheck = parentCheck
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
