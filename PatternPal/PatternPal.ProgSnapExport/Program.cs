#region

using System.Globalization;

using PatternPal.ProgSnapExport.Data;
using PatternPal.LoggingServer.Models;
using PatternPal.LoggingServer;

using Spectre.Console.Cli;
using Spectre.Console;

using CsvHelper;
using CsvHelper.Configuration;

#endregion

namespace PatternPal.ProgSnapExport;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class ProgSnapExportCommand : Command<Settings>
{
    // We are disabling this warning since we know for sure that in every context
    // these fields are used, they will be set.
#pragma warning disable CS8618
    private ProgSnap2ContextClass _dbContext;

    private Settings _settings;

    private string _logFile;
    private string _csvFile;
    private string _codeStateExportDir;

    private DateTime _lowerBound;
    private DateTime _upperBound;
#pragma warning restore CS8618

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

        // Obtain distinct sessionIds
        // Note that we order by ClientDateTime to maintain that ordening in the eventual .csv-export.
        LogInfo("Obtaining distinct sessionIDs...");
        List<Guid> sessions = _dbContext.Events
            .Where(e => e.ClientDatetime >= _lowerBound && e.ClientDatetime <= _upperBound)
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
        File.Create(_logFile).Close();
        Directory.CreateDirectory(_codeStateExportDir);

        // Prepare interval bounds
        // We set the lower and the upper bounds to our query;
        // if StartInterval is defined we check if it is later than epoch and -- if so -- convert to UTC.
        // Similarly we parse the EndInterval, but we compare it against now.
        _lowerBound = DateTime.UnixEpoch;
        if (_settings.StartInterval != null && _settings.StartInterval.Value > _lowerBound)
        {
            _lowerBound = DateTime.SpecifyKind(_settings.StartInterval.Value, DateTimeKind.Utc);
        }

        _upperBound = DateTime.UtcNow;
        if (_settings.EndInterval != null && _settings.EndInterval.Value < _upperBound)
        {
            _upperBound = DateTime.SpecifyKind(_settings.EndInterval.Value, DateTimeKind.Utc);
        }
        
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
        LogInfo($"Parsing session [bold]{sessionId}[/]");
        LogInfo($"Obtaining data...");
        List<ProgSnap2Event> data = _dbContext.Events
            .Where(e => e.ClientDatetime >= _lowerBound && e.ClientDatetime <= _upperBound)
            .Where(e => e.SessionId == sessionId)
            .OrderBy(e => e.Order)
            .ToList();
        
        if (!_settings.CsvOnly)
        {
            // We first restore the codeStates since this process might add
            // extra details to the eventual database.
            LogInfo($"Restoring CodeStates...");
            
            // We need to obtain all unique projectIDs...
            List<string> projects = _dbContext.Events
                .Where(e => e.ClientDatetime >= _lowerBound && e.ClientDatetime <= _upperBound)
                .Select(e => e.ProjectId)
                .Where(projectId => projectId != null)
                .Distinct()
                .ToList()!;
            
            projects.ForEach(projectId => RestoreProject(projectId, ref data));
            
            // TODO Should we create a warning for each event without a projectId?
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
    /// <param name="projectId"></param>
    /// <param name="data"></param>
    private void RestoreProject(string projectId, ref List<ProgSnap2Event> data)
    {
        LogInfo($"Restoring project [bold]{projectId}[/]");
        
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
                               $"\t[bold]ProjectID[/]:\t{projectId}\n" +
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
            // TODO logging multiple lines renders improperly in conjunction with the progress bar
            AnsiConsole.Write(new Markup($"{consoleHead}{message}\n"));
        }

        if (toFile)
        {
            // TODO Markdown directives are currently included in log file
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

// ReSharper disable once ClassNeverInstantiated.Global
internal class Program
{
    private static int Main(string[] args)
    {
        CommandApp<ProgSnapExportCommand> app = new();
        return app.Run(args);
    }
}
