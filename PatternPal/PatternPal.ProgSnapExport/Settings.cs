#region

using System.ComponentModel;
using System.Text.Json;

using Spectre.Console.Cli;
using Spectre.Console;
// ReSharper disable UnusedAutoPropertyAccessor.Global

#endregion

namespace PatternPal.ProgSnapExport;

/// <summary>
/// Defines all CLI-arguments used by Spectre
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class Settings : CommandSettings
{
    // CLI Settings
#pragma warning disable CS8618
    [Description("Path to config file; defaults to \"./config.json\".")]
    [CommandArgument(0, "[configFile]")]
    [DefaultValue("./config.json")]
    public string ConfigFile { get; init; }
    
    [Description("Path to export directory; defaults to \"./export\".")]
    [CommandArgument(1, "[exportDirectory]")]
    [DefaultValue("./export")]
    public string ExportDirectory { get; init;  }
#pragma warning disable CS8618

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
    public DateTime? StartInterval { get; private set; } 
    public DateTime? EndInterval { get; private set; }

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
            if (!DateTime.TryParse(StartIntervalStr, out DateTime parsed))
            {
                return ValidationResult.Error($"Format of start interval was invalid: \"{StartIntervalStr}\".");
            }

            StartInterval = parsed;
        }
            
        if (EndIntervalStr != null)
        {
            if (!DateTime.TryParse(EndIntervalStr, out DateTime parsed))
            {
                return ValidationResult.Error($"Format of end interval was invalid: \"{EndIntervalStr}\".");
            }

            EndInterval = parsed;
        }
            
        return ValidationResult.Success();
    }
}

public enum LogLevel
{
    Error,
    Warning, 
    Info
}
