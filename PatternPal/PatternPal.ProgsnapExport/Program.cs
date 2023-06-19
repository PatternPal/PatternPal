#region

using System.ComponentModel;

using Spectre.Console.Cli;
using Spectre.Console;

#endregion

namespace PatternPal.ProgSnapExport;

internal sealed class ProgSnapExportCommand : Command<ProgSnapExportCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        // TODO Add validation
        [Description("Path to config file; defaults to \"./settings.json\".")]
        [CommandArgument(0, "[configFile]")]
        [DefaultValue("./settings.json")]
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

        [CommandOption("-v|--verbose")]
        [DefaultValue(false)]
        public bool Verbose { get; init; }

        // TODO Add storage option for online

        public override ValidationResult Validate()
        {
            base.Validate();

            return File.Exists(ConfigFile)
                ? ValidationResult.Success()
                : ValidationResult.Error("Invalid path to config file specified");
            
            // TODO Add timestamp validation
        }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        Console.WriteLine(settings.ConfigFile);
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
