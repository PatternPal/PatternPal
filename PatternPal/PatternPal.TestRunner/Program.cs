namespace PatternPal.TestRunner;

internal class Program
{
    private const string ConfigArg = "Config";
    private const string HelpMessage = "Please supply the path to the configuration file.\nUsage:\n\tTestRunner --config=/path/to/file.json";

    private readonly FileManager _fileManager;
    private readonly TestConfiguration _configuration;

    public static void Main(
        string[ ] args)
    {
        // Parse config
        TestConfiguration configuration;
        try
        {
            configuration = ParseConfig(args);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return;
        }

        new Program(configuration).Run();
    }

    private Program(
        TestConfiguration configuration)
    {
        _fileManager = new FileManager();
        _configuration = configuration;
    }

    private void Run()
    {
        // Run PatternPal for all the test projects; filter nulls
        IList< ProjectResult > results = _configuration.Projects.Select(TestProject).Where(result => result != null).Select(project => project!).ToList();
        int totalProjectsChecked = results.Count;

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

            if (result.Correct(_configuration.CheckAllResults))
            {
                correctlyDetectedPatterns++;
            }
        }

        // Calculate, print statistics
        {
            int projectsWithResults = totalProjectsChecked - projectsWithoutResults;
            double recall = projectsWithResults / (double)totalProjectsChecked * 100;
            double precision = correctlyDetectedPatterns / (double)projectsWithResults * 100;

            Console.WriteLine($"Implementations with results: {projectsWithResults} of {totalProjectsChecked}");
            Console.WriteLine($"Patterns correctly detected: {correctlyDetectedPatterns}");
            Console.WriteLine($"Patterns incorrectly detected: {results.Count - correctlyDetectedPatterns}\n");
            Console.WriteLine($"Recall:    {recall:F1}%");
            Console.WriteLine($"Precision: {precision:F1}%\n");
        }

        // If option specified in configuration: print incorrect results.
        if (!_configuration.ShowIncorrectResults)
        {
            return;
        }

        foreach (ProjectResult result in results)
        {
            if (!result.Correct(_configuration.CheckAllResults))
            {
                PrintIncorrectResult(result);
            }
        }
    }

    /// <summary>
    /// Parses the .json-config to a TestConfiguration using the path supplied in the CL-args.
    /// </summary>
    /// <param name="args">Array of command line args</param>
    /// <returns>A TestConfiguration created from the supplied .json-config.</returns>
    /// <exception cref="ArgumentException">Thrown when config filepath was not supplied in the CL-args.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the config file was not found at the specified path.</exception>
    /// <exception cref="FormatException">Thrown when the config file could not be parsed.</exception>
    private static TestConfiguration ParseConfig(
        string[ ] args)
    {
        // Parse command line arguments
        IConfigurationBuilder commandLineArgsConfigBuilder = new ConfigurationManager();
        commandLineArgsConfigBuilder.AddCommandLine(args);
        IConfigurationRoot commandLineArgs = commandLineArgsConfigBuilder.Build();

        string ? configFilePath = commandLineArgs[ ConfigArg ];

        if (string.IsNullOrWhiteSpace(configFilePath))
        {
            // Arg was not supplied, thus we print the help message.
            throw new ArgumentException(HelpMessage);
        }

        FileInfo configFile = new( configFilePath );
        if (!configFile.Exists)
        {
            throw new FileNotFoundException(
                $"Unable to open config file '{configFile.FullName}' because it does not exist");
        }

        // Parse config file
        IConfigurationBuilder configurationBuilder = new ConfigurationManager();
        try
        {
            configurationBuilder.AddJsonFile(configFile.FullName);
        }
        catch (Exception)
        {
            throw new FormatException("Failed to parse configuration file");
        }

        TestConfiguration ? configuration = configurationBuilder.Build().Get< TestConfiguration >();
        if (configuration is null)
        {
            throw new FormatException("Failed to parse configuration file");
        }

        return configuration;
    }

    /// <summary>
    /// Test the specified project.
    /// </summary>
    /// <param name="project">The project to test</param>
    /// <returns>The result if the project was not skipped, else null.</returns>
    private ProjectResult ? TestProject(
        Project project)
    {
        if (project.Skip)
        {
            return null;
        }

        ProjectResult result = new()
                               {
                                   Directory = project.Directory,
                                   ImplementedPattern = project.ImplementedPattern,
                                   Results = new List< DetectionResult >(),
                               };

        RecognizerRunner runner = new(
            _fileManager.GetAllCSharpFilesFromDirectory(project.Directory),
            RecognizerRunner.SupportedRecognizers.Values.ToList() );

        foreach ((_, ICheckResult checkResult) in runner.Run())
        {
            result.Results.Add(
                new DetectionResult
                {
                    ClassName = checkResult.MatchedNode?.GetName() ?? string.Empty,
                    DetectedPattern = string.Empty,
                    Score = 0
                });
        }

        return result;
    }

    /// <summary>
    /// Prints details on an incorrectly detected pattern, including the detected pattern with the highest score.
    /// </summary>
    /// <param name="result">The result to be printed.</param>
    private static void PrintIncorrectResult(
        ProjectResult result)
    {
        if (result.Results.Count == 0)
        {
            Console.WriteLine($"{result.Directory}: No pattern detected");
            return;
        }

        Console.WriteLine($"{result.Directory}: Expected '{result.ImplementedPattern}', found:");
        foreach (DetectionResult res in result.Results)
        {
            Console.WriteLine($"  - '{res.DetectedPattern}' with Score {res.Score} (implemented in '{res.ClassName}')");
        }
    }
}
