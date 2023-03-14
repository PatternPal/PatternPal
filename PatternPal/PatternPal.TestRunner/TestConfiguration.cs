namespace PatternPal.TestRunner;

/// <summary>
/// Defines the configuration format which can be used to add projects to be tested.
/// </summary>
public class TestConfiguration
{
    /// <summary>
    /// Whether to check if any result matches the implemented pattern.
    /// </summary>
    public bool CheckAllResults { get; init; }

    /// <summary>
    /// Whether to show the found results if the implemented pattern is not found.
    /// </summary>
    public bool ShowIncorrectResults { get; init; }

    /// <summary>
    /// The projects to test.
    /// </summary>
    public required IList< Project > Projects { get; init; }
}

/// <summary>
/// Defines the configuration format for a project to be tested.
/// </summary>
public class Project 
{
    /// <summary>
    /// The directory containing the project to be analyzed.
    /// </summary>
    public required string Directory { get; init; }

    /// <summary>
    /// Which pattern is implemented.
    /// </summary>
    public required string ImplementedPattern { get; init; }

    /// <summary>
    /// Whether to skip the project (e.g. because it leads to errors).
    /// </summary>
    public bool Skip { get; init; }
}
