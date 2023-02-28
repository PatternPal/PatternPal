#region

using PatternPal.Core;
using PatternPal.Core.Models;

#endregion

// Collect projects to test
using StreamReader reader = new(
    new FileStream(
        args[ 0 ],
        FileMode.Open) );
IDictionary< string, string > testProjects = new Dictionary< string, string >();
while (reader.ReadLine() is { } line)
{
    if (line[ 0 ] == '#')
    {
        continue;
    }

    string[ ] lineParts = line.Split(',');
    testProjects.Add(
        lineParts[ 0 ],
        lineParts[ 1 ]);
}

bool checkAllResultsForCorrectness = args.Length > 1 && args[ 1 ] == "all";

FileManager fileManager = new();
RecognizerRunner runner = new();
IList< ProjectResult > results = new List< ProjectResult >();

// Run PatternPal for all the test projects
foreach ((string projectDir, string implementedPattern) in testProjects)
{
    ProjectResult result = new()
                           {
                               ProjectName = projectDir, ImplementedPattern = implementedPattern, Results = new List< DetectionResult >(),
                           };

    runner.CreateGraph(fileManager.GetAllCSharpFilesFromDirectory(projectDir));

    foreach (RecognitionResult recognitionResult in runner.Run(RecognizerRunner.DesignPatterns.ToList()).Where(x => x.Result.GetScore() >= 50).OrderByDescending(x => x.Result.GetScore()).ToList())
    {
        result.Results.Add(
            new DetectionResult
            {
                ClassName = recognitionResult.EntityNode.GetName(), DetectedPattern = recognitionResult.Pattern.Name, Score = recognitionResult.Result.GetScore(),
            });
    }

    results.Add(result);
}

// Calculate correctly detected patterns
int correctlyDetectedPatterns = 0;
int projectsWithoutResults = 0;
foreach (ProjectResult result in results)
{
    if (result.Results.Count == 0)
    {
        projectsWithoutResults++;
        continue;
    }

    if (result.Correct(checkAllResultsForCorrectness))
    {
        correctlyDetectedPatterns++;
    }
}

int totalProjectCount = testProjects.Count;
int projectsWithResults = totalProjectCount - projectsWithoutResults;
double recall = projectsWithResults / (double)totalProjectCount * 100;
double precision = correctlyDetectedPatterns / (double)projectsWithResults * 100;

// Prints stats
Console.WriteLine($"Implementations with results: {projectsWithResults} of {testProjects.Count}");
Console.WriteLine($"Patterns correctly detected: {correctlyDetectedPatterns}");
Console.WriteLine($"Patterns incorrectly detected: {results.Count - correctlyDetectedPatterns}");
Console.WriteLine();
Console.WriteLine($"Recall:    {recall:F1}%");
Console.WriteLine($"Precision: {precision:F1}%");
Console.WriteLine();

// Print incorrect results, with detected pattern with highest score
foreach (ProjectResult result in results)
{
    if (result.Correct(checkAllResultsForCorrectness))
    {
        continue;
    }

    if (result.Results.Count == 0)
    {
        Console.WriteLine($"{result.ProjectName}: No pattern detected");
        continue;
    }

    Console.WriteLine($"{result.ProjectName}: Expected '{result.ImplementedPattern}', found:");
    foreach (DetectionResult res in result.Results)
    {
        Console.WriteLine($"  - '{res.DetectedPattern}' with score {res.Score} (implemented in '{res.ClassName}')");
    }
}

class ProjectResult
{
    internal required string ProjectName { get; init; }
    internal required string ImplementedPattern { get; init; }

    // KNOWN: Results are sorted by score in descending order
    internal required IList< DetectionResult > Results { get; init; }

    private bool ? m_Correct;

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

class DetectionResult
{
    internal required string ClassName { get; init; }
    internal required string DetectedPattern { get; init; }
    internal required int Score { get; init; }
}
