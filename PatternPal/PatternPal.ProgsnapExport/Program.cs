#region

using System.ComponentModel;
using System.Globalization;
using System.Text.Json;

using PatternPal.ProgSnapExport.Data;
using PatternPal.LoggingServer.Models;

using Spectre.Console.Cli;
using Spectre.Console;

using CsvHelper;
using CsvHelper.Configuration;
using PatternPal.LoggingServer;

#endregion

namespace PatternPal.ProgSnapExport;

internal sealed class ProgSnapExportCommand : Command<ProgSnapExportCommand.Settings>
{
    private ProgSnap2ContextClass _dbContext;
    private Settings _settings;

    private string _logFile;
    private string _csvFile;
    private string _codeStateExportDir;

    /// <summary>
    /// Defines all CLI-arguments.
    /// </summary>
    public sealed class Settings : CommandSettings
    {
        // CLI Settings
        [Description("Path to config file; defaults to \"./config.json\".")]
        [CommandArgument(0, "[configFile]")]
        [DefaultValue("./config.json")]
        public string ConfigFile { get; init; }

        [Description("Path to export directory; defaults to \"./export\".")]
        [CommandArgument(1, "[exportDirectory]")]
        [DefaultValue("./export")]
        public string ExportDirectory { get; init;  }

        [Description("Specifies the starting point of the time interval to be exported. Format: \"YYYY-MM-DD HH:MM:SS\".")]
        [CommandOption("-s|--start")]
        public string? StartIntervalStr { get; init; }

        [Description("Specifies the ending point of the time interval to be exported. Format: \"YYYY-MM-DD HH:MM:SS\".")]
        [CommandOption("-e|--end")]
        public string? EndIntervalStr { get; init; }
        
        [Description("Overwrites the contents of the export directory (if any).")]
        [CommandOption("-f|--force")]
        public bool ForceExportDirectory { get; init; }
        
        [Description("Does not include the CodeStates in the export.")]
        [CommandOption("--csv-only")]
        [DefaultValue(false)]
        public bool CsvOnly { get; init; }
        
        [Description("Sets the log level of what is logged to the logfile. Format: INFO|WARNING|ERROR (default: \"WARNING\").")]
        [CommandOption("--log-level")]
        [DefaultValue(LogLevel.Warning)]
        public LogLevel LogLevel { get; init; }
        
        [Description("Prints all messages to the console.")]
        [CommandOption("-v|--verbose")]
        [DefaultValue(false)]
        public bool Verbose { get; init; }
        
        [Description("Surpresses all messages on the console.")]
        [CommandOption("-q|--quiet")]
        [DefaultValue(false)]
        public bool Quiet { get; init; }

        // Properties
        public string ConnectionString { get; private set; }
        public string? CodeStateDirectory { get; private set; }
        public DateTimeOffset? StartInterval { get; private set; } = null;
        public DateTimeOffset? EndInterval { get; private set; } = null;

        /// <summary>
        /// Extends the build-in validation of the supplied CLI-arguments.
        /// </summary>
        /// <returns></returns>
        public override ValidationResult Validate()
        {
            ValidationResult baseValidationResult = base.Validate();

            if (!baseValidationResult.Successful)
            {
                return baseValidationResult;
            }

            // Config file
            if (!File.Exists(ConfigFile))
            { 
                return ValidationResult.Error($"Invalid path to config file specified: \"{ConfigFile}\".");
            }

            try
            {
                Dictionary<string, string> config =
                    JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(ConfigFile));
                ConnectionString =
                    $"Server={config["ServerUrl"]};Username={config["Username"]};Database={config["Database"]};Password={config["Password"]}";

                if (!CsvOnly)
                {
                    // We do not check for the presence of CodeStateDirectory in the config file when
                    // --csv-only is set.
                    CodeStateDirectory = config["CodeStateDirectory"];
                }
            }
            catch (JsonException)
            {
                return ValidationResult.Error("Config file contained invalid JSON.");
            }
            catch (KeyNotFoundException e)
            {
                return ValidationResult.Error(e.Message);
            }

            // Export Directory
            if (!ForceExportDirectory && Directory.Exists(ExportDirectory) && Directory.EnumerateFileSystemEntries(ExportDirectory).Any())
            {
                // We use a lazy way to check whether a directory is empty or not.
                return ValidationResult.Error($"The supplied export directory is not empty: \"{ExportDirectory}\".");
            }
            
            // CodeState Directory
            if (CodeStateDirectory != null && !Directory.Exists(CodeStateDirectory))
            {
                return ValidationResult.Error(
                    $"Invalid path to CodeState-directory specified: \"{CodeStateDirectory}\".");
            }
            
            // Intervals
            if (StartIntervalStr != null)
            {
                if (!DateTimeOffset.TryParse(StartIntervalStr, out DateTimeOffset parsed))
                {
                    return ValidationResult.Error($"Format of start interval was invalid: \"{StartIntervalStr}\".");
                }

                StartInterval = parsed;
            }
            
            if (EndIntervalStr != null)
            {
                if (!DateTimeOffset.TryParse(EndIntervalStr, out DateTimeOffset parsed))
                {
                    return ValidationResult.Error($"Format of end interval was invalid: \"{EndIntervalStr}\".");
                }

                EndInterval = parsed;
            }
            
            return ValidationResult.Success();
        }
    }

    /// <summary>
    /// Defines the main program logic.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="settings"></param>
    /// <returns>The application's exit code</returns>
    public override int Execute(CommandContext context, Settings settings)
    {
        if (!SetUp(settings))
        {
            LogError("Failed connecting to the PostgreSQL-database.");
            return 105; // https://github.com/dotnet/templating/wiki/Exit-Codes
        }

        // TODO We should always query within the interval the first query was performed in, as in:
        //  1) If no bounds are specified, define a Now() here.
        //  2) If a bound is specified, check if it's not later in the future than now.

        // Obtain distinct sessionIds
        // Note that we order by ClientDateTime to maintain that ordening in the eventual .csv-export.
        LogInfo("Obtaining distinct sessionIDs...");
        List<Guid> sessions = _dbContext.Events
            .OrderBy(e => e.ClientDatetime)
            .Select(e => e.SessionId)
            .Distinct()
            .ToList();

        // Parse all sessions
        // This is wrapped in a progressContext to display a progress bar.
        AnsiConsole.Progress()
            .Start(ctx =>
            {
                ProgressTask task = ctx.AddTask("Parsing entries...", true, sessions.Count);
                foreach (Guid sessionId in sessions)
                {
                    ParseSession(sessionId);
                    task.Increment(1);
                }
            });
        
        return 0;
    }

    /// <summary>
    /// Handles the initialization of relevant fields,
    /// preparing the export directory and establishing a
    /// connection to the PostgreSQL-database.
    /// </summary>
    /// <param name="settings"></param>
    /// <returns>Whether the database-connection could be established</returns>
    private bool SetUp(Settings settings)
    {
        _settings = settings;
        _csvFile = Path.Join(settings.ExportDirectory, "data.csv");
        _logFile = Path.Join(settings.ExportDirectory, "log.txt");
        _codeStateExportDir = Path.Join(settings.ExportDirectory, "codestates");
        
        // Create export directory
        if (_settings.ForceExportDirectory && Directory.Exists(_settings.ExportDirectory))
        {
            Directory.Delete(_settings.ExportDirectory, true);
        }
        Directory.CreateDirectory(_settings.ExportDirectory);
        
        // Create log file, CodeState export directory
        using (FileStream fs = File.Create(_logFile)) ;
        Directory.CreateDirectory(_codeStateExportDir);

        // Set-up database
        _dbContext = new ProgSnap2ContextClass(_settings.ConnectionString);
        
        return _dbContext.Database.CanConnect();
    }
    
    #region Session parsing
    /// <summary>
    /// Parses a single session by obtaining its data, writing it to *.csv and
    /// (if enabled) restoring its codeState.
    /// </summary>
    /// <param name="sessionId"></param>
    private void ParseSession(Guid sessionId)
    {
        // TODO Included markdown is currently showing in log file
        LogInfo($"Parsing session [bold]{sessionId}[/]");
        LogInfo($"Obtaining data...");
        List<ProgSnap2Event> data = _dbContext.Events
            .Where(e => e.SessionId == sessionId)
            .OrderBy(e => e.Order)
            .ToList();
        
        if (!_settings.CsvOnly)
        {
            // We first restore the codeStates since this process might add
            // extra details to the eventual database.
            LogInfo($"Restoring CodeStates...");
            RestoreCodeState(ref data);
        }
        
        LogInfo($"Writing data to csv...");
        WriteDataToCsv(data, _csvFile);

        
    }

    /// <summary>
    /// Writes the supplied data to the *.csv-file at path.
    /// Appends when the file already existed.
    /// </summary>
    /// <param name="data">A list of ProgSnap2Events</param>
    /// <param name="path">The path to the csv-file</param>
    private static void WriteDataToCsv(List<ProgSnap2Event> data, string path)
    {
        // Largely analogous to https://joshclose.github.io/CsvHelper/examples/writing/appending-to-an-existing-file/
        if (!Path.Exists(path))
        {
            // The csv-file was not created, and we are thus not appending.
            using StreamWriter sw = new(path);
            using CsvWriter csv = new(sw, CultureInfo.InvariantCulture);
            csv.WriteRecords(data);
        }
        else
        {
            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                // If the file already existed, we should not re-include the header column of the *.csv-file.
                HasHeaderRecord = false
            };

            using FileStream fs = File.Open(path, FileMode.Append);
            using StreamWriter sw = new(fs);
            using CsvWriter csv = new(sw, config);
            csv.WriteRecords(data);
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="data"></param>
    private void RestoreCodeState(ref List<ProgSnap2Event> data)
    {
        (Guid Id, bool Full)? previousCodeState = null; 
        
        foreach (ProgSnap2Event ev in data)
        {
            string codeStateDir;
            if (ev.CodeStateId != null)
            {
                // If the codeStateId was set, a CodeState has been stored in the source for the 
                // current event. We check if that respective directory actually exists and issue
                // a warning if not.
                codeStateDir = Path.Join(_codeStateExportDir, ev.CodeStateId.ToString());
                if (!Directory.Exists(codeStateDir))
                {
                    // TODO Should maybe be ERROR; included markdown is currently showing in log file
                    LogWarning($"CodeState not present in source directory\n" +
                               $"\t[bold]SessionID[/]:\t{ev.SessionId}\n" +
                               $"\t[bold]CodeStateID[/]:\t{ev.CodeStateId}");

                    // TODO Determine what to do; copy the last complete one? 
                    continue;
                }
                
                // TODO Differentiate between full and partial states
                previousCodeState = (ev.CodeStateId.Value, ev.FullCodeState.Value);
            }

            else
            {
                // If the codeStateId was not set, we know for sure that we will have to create a new directory
                // in the export for the current event and register the corresponding ID in the exported database.
                // Also, in all cases, we need to copy the contents of the previous codeState to this new directory.
                
                if (previousCodeState == null)
                {
                    // If no previous codeState is known, we are not able to perform any work for the current event,
                    // thus we continue.
                    // TODO Should a message be included here?
                    continue;
                }
                ev.CodeStateId = new Guid();
                codeStateDir = Path.Join(_codeStateExportDir, ev.CodeStateId.ToString());
                Directory.CreateDirectory(codeStateDir);

                string previousCodeStateDir = Path.Join(_codeStateExportDir, previousCodeState.Value.Id.ToString());
                CopyDirectory(previousCodeStateDir, codeStateDir, true);
                
                // Now, some more work is required on a per-event basis.
                switch (ev.EventType)
                {
                    case EventType.EvtFileDelete:
                        // A file was deleted and thus we must delete it as well from the current CodeState.
                        // In this case, the current codeState will also be the previous one for the next iteration.
                        string path = Path.Join(codeStateDir, ev.CodeStateSection);
                        File.Delete(path);
                        previousCodeState = (ev.CodeStateId.Value, false);
                        break;
                    
                    case EventType.EvtFileRename:
                        // A file was renamed and thus we must rename it as well in the current CodeState.
                        // In this case, the current codeState will also be the previous one for the next iteration.
                        string oldPath = Path.Join(codeStateDir, ev.OldFileName);
                        string newPath = Path.Join(codeStateDir, ev.CodeStateSection);
                        File.Move(oldPath, newPath);
                        previousCodeState = (ev.CodeStateId.Value, false);
                        break;
                }
            }
        }
    }
    #endregion

    #region Logging
    
    internal enum LogLevel
    {
        Error,
        Warning, 
        Info
    }

    /// <summary>
    /// Prints a formatted info message to the AnsiConsole (if verbose-mode is enabled).
    /// Automatically adds a trailing "." and newline.
    /// </summary>
    /// <param name="message"></param>
    private void LogInfo(string message)
    {
        bool toConsole = _settings is {Quiet: false, Verbose: true};
        bool toFile = _settings.LogLevel >= LogLevel.Info;
        Log(message, "[blue]Info:[/] ", toConsole, "[INFO] ", toFile);
    }

    /// <summary>
    /// Prints a formatted warning to the AnsiConsole.
    /// Automatically adds a trailing "." and newline.
    /// </summary>
    /// <param name="message"></param>
    private void LogWarning(string message)
    {
        bool toConsole = !_settings.Quiet;
        bool toFile = _settings.LogLevel >= LogLevel.Warning;
        Log(message, "[yellow]Warning:[/] ", toConsole, "[WARNING] ", toFile);
    }
    
    /// <summary>
    /// Prints a formatted error to the AnsiConsole.
    /// Automatically adds a trailing "." and newline.
    /// </summary>
    /// <param name="message"></param>
    private void LogError(string message)
    {
        bool toConsole = !_settings.Quiet;
        bool toFile = _settings.LogLevel >= LogLevel.Error;
        Log(message, "[red]Error:[/] ", toConsole, "[ERROR] ", toFile);
    }

    /// <summary>
    /// Wraps all logging (to the logfile and the console) used by the application while
    /// enabling finer customization.
    /// </summary>
    /// <param name="message">The message to be logged</param>
    /// <param name="consoleHead">String to be included before the message when logging to the console</param>
    /// <param name="toConsole">Whether to log to the console</param>
    /// <param name="fileHead">String to be included before the message when logging to the logfile</param>
    /// <param name="toFile">Whether to log to the logfile</param>
    private void Log(string message, string consoleHead, bool toConsole, string fileHead, bool toFile)
    {
        if (toConsole)
        {
            AnsiConsole.Write(new Markup($"{consoleHead}{message}\n"));
        }

        if (toFile)
        {
            using FileStream fs = File.Open(_logFile, FileMode.Append);
            using StreamWriter sw = new(fs);
            sw.WriteLine($"{fileHead}{message}");
        }
    }
    #endregion
    
    #region Utils
    /// <summary>
    /// Copies the contents of the source directory to the destination directory.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories">Source (Microsoft)</see>
    /// <param name="sourceDir"></param>
    /// <param name="destinationDir"></param>
    /// <param name="recursive"></param>
    /// <exception cref="DirectoryNotFoundException"></exception>
    private static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        // Get information about the source directory
        DirectoryInfo dir = new(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
        }

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }
    #endregion
}

internal class Program
{
    private static int Main(string[] args)
    {
        CommandApp<ProgSnapExportCommand> app = new();
        return app.Run(args);
    }
}
