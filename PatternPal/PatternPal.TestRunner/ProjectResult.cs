namespace PatternPal.TestRunner;

/// <summary>
/// Analysis result for a project.
/// </summary>
class ProjectResult
{
    /// <summary>
    /// The directory where the project is located.
    /// </summary>
    internal required string Directory { get; init; }

    /// <summary>
    /// The pattern which is supposed to be implemented in the project.
    /// </summary>
    internal required string ImplementedPattern { get; init; }

    /// <summary>
    /// The analysis results for this project.
    /// </summary>
    /// <remarks>Results are sorted by score in descending order.</remarks>
    internal required IList< DetectionResult > Results { get; init; }

    private bool ? m_Correct;

    /// <summary>
    /// Checks whether the implemented pattern is recognized.
    /// </summary>
    /// <param name="checkAllResultsForCorrectness">Whether to check if the implemented pattern is recognized with any score, not just the top score.</param>
    /// <returns>Returns <see langword="true"/> if the implemented pattern is recognized.</returns>
    internal bool Correct(
        bool checkAllResultsForCorrectness)
    {
        if (m_Correct.HasValue)
        {
            return m_Correct.Value;
        }

        if (Results.Count == 0)
        {
            m_Correct = false;
            return m_Correct.Value;
        }

        // Remove any spaces which may be present in the pattern name (e.g. 'Factory Method' ->
        // 'FactoryMethod').
        string implementedPatternNormalized = ImplementedPattern.Replace(
            " ",
            string.Empty);

        foreach (DetectionResult result in Results)
        {
            if (result.DetectedPattern.Replace(
                    " ",
                    string.Empty)
                == implementedPatternNormalized)
            {
                m_Correct = true;
                return m_Correct.Value;
            }

            if (!checkAllResultsForCorrectness)
            {
                break;
            }
        }

        m_Correct = false;
        return m_Correct.Value;
    }
}

/// <summary>
/// Analysis result for a class.
/// </summary>
class DetectionResult
{
    /// <summary>
    /// The class name in which a pattern was detected.
    /// </summary>
    internal required string ClassName { get; init; }

    /// <summary>
    /// The pattern which was detected.
    /// </summary>
    internal required string DetectedPattern { get; init; }

    /// <summary>
    /// The score for the detected pattern.
    /// </summary>
    internal required int Score { get; init; }
}
