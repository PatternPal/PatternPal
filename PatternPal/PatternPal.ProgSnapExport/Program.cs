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
        
        if (sessions.Count == 0) 
        {
            LogInfo("None found within query.");
            return 0;
        }

        for (int i = 0; i < sessions.Count; ++i)
        {
            Guid sessionId = sessions[i];
            LogInfo($"Parsing session {sessionId} ({i + 1}/{sessions.Count})");
            ParseSession(sessionId);
        }

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
        _codeStateExportDir = Path.Join(settings.ExportDirectory, "CodeStates");
        
        // Create export directory
        if (_settings.ForceExportDirectory && Directory.Exists(_settings.ExportDirectory))
        {
            Directory.Delete(_settings.ExportDirectory, true);
        }
        Directory.CreateDirectory(_settings.ExportDirectory);
        
        // Create log file, CodeState export directory
        File.Create(_logFile).Close();
        if (!_settings.CsvOnly)
        {
            Directory.CreateDirectory(_codeStateExportDir);
        }

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
        LogInfo("Obtaining data...");
        List<ProgSnap2Event> data = _dbContext.Events
            .Where(e => e.ClientDatetime >= _lowerBound && e.ClientDatetime <= _upperBound)
            .Where(e => e.SessionId == sessionId)
            .OrderBy(e => e.Order)
            .ToList();

        if (!_settings.CsvOnly)
        {
            // We first restore the codeStates since this process might add
            // extra details to the eventual database.
            LogInfo("Restoring CodeStates...");
            RestoreCodeState(ref data);
        }
        
        LogInfo("Writing data to csv...");
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
    /// Restores the codeState based on the supplied data and the codeState source.
    /// </summary>
    /// <param name="data"></param>
    private void RestoreCodeState(ref List<ProgSnap2Event> data)
    {
        Dictionary<string, Guid> previousCodeState = new();
        foreach (ProgSnap2Event ev in data)
        {
            if (ev.ProjectId == null)
            {   
                // Not every codeState should have a projectID; we omit the warning for session start and session end.
                if (ev.EventType is EventType.EvtSessionStart or EventType.EvtSessionEnd)
                {
                    continue;
                }

                // Cannot reliably assess what codeState should be restored without projectID.
                LogWarning(
                    "Event does not have a projectID: could not include a reconstructed CodeState.",
                    (nameof(ev.EventId), ev.EventId.ToString()), (nameof(ev.EventType), ev.EventType.ToString())
                    );
                continue;
            }

            // Case 1: Event included codeState
            if (ev.CodeStateId != null)
            {
                string codeStateSrc = Path.Join(_settings.CodeStateDirectory, ev.CodeStateId.ToString());

                if (!Directory.Exists(codeStateSrc))
                {
                    // We need to check if the specified codeState actually exists; if it does not,
                    // we issue an error.
                    LogError(
                        "CodeState not present in source directory.",
                        (nameof(ev.SessionId), ev.SessionId.ToString()), (nameof(ev.ProjectId), ev.ProjectId), 
                        (nameof(ev.CodeStateId),ev.CodeStateId.ToString())
                        );
                    continue;
                }

                string codeStateDest = Path.Join(_codeStateExportDir, ev.CodeStateId.ToString());
                Directory.CreateDirectory(codeStateDest);
                
                // Case 1: Included full codeState
                if (ev.FullCodeState!.Value)
                {
                    if (ev.EventType is EventType.EvtFileEdit or EventType.EvtFileDelete or EventType.EvtFileRename)
                    {
                        // These events might cause a full codeState-log in case of specific errors, even though that is not the
                        // expected behaviour. If that is the case, we include a warning as well.
                        LogWarning(
                            "Event unexpectedly included a full codeState in source: please check validity of result.",
                            (nameof(ev.ProjectId), ev.ProjectId), (nameof(ev.EventId), ev.EventId.ToString()), 
                            (nameof(ev.EventType), ev.EventType.ToString())
                        );
                    }

                    // We copy the entire source to the dest.
                    CopyDirectory(codeStateSrc, codeStateDest);
                }

                // Case 2: Did not include full codeState
                else
                {
                    if (!previousCodeState.ContainsKey(ev.ProjectId))
                    {
                        // If no previous codeState was evaluated for the current project, we can only copy the partial codeState from the
                        // source to the destination export.
                        LogWarning(
                            "No previous codeState is known for the current projectID: only including partial CodeState.",
                            (nameof(ev.ProjectId), ev.ProjectId), (nameof(ev.CodeStateId), ev.CodeStateId.ToString()), 
                            (nameof(ev.EventId), ev.EventId.ToString()), (nameof(ev.EventType), ev.EventType.ToString())
                        );
                        
                        CopyDirectory(codeStateSrc, codeStateDest);
                        continue;
                    }

                    string prevCodeState = Path.Join(_codeStateExportDir, previousCodeState[ev.ProjectId].ToString());
                    CopyDirectory(prevCodeState, codeStateDest);
                    CopyDirectory(codeStateSrc, codeStateDest, true);
                }

                previousCodeState[ev.ProjectId] = ev.CodeStateId.Value;
            }

            // Case 2: Event did not include codeState
            else
            {
                if (!previousCodeState.ContainsKey(ev.ProjectId))
                {
                    // If no previous codeState is known for the current project, we are not able to perform any work for the current event,
                    // thus we continue.
                    LogWarning(
                        "No previous codeState is known for the current projectID: could not include a reconstructed CodeState.",
                        (nameof(ev.ProjectId), ev.ProjectId), (nameof(ev.EventId), ev.EventId.ToString()),
                        (nameof(ev.EventType), ev.EventType.ToString())
                    );
                    continue;
                }

                // Now we know that the current event does not have an included codeState but there is one available for the current project,
                // we are able to restore. The work now differs per event.
                switch (ev.EventType)
                {
                    case EventType.EvtFileDelete:
                    {
                        // We generate a new CodeStateID and create a new corresponding directory in the export...
                        ev.CodeStateId = Guid.NewGuid();
                        string codeStateDest = Path.Join(_codeStateExportDir, ev.CodeStateId.ToString());
                        Directory.CreateDirectory(codeStateDest);

                        //.. and subsequently copy the contents of the previous src to that destination.
                        string prevCodeStateSrc = Path.Join(_codeStateExportDir, previousCodeState[ev.ProjectId].ToString());
                        CopyDirectory(prevCodeStateSrc, codeStateDest);
                        
                        // Since a file was deleted, we must delete it from the current CodeState.
                        // Some work is needed to remove the project directory from the path (i.e. ProjectDir/FileToDelete.cs).
                        
                        // This ensures proper conversion from windows paths on other platforms
                        string projectFile = ev.ProjectId.Replace('\\', Path.DirectorySeparatorChar);
                        
                        string projectDirectory = Path.GetDirectoryName(projectFile);
                        string file = Path.GetRelativePath(projectDirectory!, ev.CodeStateSection!.Replace('\\', Path.DirectorySeparatorChar));
                        File.Delete(Path.Join(codeStateDest, file));
                        
                        // Finally, the new previous codeState is the one we just created.
                        previousCodeState[ev.ProjectId] = ev.CodeStateId.Value;
                        break;
                    }

                    case EventType.EvtFileRename:
                    {
                        // We generate a new CodeStateID and create a new corresponding directory in the export...
                        ev.CodeStateId = Guid.NewGuid();
                        string codeStateDest = Path.Join(_codeStateExportDir, ev.CodeStateId.ToString());
                        Directory.CreateDirectory(codeStateDest);

                        //.. and subsequently copy the contents of the previous src to that destination.
                        string prevCodeStateSrc = Path.Join(_codeStateExportDir, previousCodeState[ev.ProjectId].ToString());
                        CopyDirectory(prevCodeStateSrc, codeStateDest);

                        // Since a file was renamed, we must rename it in the current CodeState.
                        // Some work is needed to remove the project directory from the path (i.e. ProjectDir/FileToDelete.cs).
                        
                        // This ensures proper conversion from windows paths on other platforms
                        string projectFile = ev.ProjectId.Replace('\\', Path.DirectorySeparatorChar);
                        
                        
                        string projectDirectory = Path.GetDirectoryName(projectFile);
                        string oldFile = Path.GetRelativePath(projectDirectory!, ev.OldFileName!.Replace('\\', Path.DirectorySeparatorChar));
                        string newFile = Path.GetRelativePath(projectDirectory!, ev.CodeStateSection!.Replace('\\', Path.DirectorySeparatorChar));
                        File.Move(Path.Join(codeStateDest, oldFile), Path.Join(codeStateDest, newFile));
                        
                        // Finally, the new previous codeState is the one we just created.
                        previousCodeState[ev.ProjectId] = ev.CodeStateId.Value;
                        break;
                    }

                    default:
                        // In all other cases, simply referring to the previous codeState in the export is enough.
                        ev.CodeStateId = previousCodeState[ev.ProjectId];
                        break;
                }
            }
        }
    }
    #endregion

    #region Logging
    /// <summary>
    ///  Prints a formatted info message to the AnsiConsole (if verbose-mode is enabled).
    /// Automatically adds a trailing "." and newline.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="metadata"></param>
    private void LogInfo(string message, params (string Name, string Value)[] metadata)
    {
        bool toConsole = _settings is {Quiet: false, Verbose: true};
        bool toFile = _settings.LogLevel >= LogLevel.Info;
        Log(message, metadata, "[blue]Info:[/] ", toConsole, "[INFO] ", toFile);
    }

    /// <summary>
    /// Prints a formatted warning to the AnsiConsole.
    /// Automatically adds a trailing "." and newline.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="metadata"></param>

    private void LogWarning(string message, params (string Name, string Value)[] metadata)
    {
        bool toConsole = !_settings.Quiet;
        bool toFile = _settings.LogLevel >= LogLevel.Warning;
        Log(message, metadata, "[yellow]Warning:[/] ", toConsole, "[WARNING] ", toFile);
    }

    /// <summary>
    /// Prints a formatted error to the AnsiConsole.
    /// Automatically adds a trailing "." and newline.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="metadata"></param>

    private void LogError(string message, params (string Name, string Value)[] metadata)
    {
        bool toConsole = !_settings.Quiet;
        bool toFile = _settings.LogLevel >= LogLevel.Error;
        Log(message, metadata, "[red]Error:[/] ", toConsole, "[ERROR] ", toFile);
    }

    /// <summary>
    /// Wraps all logging (to the logfile and the console) used by the application while
    /// enabling finer customization.
    /// </summary>
    /// <param name="message">The message to be logged</param>
    /// <param name="metadata">Metadata that should also be printed. Format: (Name, Value)</param>
    /// <param name="consoleHead">String to be included before the message when logging to the console</param>
    /// <param name="toConsole">Whether to log to the console</param>
    /// <param name="fileHead">String to be included before the message when logging to the logfile</param>
    /// <param name="toFile">Whether to log to the logfile</param>
    /// 
    private void Log(string message, (string Name, string Value)[] metadata, string consoleHead, bool toConsole, string fileHead, bool toFile)
    {
        if (toConsole)
        {
           string info = metadata.Aggregate("", (current, next) => current + $"\n\t[bold]{next.Name}[/]:\t{next.Value}");
           AnsiConsole.Write(new Markup($"{consoleHead}{message}{info}\n"));
        }

        if (toFile)
        {
            using FileStream fs = File.Open(_logFile, FileMode.Append);
            using StreamWriter sw = new(fs);
            string info = metadata.Aggregate("", (current, next) => current + $"\n\t{next.Name}:\t{next.Value}");
            sw.WriteLine($"{fileHead}{message}{info}");
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
    /// <param name="overwrite">Defaults to false</param>
    /// <param name="recursive">Defaults to true</param>
    /// <exception cref="DirectoryNotFoundException"></exception>
    private static void CopyDirectory(string sourceDir, string destinationDir, bool overwrite = false, bool recursive = true)
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
            file.CopyTo(targetFilePath ,overwrite);
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
