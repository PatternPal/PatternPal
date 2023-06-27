namespace PatternPal.Core.Runner;

public partial class RecognizerRunner
{
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
}
