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
public class RecognizerRunner
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
    /// <param name="filePath">The path of the file to run the <paramref name="instruction"/> on.</param>
    /// <param name="instruction">The <see cref="IInstruction"/> to run.</param>
    public RecognizerRunner(
        IEnumerable< string > filePaths,
        IInstruction instruction)
    {
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
        Recognizer ? RecognizerType,
        ICheckResult CheckResult,
        IList< string > ? Requirements);

    private static IEnumerable< string > GetRequirementsFromCheckResult(
        ICheckResult checkResult)
    {
        if (!string.IsNullOrEmpty(checkResult.Check.Requirement))
        {
            yield return checkResult.Check.Requirement;
        }

        switch (checkResult)
        {
            case LeafCheckResult:
                yield break;
            case NotCheckResult notCheckResult:
                foreach (string requirement in GetRequirementsFromCheckResult(notCheckResult.NestedResult))
                {
                    yield return requirement;
                }
                yield break;
            case NodeCheckResult nodeCheckResult:
                foreach (ICheckResult childCheckResult in nodeCheckResult.ChildrenCheckResults)
                {
                    foreach (string requirement in GetRequirementsFromCheckResult(childCheckResult))
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
                // TODO: Sort. Prepend e.g. '1.1', '2', '3.2' to requirement, and sort based on this.
                IList< string > requirements = GetRequirementsFromCheckResult(rootCheckResult).ToList();

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
                    null,
                    _instruction.Checks);
                results.Add(
                    new RunResult(
                        null,
                        RunImpl(rootCheck),
                        null));
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
                                         ParentCheck = rootCheck,
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

            SortResultsByPriorities(
                resultsByNode,
                rootResult);

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

    /// <summary>
    /// Filters the sub-<see cref="ICheckResult"/>s of <paramref name="parentCheckResult" /> by
    /// removing any results which can be safely pruned.
    /// </summary>
    /// <param name="resultsByNode">A <see cref="Dictionary{TKey,TValue}"/> to get the <see cref="ICheckResult"/>s belonging to the <see cref="INode"/>
    /// found by an <see cref="ICheck"/> related to another <see cref="ICheck"/> by either a <see cref="RelationCheck"/> or <see cref="TypeCheck"/></param>
    /// <param name="parentCheckResult">The <see cref="ICheckResult"/> currently being evaluated</param>
    /// <param name="pruneAll">Whether to prune regardless of <see cref="Priority"/>s</param>
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
        NodeCheckResult parentCheckResult,
        bool pruneAll = false)
    {
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
                    if (Prune(
                        leafCheckResult.Priority,
                        pruneAll))
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
                            nodeCheckResult,
                            pruneAll)
                        && Prune(
                            nodeCheckResult.Priority,
                            pruneAll))
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
                            if (Prune(
                                notCheckResult.Priority,
                                pruneAll))
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
                                    nodeCheckResult,
                                    pruneAll)
                                && Prune(
                                    notCheckResult.Priority,
                                    pruneAll))
                            {
                                resultsToBePruned.Add(notCheckResult);
                                notCheckResult.Pruned = true;
                            }
                            break;
                        }
                        case NotCheckResult:
                            throw new ArgumentException("Nested not checks not supported");
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
        if (resultsToBePruned.Count > 0
            && (resultsToBePruned.Count == parentCheckResult.ChildrenCheckResults.Count
                || parentCheckResult.CollectionKind == CheckCollectionKind.All))
        {
            // Parent becomes empty.
            parentCheckResult.ChildrenCheckResults.Clear();
            return true;
        }

        if (parentCheckResult is
            {
                NodeCheckCollectionWrapper: true,
                Check: RelationCheck or TypeCheck
            })
        {
            // We're currently processing the NodeCheckResult of a RelationCheck or TypeCheck. 

            // Empty relation/type check results can also be pruned.
            if (parentCheckResult.ChildrenCheckResults.Count == 0)
            {
                return true;
            }

            foreach (ICheckResult dependentCheckResult in parentCheckResult.ChildrenCheckResults)
            {
                // If `dependentCheckResult` has already been pruned earlier on, we don't need to
                // check it here again.
                if (dependentCheckResult.Pruned)
                {
                    continue;
                }

                if (dependentCheckResult is not LeafCheckResult dependentResult)
                {
                    throw new ArgumentException($"Unexpected check type '{dependentCheckResult.GetType()}', expected '{typeof( LeafCheckResult )}'");
                }

                // Always prune results of incorrect relations.
                if (!dependentResult.Correct)
                {
                    resultsToBePruned.Add(dependentResult);
                    dependentResult.Pruned = true;
                    continue;
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

        if (resultsToBePruned.Count > 0
            && (resultsToBePruned.Count == parentCheckResult.ChildrenCheckResults.Count
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
    /// Whether the <see cref="ICheckResult"/> should be pruned in case it is incorrect.
    /// </summary>
    /// <param name="priority">The <see cref="Priority"/> of the <see cref="ICheckResult"/></param>
    /// <param name="pruneAll">Whether to prune regardless of <see cref="Priority"/>s</param>
    private static bool Prune(
        Priority priority,
        bool pruneAll)
        => pruneAll || priority == Priority.Knockout;

    /// <summary>
    /// Sorts a <see cref="NodeCheckResult"/> based on <see cref="Priority"/>s and whether the
    /// <see cref="LeafCheckResult"/>s are correct. It goes recursively through the tree of
    /// <see cref="ICheckResult"/>s, and gives a <see cref="Score"/> to each <see cref="ICheckResult"/>
    /// based their <see cref="Priority"/>s and the <see cref="Score"/>s and <see cref="Priority"/>s
    /// of their children.
    /// </summary>
    internal static void SortResultsByPriorities(
        IReadOnlyDictionary< INode, List< ICheckResult > > resultsByNode,
        NodeCheckResult result)
    {
        foreach (ICheckResult childResult in result.ChildrenCheckResults)
        {
            SortResultsByPrioritiesImpl(
                resultsByNode,
                childResult);
            result.Score += childResult.Score;
        }

        ((List< ICheckResult >)result.ChildrenCheckResults).Sort(
            (
                a,
                b) => a.Score.CompareTo(b.Score));

        static void SortResultsByPrioritiesImpl(
            IReadOnlyDictionary< INode, List< ICheckResult > > resultsByNode,
            ICheckResult childResult)
        {
            switch (childResult)
            {
                case LeafCheckResult leafResult:
                {
                    leafResult.Score = Score.CreateScore(
                        leafResult.Priority,
                        leafResult.Correct);

                    // Take into account the score of the related node if this result belongs to a
                    // Relation- or TypeCheck.
                    if (leafResult is {Correct: true, Check: RelationCheck or TypeCheck, MatchedNode: not null})
                    {
                        // Get the CheckResults belonging to the related node.
                        List< ICheckResult > resultsOfNode = resultsByNode[ leafResult.MatchedNode ];

                        // Find the CheckResult linked to the check which searched for the related node.
                        ICheckResult ? relevantResult = resultsOfNode.FirstOrDefault(
                            result => result.Check == leafResult.RelatedCheck);

                        if (relevantResult == null)
                        {
                            // Probably unreachable.
                            break;
                        }

                        // If the type check is for the current entity, just increment the score
                        // with 1. Otherwise the score would become infinite ;).
                        if (leafResult.Check is TypeCheck {IsForCurrentEntity: true})
                        {
                            leafResult.Score += Score.CreateScore(
                                leafResult.Priority,
                                true);
                            break;
                        }

                        // Increment the score with the score of the related node. If the related
                        // node is not an entity, take the score of its entity parent. Otherwise,
                        // correctly implemented members in bad parents would weigh more than poorly
                        // implemented members in good parents.
                        leafResult.Score += leafResult.MatchedNode switch
                        {
                            IEntity => relevantResult.Score,
                            IMember member => GetParentScoreFromMember(
                                resultsByNode,
                                relevantResult,
                                member),
                            _ => throw new UnreachableException("Relation to non-IMember or IEntity")
                        };

                        // Gets the score of the `member`s parent of type Entity.
                        static Score GetParentScoreFromMember(
                            IReadOnlyDictionary< INode, List< ICheckResult > > resultsByNode,
                            ICheckResult relevantResult,
                            IMember member)
                        {
                            IEntity parent = member.GetParent();

                            foreach (ICheckResult parentResult in resultsByNode[ parent ])
                            {
                                if (parentResult is LeafCheckResult)
                                {
                                    continue;
                                }

                                if (HasDescendantResult(
                                    parentResult,
                                    relevantResult))
                                {
                                    return parentResult.Score;
                                }
                            }

                            return default;

                            // Checks if `relevantResult` is a descendant of `parentResult`.
                            static bool HasDescendantResult(
                                ICheckResult parentResult,
                                ICheckResult relevantResult)
                            {
                                if (parentResult == relevantResult)
                                {
                                    return true;
                                }

                                switch (parentResult)
                                {
                                    case LeafCheckResult:
                                        return false;
                                    case NotCheckResult notCheckResult:
                                        return HasDescendantResult(
                                            notCheckResult.NestedResult,
                                            relevantResult);
                                    case NodeCheckResult nodeCheckResult:
                                        return nodeCheckResult.ChildrenCheckResults.Any(
                                            childResult => HasDescendantResult(
                                                childResult,
                                                relevantResult));
                                    default:
                                        throw new ArgumentException($"Unsupported CheckResult type: {parentResult.GetType()}");
                                }
                            }
                        }
                    }
                    break;
                }
                case NodeCheckResult nodeResult:
                    SortResultsByPriorities(
                        resultsByNode,
                        nodeResult);
                    break;
                case NotCheckResult notResult:
                    SortResultsByPrioritiesImpl(
                        resultsByNode,
                        notResult.NestedResult);
                    notResult.Score = Score.GetNot(
                        notResult.NestedResult.Priority,
                        notResult.NestedResult.Score);
                    break;
                default:
                    throw new ArgumentException($"{childResult} is not a supported ICheckResult");
            }
        }
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
                                   ParentCheck = parentCheck,
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
    public required ICheck ParentCheck { get; init; }

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
