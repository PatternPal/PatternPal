#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using PatternPal.Core;
using PatternPal.Core.Runner;
using PatternPal.Protos;

using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;

#endregion

namespace PatternPal.ConsoleApp;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class PatternPalCommand : Command< Settings >
{
    public override int Execute(
        CommandContext context,
        Settings settings)
    {
        FileManager fileManager = new();

        FileAttributes attributes = File.GetAttributes(settings.FileOrDirectory);
        List< string > selectedFiles = (attributes & FileAttributes.Directory) == FileAttributes.Directory
            ? fileManager.GetAllCSharpFilesFromDirectory(settings.FileOrDirectory)
            : new List< string >
              {
                  settings.FileOrDirectory
              };

        AnsiConsole.WriteLine("Selected files:");
        AnsiConsole.Write(new Rows(selectedFiles.Select(file => new TextPath(file))));

        RecognizerRunner recognizerRunner = new(
            selectedFiles,
            new[ ]
            {
                settings.Pattern
            } );

        IList< (Recognizer, ICheckResult) > result = recognizerRunner.Run();

        string jsonOutput = JsonSerializer.Serialize(
            result.Select(res => res.Item2),
            new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter()
                },
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
                // NOTE: Workaround to ignore the required `ICheckResult.Check` property.
                // See: https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/required-properties
                TypeInfoResolver = new DefaultJsonTypeInfoResolver
                                   {
                                       Modifiers =
                                       {
                                           static ti =>
                                           {
                                               if (ti.Kind != JsonTypeInfoKind.Object)
                                               {
                                                   return;
                                               }

                                               foreach (JsonPropertyInfo propertyInfo in ti.Properties)
                                               {
                                                   propertyInfo.IsRequired = false;
                                               }
                                           }
                                       }
                                   }
            });

        if (settings.Save.HasValue
            && settings.Save.Value)
        {
            string pattern = settings.Pattern.ToString();
            string outputFileName = Path.Combine(
                Directory.GetCurrentDirectory(),
                $"{pattern}.json");

            if (File.Exists(outputFileName))
            {
                if (!AnsiConsole.Confirm($"'{pattern}.json' already exists, do you want to overwrite it?"))
                {
                    string name = AnsiConsole.Ask< string >("Enter a file name:");
                    outputFileName = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        $"{name}.json");
                }
            }

            File.WriteAllText(
                outputFileName,
                jsonOutput);

            AnsiConsole.WriteLine($"Saved output to '{outputFileName}'");
        }
        else
        {
            AnsiConsole.WriteLine("Results:");
            AnsiConsole.Write(new JsonText(jsonOutput));
        }

        return 0;
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class Settings : CommandSettings
{
    [Description("Pattern to recognize")]
    [CommandArgument(
        0,
        "<Pattern>")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required Recognizer Pattern { get; init; }

    [Description("File or directory to run the recognizers on")]
    [CommandArgument(
        1,
        "<FileOrDirectory>")]
    public required string FileOrDirectory { get; set; }

    [Description("Save the JSON output to a file")]
    [CommandOption("-s|--save")]
    public bool ? Save { get; set; }

    public override ValidationResult Validate()
    {
        ValidationResult baseValidationResult = base.Validate();
        if (baseValidationResult.Successful)
        {
            FileOrDirectory = Path.GetFullPath(FileOrDirectory);
            if (!Path.Exists(FileOrDirectory))
            {
                throw new InvalidOperationException("File or directory does not exist");
            }
        }

        return baseValidationResult;
    }
}

internal static class Program
{
    private static int Main(
        string[ ] args)
    {
        CommandApp< PatternPalCommand > app = new();
        return app.Run(args);
    }
}
