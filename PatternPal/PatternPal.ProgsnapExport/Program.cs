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

#endregion

namespace PatternPal.ProgSnapExport;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class ProgSnapExportCommand : Command<ProgSnapExportCommand.Settings>
{
    private ProgSnap2ContextClass _dbContext;
    private Settings _settings;

    private string _logFile;
    private string _csvFile;
    private string _codeStateExport;

    /// <summary>
    /// Defines all CLI-arguments.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
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
    /// TODO
    /// </summary>
    /// <param name="context"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public override int Execute(CommandContext context, Settings settings)
    {
        _settings = settings;
        _csvFile = Path.Join(settings.ExportDirectory, "data.csv");
        _logFile = Path.Join(settings.ExportDirectory, "log.txt");
        _codeStateExport = Path.Join(settings.ExportDirectory, "codestates");
        
        // Create export directory
        if (_settings.ForceExportDirectory && Directory.Exists(_settings.ExportDirectory))
        {
            Directory.Delete(_settings.ExportDirectory, true);
        }
        Directory.CreateDirectory(_settings.ExportDirectory);
        
        // Create log file
        using (FileStream fs = File.Create(_logFile)) ;
        
        // Create CodeState export directory
        Directory.CreateDirectory(_codeStateExport);

        // Set-up database
        _dbContext = new ProgSnap2ContextClass(_settings.ConnectionString);
        if (!_dbContext.Database.CanConnect())
        {
            LogError("Failed connecting to the PostgreSQL-database");
            return 105; // https://github.com/dotnet/templating/wiki/Exit-Codes
        }

        // TODO We should always query within the interval the first query was performed in, as in:
        //  1) If no bounds are specified, define a Now() here.
        //  2) If a bound is specified, check if it's not later in the future than now.

        // Obtain distinct sessionIds
        // Note that we order by ClientDateTime to maintain that ordening
        // in the eventual .csv-export.
        LogInfo("Obtaining distinct sessionIDs..");
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

    #region Session parsing
    /// <summary>
    /// Parses a single session by obtaining its data, writing it to *.csv and
    /// (if enabled) restoring its codeState.
    /// </summary>
    /// <param name="sessionId"></param>
    private void ParseSession(Guid sessionId)
    {
        LogInfo($"Parsing session [bold]{sessionId}[/]..");
        LogInfo($"Obtaining data..");
        List<ProgSnap2Event> data = _dbContext.Events
            .Where(e => e.SessionId == sessionId)
            .OrderBy(e => e.Order)
            .ToList();
        
        LogInfo($"Writing data to csv..");
        WriteDataToCsv(data, _csvFile);

        if (!_settings.CsvOnly)
        {
            LogInfo($"Restoring CodeStates..");
            RestoreCodeState();
        }
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
            using StreamWriter sw = new StreamWriter(path);
            using CsvWriter csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
            csv.WriteRecords(data);
        }
        else
        {
            CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // If the file already existed, we should not re-include the header column of the *.csv-file.
                HasHeaderRecord = false
            };

            using FileStream fs = File.Open(path, FileMode.Append);
            using StreamWriter sw = new StreamWriter(fs);
            using CsvWriter csv = new CsvWriter(sw, config);
            csv.WriteRecords(data);
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    private static void RestoreCodeState()
    {
        //TODO
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
        if(_settings is {Quiet: false, Verbose: true})
        {
            AnsiConsole.Write(new Markup($"[blue]Info:[/] {message}.\n"));
        }

        if (_settings.LogLevel >= LogLevel.Info)
        {
            using FileStream fs = File.Open(_logFile, FileMode.Append);
            using StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine($"[INFO] {message}");
        }
    }

    /// <summary>
    /// Prints a formatted warning to the AnsiConsole.
    /// Automatically adds a trailing "." and newline.
    /// </summary>
    /// <param name="message"></param>
    private void LogWarning(string message)
    {
        if (!_settings.Quiet)
        {
            AnsiConsole.Write(new Markup($"[orange]Warning:[/] {message}.\n"));
        }
        
        if (_settings.LogLevel >= LogLevel.Warning)
        {
            using FileStream fs = File.Open(_logFile, FileMode.Append);
            using StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine($"[WARNING] {message}");
        }
    }
    
    /// <summary>
    /// Prints a formatted error to the AnsiConsole.
    /// Automatically adds a trailing "." and newline.
    /// </summary>
    /// <param name="message"></param>
    private void LogError(string message)
    {
        if (!_settings.Quiet)
        {
            AnsiConsole.Write(new Markup($"[red]Error:[/] {message}.\n"));
        }
        
        if (_settings.LogLevel >= LogLevel.Error)
        {
            using FileStream fs = File.Open(_logFile, FileMode.Append);
            using StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine($"[ERROR] {message}");
        }
    }
    #endregion
}

// ReSharper disable once ClassNeverInstantiated.Global
internal class Program
{
    private static int Main(string[] args)
    {
        CommandApp<ProgSnapExportCommand> app = new();
        return app.Run(args);
    }
}
