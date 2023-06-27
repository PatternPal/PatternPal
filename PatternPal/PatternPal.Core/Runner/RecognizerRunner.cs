#region

using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;

using PatternPal.Core.Recognizers;
using PatternPal.Core.StepByStep;
using PatternPal.SyntaxTree;
using PatternPal.SyntaxTree.Abstractions.Root;

#endregion

namespace PatternPal.Core.Runner;

/// <summary>
/// This class is the driver which handles running the recognizers.
/// </summary>
public partial class RecognizerRunner
{
    /// <summary>
    /// The <see cref="IRecognizer"/>s which are currently supported.
    /// </summary>
    public static IDictionary< Recognizer, IRecognizer > SupportedRecognizers = null!;

    /// <summary>
    /// The <see cref="IStepByStepRecognizer"/>s which are currently supported.
    /// </summary>
    public static IDictionary< Recognizer, IStepByStepRecognizer > SupportedStepByStepRecognizers = null!;

    /// <summary>
    /// Finds the recognizers which are defined in this assembly and adds them to <see cref="SupportedRecognizers"/>.
    /// </summary>
    // See: https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca2255#when-to-suppress-warnings
#pragma warning disable CA2255 // The 'ModuleInitializer' attribute should not be used in libraries
    [ModuleInitializer]
#pragma warning restore CA2255 // The 'ModuleInitializer' attribute should not be used in libraries
    public static void ModuleInitializer()
    {
        SupportedRecognizers = new Dictionary< Recognizer, IRecognizer >();
        GetImplementingTypes(
            SupportedRecognizers,
            nameof( IRecognizer.RecognizerType ));

        SupportedStepByStepRecognizers = new Dictionary< Recognizer, IStepByStepRecognizer >();
        GetImplementingTypes(
            SupportedStepByStepRecognizers,
            nameof( IStepByStepRecognizer.RecognizerType ));

        void GetImplementingTypes< T >(
            IDictionary< Recognizer, T > supportedRecognizers,
            string nameOfRecognizerProperty)
        {
            Type recognizerType = typeof( T );

            // Find all types which derive from `T`.
            foreach (Type type in recognizerType.Assembly.GetTypes().Where(ty => ty != recognizerType && recognizerType.IsAssignableFrom(ty)))
            {
                T instance = (T)Activator.CreateInstance(type)!;

                // Get the mapping from `Recognizer` to `T` by invoking the property on the
                // recognizer which specifies it.
                supportedRecognizers.Add(
                    (Recognizer)type.GetRuntimeProperty(nameOfRecognizerProperty)!.GetValue(instance)!,
                    instance);
            }
        }
    }

    // The selected recognizers which should be run.
    private readonly IList< IRecognizer > ? _recognizers;

    // The recognizer to which the instruction belongs, if the runner is created for step-by-step
    // mode.
    private readonly Recognizer _stepByStepRecognizer;
    private readonly IInstruction ? _instruction;

    // The syntax graph of the code currently being recognized.
    private readonly SyntaxGraph _graph;

    /// <summary>
    /// Create a new recognizer runner instance.
    /// </summary>
    /// <param name="files">The files to run the recognizers on.</param>
    /// <param name="recognizers">The recognizers to run.</param>
    public RecognizerRunner(
        IEnumerable< string > files,
        IEnumerable< Recognizer > recognizers)
        : this(
            files,
            recognizers.Select(
                recognizer =>
                {
                    SupportedRecognizers.TryGetValue(
                        recognizer,
                        out IRecognizer ? recognizerImpl);
                    return recognizerImpl;
                }).Where(recognizer => null != recognizer).Select(recognizer => recognizer!))
    {
    }

    /// <summary>
    /// Create a new recognizer runner instance.
    /// </summary>
    /// <param name="files">The files to run the recognizers on.</param>
    /// <param name="recognizers">The design patterns for which to run the recognizers.</param>
    public RecognizerRunner(
        IEnumerable< string > files,
        IEnumerable< IRecognizer > recognizers)
    {
        _recognizers = recognizers.ToList();

        _graph = new SyntaxGraph();
        foreach (string file in files)
        {
            if (!File.Exists(file))
            {
                throw new ArgumentException($"'{file}' does not exist");
            }

            _graph.AddFile(
                File.ReadAllText(file),
                file);
        }
        _graph.CreateGraph();
    }

    /// <summary>
    /// Create a new recognizer runner instance.
    /// </summary>
    /// <param name="stepByStepRecognizer">The <see cref="Recognizer"/> to which the <paramref name="instruction"/> belongs.</param>
    /// <param name="filePaths">The path of the file to run the <paramref name="instruction"/> on.</param>
    /// <param name="instruction">The <see cref="IInstruction"/> to run.</param>
    public RecognizerRunner(
        Recognizer stepByStepRecognizer,
        IEnumerable< string > filePaths,
        IInstruction instruction)
    {
        _stepByStepRecognizer = stepByStepRecognizer;
        _instruction = instruction;
        _graph = new SyntaxGraph();
        foreach (string file in filePaths)
        {
            if (!File.Exists(file))
            {
                throw new ArgumentException($"'{file}' does not exist");
            }

            _graph.AddFile(
                File.ReadAllText(file),
                file);
        }
        _graph.CreateGraph();
    }

    public record RunResult(
        Recognizer RecognizerType,
        ICheckResult CheckResult,
        IList< string > Requirements);

    /// <summary>
    /// Gets the requirements from the given <paramref name="check"/>.
    /// </summary>
    /// <param name="check">The <see cref="ICheck"/> from which to get the requirements.</param>
    /// <returns>The requirements.</returns>
    private static IEnumerable< string > GetRequirementsFromCheck(
        ICheck check)
    {
        if (!string.IsNullOrEmpty(check.Requirement))
        {
            yield return check.Requirement;
        }

        switch (check)
        {
            case NotCheck notCheck:
                foreach (string requirement in GetRequirementsFromCheck(notCheck.NestedCheck))
                {
                    yield return requirement;
                }
                yield break;
            case NodeCheckBase nodeCheckBase:
                foreach (ICheck childCheck in nodeCheckBase.SubChecks)
                {
                    foreach (string requirement in GetRequirementsFromCheck(childCheck))
                    {
                        yield return requirement;
                    }
                }
                yield break;
        }
    }

    /// <summary>
    /// Runs the configured <see cref="IRecognizer"/>s.
    /// </summary>
    /// <param name="pruneAll">Whether to prune regardless of <see cref="Priority"/>s</param>
    /// <returns>The result of the <see cref="IRecognizer"/>, or <see langword="null"/> if the <see cref="SyntaxGraph"/> is empty.</returns>
    public IList< RunResult > Run(
        bool pruneAll = false)
    {
        // If the graph is empty, we don't have to do any work.
        if (_graph.IsEmpty)
        {
            return new List< RunResult >();
        }

        List< RunResult > results = new();
        if (_recognizers != null)
        {
            foreach (IRecognizer recognizer in _recognizers)
            {
                ICheck rootCheck = recognizer.CreateRootCheck();
                ICheckResult rootCheckResult = RunImpl(rootCheck);
                IList< string > requirements = GetRequirementsFromCheck(rootCheck).ToList();

                results.Add(
                    new RunResult(
                        recognizer.RecognizerType,
                        rootCheckResult,
                        requirements));
            }
        }
        else
        {
            if (_instruction != null)
            {
                ICheck rootCheck = new NodeCheck< INode >(
                    Priority.Knockout,
                    $"0. {_stepByStepRecognizer}",
                    _instruction.Checks);
                IList< string > requirements = GetRequirementsFromCheck(rootCheck).ToList();
                results.Add(
                    new RunResult(
                        _stepByStepRecognizer,
                        RunImpl(rootCheck),
                        requirements));
            }
            else
            {
                throw new ArgumentException("Provide either an instruction or recognizers to run");
            }
        }

        return results;

        ICheckResult RunImpl(
            ICheck rootCheck)
        {
            IRecognizerContext ctx = new RecognizerContext
                                     {
                                         Graph = _graph,
                                         CurrentEntity = null!,
                                         EntityCheck = rootCheck,
                                         PreviousContext = null!,
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
                rootResult,
                pruneAll);

            CalcScores(
                resultsByNode,
                rootResult);

            SortResultsByPriorities(rootResult);

            return rootResult;
        }
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
    /// The <see cref="ICheck"/> belonging to the <see cref="CurrentEntity"/>.
    /// </summary>
    internal ICheck EntityCheck { get; }

    /// <summary>
    /// The <see cref="IRecognizerContext"/> from which this <see cref="IRecognizerContext"/> was
    /// created, or <see langword="null"/> if this is the root <see cref="IRecognizerContext"/>.
    /// </summary>
    internal IRecognizerContext ? PreviousContext { get; }

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
                                   EntityCheck = currentNode is IEntity
                                       ? parentCheck
                                       : oldCtx.EntityCheck,
                                   PreviousContext = oldCtx,
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
    public required ICheck EntityCheck { get; init; }

    /// <inheritdoc />
    public IRecognizerContext ? PreviousContext { get; init; }
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

    /// <inheritdoc />
    bool INode.IsPlaceholder => true;
}
