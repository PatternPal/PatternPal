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

    /// <summary>
    /// Sort results recursively.
    /// </summary>
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

    /// <summary>
    /// Calculates the scores of each <see cref="ICheckResult"/>.
    /// </summary>
    /// <param name="resultsByNode">Maps <see cref="INode"/>s to the <see cref="ICheckResult"/> which matched them.</param>
    /// <param name="result">The root <see cref="ICheckResult"/>.</param>
    internal static void CalcScores(
        IReadOnlyDictionary< INode, List< ICheckResult > > resultsByNode,
        NodeCheckResult result)
    {
        Score totalChildScore = default;
        Score totalPerfectChildScore = default;
        foreach (ICheckResult childResult in result.ChildrenCheckResults)
        {
            CalcScoreImpl(
                resultsByNode,
                childResult);

            totalChildScore += childResult.Score;
            totalPerfectChildScore += childResult.PerfectScore;
        }

        CalcPerfectScore(
            result,
            ref totalPerfectChildScore);

        switch (result.CollectionKind)
        {
            case CheckCollectionKind.Any:
                ICheckResult ? childResult = result.ChildrenCheckResults.MaxBy(r => r.Score.PercentageOf(r.PerfectScore));
                result.SetScore(
                    false,
                    childResult?.Score ?? default);
                result.SetScore(
                    true,
                    childResult?.PerfectScore ?? totalPerfectChildScore);
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

    /// <summary>
    /// Calculates the scores of each <see cref="ICheckResult"/>.
    /// </summary>
    private static void CalcScoreImpl(
        IReadOnlyDictionary< INode, List< ICheckResult > > resultsByNode,
        ICheckResult childResult)
    {
        switch (childResult)
        {
            case LeafCheckResult leafResult:
            {
                leafResult.SetScore(
                    false,
                    Score.CreateScore(
                        leafResult.Priority,
                        leafResult.Correct));
                leafResult.SetScore(
                    true,
                    Score.CreateScore(
                        leafResult.Priority,
                        true));
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

    /// <summary>
    /// Calculates the perfect score of <paramref name="result"/>, and falls back to the
    /// <see cref="ICheck.PerfectScore"/> of the <see cref="ICheckResult.Check"/> if its results
    /// have been pruned.
    /// </summary>
    /// <param name="result">The <see cref="ICheckResult"/> for which to calculate the perfect <see cref="Score"/>.</param>
    /// <param name="totalPerfectChildScore">The perfect <see cref="Score"/> of the child checks of <paramref name="result"/>.</param>
    private static void CalcPerfectScore(
        NodeCheckResult result,
        ref Score totalPerfectChildScore)
    {
        // Calculate perfect score for pruned results.
        if (result is {NodeCheckCollectionWrapper: true, ChildrenCheckResults.Count: 0})
        {
            // Results were pruned.
            totalPerfectChildScore = result.Check.PerfectScore;
            return;
        }

        IEnumerable< ICheck > checksToEvaluate = result.Check is NodeCheckBase nodeCheckBase
            ? nodeCheckBase.SubChecks
            : new[ ]
              {
                  result.Check
              };

        NodeCheckResult bestResult = result.NodeCheckCollectionWrapper
            ? result.ChildrenCheckResults.MaxBy(r => r.Score.PercentageOf(r.PerfectScore))! as NodeCheckResult ?? result
            : result;

        switch (bestResult.CollectionKind)
        {
            case CheckCollectionKind.Any:
            {
                Score bestScore = default;
                foreach (ICheck subCheck in checksToEvaluate)
                {
                    bestScore = subCheck.PerfectScore.CompareTo(bestScore) > 0
                        ? bestScore
                        : subCheck.PerfectScore;
                }
                totalPerfectChildScore = bestScore;
                break;
            }
            case CheckCollectionKind.All:
            {
                foreach (ICheck subCheck in checksToEvaluate)
                {
                    if (bestResult.ChildrenCheckResults.Count != 0
                        && bestResult.ChildrenCheckResults.All(r => r.Check != subCheck))
                    {
                        // Result was pruned.
                        totalPerfectChildScore += subCheck.PerfectScore;
                    }
                }
                break;
            }
            default:
                throw new ArgumentException($"Unknown CheckCollectionKind '{bestResult.CollectionKind}'");
        }
    }
}
