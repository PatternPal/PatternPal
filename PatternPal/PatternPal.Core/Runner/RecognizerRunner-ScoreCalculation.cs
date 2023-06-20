namespace PatternPal.Core.Runner;

public partial class RecognizerRunner
{
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
        Score score = default;
        foreach (ICheckResult childResult in result.ChildrenCheckResults)
        {
            SortResultsByPrioritiesImpl(
                resultsByNode,
                childResult);

            if (result.CollectionKind != CheckCollectionKind.Any)
            {
                score += childResult.Score;
            }
        }

        if (result.CollectionKind != CheckCollectionKind.Any)
        {
            ((ICheckResult)result).SetScore(
                false,
                score);
        }

        ((List< ICheckResult >)result.ChildrenCheckResults).Sort(
            (
                a,
                b) => a.Score.CompareTo(b.Score));

        if (result.CollectionKind == CheckCollectionKind.Any)
        {
            ((ICheckResult)result).SetScore(
                false,
                result.ChildrenCheckResults.FirstOrDefault()?.Score ?? default);
        }
    }

    private static void SortResultsByPrioritiesImpl(
        IReadOnlyDictionary< INode, List< ICheckResult > > resultsByNode,
        ICheckResult childResult)
    {
        switch (childResult)
        {
            case LeafCheckResult leafResult:
            {
                childResult.SetScore(
                    false,
                    Score.CreateScore(
                        leafResult.Priority,
                        leafResult.Correct));

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

                    // If the type check is for the current entity, just use a score of 1 (which
                    // we already assigned above). Otherwise the score would become infinite ;).
                    if (leafResult.Check is TypeCheck {IsForCurrentEntity: true})
                    {
                        break;
                    }

                    // Ensure we don't try to use the score of the current entity, because we're
                    // still calculating it.
                    if (GetParentEntityCheck(leafResult.Check) == GetParentEntityCheck(relevantResult.Check))
                    {
                        break;
                    }

                    // Increment the score with the score of the related node. If the related
                    // node is not an entity, take the score of its entity parent. Otherwise,
                    // correctly implemented members in bad parents would weigh more than poorly
                    // implemented members in good parents.
                    childResult.SetScore(
                        false,
                        leafResult.Score
                        + leafResult.MatchedNode switch
                        {
                            IEntity => relevantResult.Score,
                            IMember member => GetParentScoreFromMember(
                                resultsByNode,
                                relevantResult,
                                member),
                            _ => throw new UnreachableException("Relation to non-IMember or IEntity")
                        });

                    static ICheck GetParentEntityCheck(
                        ICheck currentCheck)
                    {
                        while (true)
                        {
                            if (currentCheck is ClassCheck or InterfaceCheck)
                            {
                                break;
                            }

                            if (currentCheck.ParentCheck == null)
                            {
                                // Should only be reached in tests.
                                break;
                            }
                            currentCheck = currentCheck.ParentCheck;
                        }
                        return currentCheck;
                    }

                    // Gets the score of the `member`s parent of type Entity.
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
                childResult.SetScore(
                    false,
                    Score.GetNot(
                        notResult.NestedResult.Priority,
                        notResult.NestedResult.Score));
                break;
            default:
                throw new ArgumentException($"{childResult} is not a supported ICheckResult");
        }
    }

    private static Score GetParentScoreFromMember(
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
    }

    private static bool HasDescendantResult(
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
