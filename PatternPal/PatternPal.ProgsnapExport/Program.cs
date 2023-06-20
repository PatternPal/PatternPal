#region

using System.ComponentModel;
using System.Text.Json;

using Spectre.Console.Cli;
using Spectre.Console;

#endregion

namespace PatternPal.ProgSnapExport;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class ProgSnapExportCommand : Command<ProgSnapExportCommand.Settings>
{
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

        // TODO Add format description to timestamps; add validation; proper type
        [Description("Specifies the starting point of the time interval to be exported. Format: \"YYYY-MM-DD HH:MM:SS\".")]
        [CommandOption("-s|--start")]
        public string? StartIntervalStr { get; init; }

        // TODO Add format description to timestamps; add validation; proper type
        [Description("Specifies the ending point of the time interval to be exported. Format: \"YYYY-MM-DD HH:MM:SS\".")]
        [CommandOption("-e|--end")]
        public string? EndIntervalStr { get; init; }
        
        [CommandOption("--csv-only")]
        [DefaultValue(false)]
        public bool CsvOnly { get; init; }

        [CommandOption("-v|--verbose")]
        [DefaultValue(false)]
        public bool Verbose { get; init; }

        // Properties
        public string ConnectionString { get; private set; }
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
            }
            catch (JsonException)
            {
                return ValidationResult.Error("Config file contained invalid JSON.");
            }
            catch (KeyNotFoundException e)
            {
                return ValidationResult.Error(e.Message);
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
        Console.WriteLine(settings.ConnectionString);
        return 0;
    }
}

internal class Program
{
    private static int Main(string[] args)
    {
        CommandApp<ProgSnapExportCommand> app = new();
        return app.Run(args);
    }
}
