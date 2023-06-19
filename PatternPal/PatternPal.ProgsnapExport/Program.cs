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
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class Settings : CommandSettings
    {
        // TODO Add validation
        [Description("Path to config file; defaults to \"./config.json\".")]
        [CommandArgument(0, "[configFile]")]
        [DefaultValue("./config.json")]
        // ReSharper disable once 
        public string ConfigFile { get; init; }

        [Description("Path to export directory; defaults to \"./export\".")]
        [CommandArgument(1, "[exportDirectory]")]
        [DefaultValue("./export")]
        public string ExportDirectory { get; init;  }

        // TODO Add format description to timestamps; add validation; proper type
        [Description("Specifies the starting point of the time interval to be exported.")]
        [CommandOption("-s|--start")]
        public string? StartInterval { get; init; }

        // TODO Add format description to timestamps; add validation; proper type
        [Description("Specifies the ending point of the time interval to be exported.")]
        [CommandOption("-e|--end")]
        public string? EndInterval { get; init; }
        
        [CommandOption("--csv-only")]
        [DefaultValue(false)]
        public bool CsvOnly { get; init; }

        [CommandOption("-v|--verbose")]
        [DefaultValue(false)]
        public bool Verbose { get; init; }

        public string ConnectionString { get; private set; }

        // TODO Add storage option for online

        /// <summary>
        /// TODO
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
            {
                if (!File.Exists(ConfigFile))
                {
                    return ValidationResult.Error("Invalid path to config file specified");
                }

                try
                {
                    Dictionary<string, string> config =
                        JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(ConfigFile));
                    ConnectionString =
                        $"Server={config["ServerUrl"]};Username={config["Username"]};Database={config["Database"]};Password={config["Password"]}";
                }
                catch
                {
                    return ValidationResult.Error("Invalid JSON specified");
                }
            }
            
            // TODO Add timestamp validation
            
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
