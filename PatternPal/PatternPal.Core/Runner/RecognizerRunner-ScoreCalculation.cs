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
        NodeCheckResult result)
    {
        foreach (ICheckResult childResult in result.ChildrenCheckResults)
        {
            SortResultsByPrioritiesImpl(childResult);
        }

        ((List< ICheckResult >)result.ChildrenCheckResults).Sort(
            (
                a,
                b) => a.Score.CompareTo(b.Score));
    }

    private static void SortResultsByPrioritiesImpl(
        ICheckResult childResult)
    {
        while (true)
        {
            switch (childResult)
            {
                case LeafCheckResult:
                    return;
                case NodeCheckResult nodeCheckResult:
                    SortResultsByPriorities(nodeCheckResult);
                    return;
                case NotCheckResult notCheckResult:
                    childResult = notCheckResult.NestedResult;
                    continue;
                default:
                    throw new ArgumentException($"Unknown CheckResult type '{childResult.GetType()}'");
            }
        }
    }

    internal static void CalcScores(
        IReadOnlyDictionary< INode, List< ICheckResult > > resultsByNode,
        NodeCheckResult result)
    {
        Score totalChildScore = default;
        Score totalPerfectChildScore = default;
        foreach (ICheckResult childResult in result.ChildrenCheckResults)
        {
            if (childResult.FeedbackMessage == "Node SingleTonTestCase5User() has a Uses relation with node GetInstance()")
            {
            }
            CalcScoreImpl(
                resultsByNode,
                childResult);

            totalChildScore += childResult.Score;
            totalPerfectChildScore += childResult.PerfectScore;
        }

        // TODO: Calc perfect score for missing results
        // Be careful with NodeCheckCollectionWrappers

        switch (result.CollectionKind)
        {
            case CheckCollectionKind.Any:
                result.SetScore(
                    false,
                    result.ChildrenCheckResults.MaxBy(r => r.Score)?.Score ?? default);
                result.SetScore(
                    true,
                    result.ChildrenCheckResults.MaxBy(r => r.PerfectScore)?.PerfectScore ?? default);
                break;
            case CheckCollectionKind.All:
                result.SetScore(
                    false,
                    totalChildScore);
                result.SetScore(
                    true,
                    totalPerfectChildScore);
                break;
            default:
                throw new ArgumentException($"Unknown CheckCollectionKind '{result.CollectionKind}'");
        }
    }

    private static void CalcScoreImpl(
        IReadOnlyDictionary< INode, List< ICheckResult > > resultsByNode,
        ICheckResult childResult)
    {
        switch (childResult)
        {
            case LeafCheckResult leafResult:
            {
                CalcLeafResultScore(
                    resultsByNode,
                    leafResult,
                    false);
                CalcLeafResultScore(
                    resultsByNode,
                    leafResult,
                    true);
                break;
            }
            case NodeCheckResult nodeResult:
                CalcScores(
                    resultsByNode,
                    nodeResult);
                break;
            case NotCheckResult notResult:
                CalcScoreImpl(
                    resultsByNode,
                    notResult.NestedResult);
                notResult.SetScore(
                    false,
                    Score.GetNot(
                        notResult.NestedResult.Priority,
                        notResult.NestedResult.Score));
                notResult.SetScore(
                    true,
                    Score.CreateScore(
                        notResult.NestedResult.Priority,
                        true));
                break;
            default:
                throw new ArgumentException($"{childResult} is not a supported ICheckResult");
        }
    }

    private static void CalcLeafResultScore(
        IReadOnlyDictionary< INode, List< ICheckResult > > resultsByNode,
        LeafCheckResult leafResult,
        bool perfect)
    {
        leafResult.SetScore(
            perfect,
            Score.CreateScore(
                leafResult.Priority,
                leafResult.Correct || perfect));

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
                return;
            }

            // If the type check is for the current entity, just use a score of 1 (which
            // we already assigned above). Otherwise the score would become infinite ;).
            if (leafResult.Check is TypeCheck {IsForCurrentEntity: true})
            {
                return;
            }

            // Ensure we don't try to use the score of the current entity, because we're
            // still calculating it.
            if (GetParentEntityCheck(leafResult.Check) == GetParentEntityCheck(relevantResult.Check))
            {
                return;
            }

            // Increment the score with the score of the related node. If the related
            // node is not an entity, take the score of its entity parent. Otherwise,
            // correctly implemented members in bad parents would weigh more than poorly
            // implemented members in good parents.
            leafResult.SetScore(
                perfect,
                (perfect
                    ? leafResult.PerfectScore
                    : leafResult.Score)
                + leafResult.MatchedNode switch
                {
                    IEntity => perfect
                        ? relevantResult.PerfectScore
                        : relevantResult.Score,
                    IMember member => GetParentScoreFromMember(
                        resultsByNode,
                        relevantResult,
                        member,
                        perfect),
                    _ => throw new UnreachableException("Relation to non-IMember or IEntity")
                });
        }
    }

    private static ICheck GetParentEntityCheck(
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

    /// <summary>
    /// Gets the score of the `member`s parent of type Entity.
    /// </summary>
    private static Score GetParentScoreFromMember(
        IReadOnlyDictionary< INode, List< ICheckResult > > resultsByNode,
        ICheckResult relevantResult,
        IMember member,
        bool perfect)
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
                return perfect
                    ? parentResult.PerfectScore
                    : parentResult.Score;
            }
        }

        return default;
    }

    /// <summary>
    /// Checks if `relevantResult` is a descendant of `parentResult`.
    /// </summary>
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
