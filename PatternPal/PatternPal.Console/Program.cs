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
            result.Select((_, r) => r),
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

        AnsiConsole.WriteLine("Results:");
        AnsiConsole.Write(new JsonText(jsonOutput));

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
