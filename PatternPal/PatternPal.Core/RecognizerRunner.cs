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

        // Get the design patterns which correspond to the given recognizers.
        _patterns = new List< DesignPattern >();
        foreach (Recognizer recognizer in recognizers)
        {
            // `Recognizer.Unknown` is the default value of the `Recognizer` enum, as required
            // by the Protocol Buffer spec. This value should never be used.
            if (recognizer == Recognizer.Unknown)
            {
                continue;
            }

            _patterns.Add(DesignPattern.SupportedPatterns[ ((int)recognizer) - 1 ]);
        }
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

        return rootResult;
    }

    /// <summary>
    /// Sorts the child <see cref="ICheckResult"/>s of a <see cref="NodeCheckResult"/> from least to most dependable, based on their <see cref="ICheckResult.DependencyCount"/>.
    /// </summary>
    /// <param name="result">The <see cref="NodeCheckResult"/> whose child <see cref="ICheckResult"/>s to sort.</param>
    private void SortCheckResults(
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
}

/// <summary>
/// Contains state for the <see cref="ICheck"/> which is currently being processed.
/// </summary>
internal interface IRecognizerContext
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
