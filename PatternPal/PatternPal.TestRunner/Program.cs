const string CONFIG_ARG = "Config";

// Parse command line arguments.
IConfigurationBuilder commandLineArgsConfigBuilder = new ConfigurationManager();
commandLineArgsConfigBuilder.AddCommandLine(args);
IConfigurationRoot commandLineArgs = commandLineArgsConfigBuilder.Build();

string ? configFilePath = commandLineArgs[ CONFIG_ARG ];
if (string.IsNullOrWhiteSpace(configFilePath))
{
    Console.Error.WriteLine("Missing config file argument");
    return;
}

FileInfo configFile = new( configFilePath );
if (!configFile.Exists)
{
    Console.Error.WriteLine($"Unable to open config file '{configFile.FullName}' because it does not exist");
    return;
}

// Parse config file.
IConfigurationBuilder configurationBuilder = new ConfigurationManager();
try
{
    configurationBuilder.AddJsonFile(configFile.FullName);
}
catch (Exception)
{
    Console.Error.WriteLine("Failed to parse configuration file");
    return;
}

TestConfiguration ? configuration = configurationBuilder.Build().Get< TestConfiguration >();
if (configuration is null)
{
    Console.Error.WriteLine("Failed to parse configuration file");
    return;
}

FileManager fileManager = new();
RecognizerRunner runner = new();
IList< ProjectResult > results = new List< ProjectResult >();

// Run PatternPal for all the test projects
int totalProjectsChecked = 0;
foreach (Project project in configuration.Projects)
{
    if (project.Skip)
    {
        continue;
    }

    totalProjectsChecked++;

    ProjectResult result = new()
                           {
                               Directory = project.Directory, ImplementedPattern = project.ImplementedPattern, Results = new List< DetectionResult >(),
                           };

    runner.CreateGraph(fileManager.GetAllCSharpFilesFromDirectory(project.Directory));

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

    if (result.Correct(configuration.CheckAllResults))
    {
        correctlyDetectedPatterns++;
    }
}

// Calculate statistics.
int projectsWithResults = totalProjectsChecked - projectsWithoutResults;
double recall = projectsWithResults / (double)totalProjectsChecked * 100;
double precision = correctlyDetectedPatterns / (double)projectsWithResults * 100;

// Prints statistics.
Console.WriteLine($"Implementations with results: {projectsWithResults} of {totalProjectsChecked}");
Console.WriteLine($"Patterns correctly detected: {correctlyDetectedPatterns}");
Console.WriteLine($"Patterns incorrectly detected: {results.Count - correctlyDetectedPatterns}");
Console.WriteLine();
Console.WriteLine($"Recall:    {recall:F1}%");
Console.WriteLine($"Precision: {precision:F1}%");
Console.WriteLine();

// Print incorrect results, with detected pattern with highest score
if (!configuration.ShowIncorrectResults)
{
    return;
}

foreach (ProjectResult result in results)
{
    if (result.Correct(configuration.CheckAllResults))
    {
        continue;
    }

    if (result.Results.Count == 0)
    {
        Console.WriteLine($"{result.Directory}: No pattern detected");
        continue;
    }

    Console.WriteLine($"{result.Directory}: Expected '{result.ImplementedPattern}', found:");
    foreach (DetectionResult res in result.Results)
    {
        Console.WriteLine($"  - '{res.DetectedPattern}' with score {res.Score} (implemented in '{res.ClassName}')");
    }
}
