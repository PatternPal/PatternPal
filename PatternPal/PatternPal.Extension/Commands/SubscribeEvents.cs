using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternPal.Extension.Commands;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Grpc.Core;
using PatternPal.Extension.Grpc;
using StreamJsonRpc;
using EnvDTE;
using PatternPal.Extension.UserControls;
using PatternPal.Extension.ViewModels;
using EnvDTE80;
using PatternPal.Protos;
using PatternPal.Extension.Model;

namespace PatternPal.Extension.Commands
{
    public static class SubscribeEvents
    {
        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        private static PatternPalExtensionPackage _package;

        private static DTE _dte;
        public static void Initialize(
            DTE dte,
            PatternPalExtensionPackage package)
        {
            _dte = dte;
            ThreadHelper.ThrowIfNotOnUIThread();
            _dte.Events.BuildEvents.OnBuildDone += BuildEvents_OnBuildDone;
            _package = package;
            SetSubjectId();
        }

        private static void BuildEvents_OnBuildDone(
            vsBuildScope Scope,
            vsBuildAction Action)
        {
       
            if (!_package.DoLogData)
            {
                return;
            }

            //Check compile result
            ThreadHelper.ThrowIfNotOnUIThread();
            string outputMessage;
            if (_dte.Solution.SolutionBuild.LastBuildInfo != 0)
            {
                outputMessage = string.Format("Build {0} with errors. See the output window for details.",
                    Action.ToString());
                // Build failed, display error message
            }
            else
            {
                outputMessage = string.Format("Build {0} succeeded.", Action.ToString());
                // Build succeeded, display success message
            }


            //Request to service
            LogEventRequest request = new LogEventRequest
            {
                SubjectId = GetSubjectId(),
                CompileResult = outputMessage
            };

            LoggingService.LoggingServiceClient client =
                new LoggingService.LoggingServiceClient(GrpcHelper.Channel);
            LogEventResponse response = client.LogEvent(request);
      
        }

        /// <summary>
        /// Sets the SubjectId of the user, if not set already, as a GUID.
        /// It creates a folder and a file for this at the UserLocalDataPath in the PatternPal Extension folder.
        /// </summary>
        static void SetSubjectId()
        {
            if (!_package.DoLogData) return;

            string pathToFolder = Path.Combine(_package.UserLocalDataPath.ToString(), "Extensions", "Team PatternPal",
                "PatternPal.Extension");
            string dataDir = Path.Combine(pathToFolder, "UserData");
            if (Directory.Exists(dataDir))
            {
                return;
            }

            Directory.CreateDirectory(dataDir);
            string fileName = "subjectid.txt";
            string filePath = Path.Combine(dataDir, fileName);
            string fileContents = Guid.NewGuid().ToString();
            File.WriteAllText(filePath, fileContents);
        }

        /// <summary>
        /// Returns the SubjectId of this specific user. 
        /// </summary>
        /// <returns></returns>
        static string GetSubjectId()
        {
            string pathToFolder = Path.Combine(_package.UserLocalDataPath.ToString(), "Extensions", "Team PatternPal",
                "PatternPal.Extension");
            string dataDir = Path.Combine(pathToFolder, "UserData");
            // Create the data directory and file if it doesn't exist
            Directory.CreateDirectory(dataDir);
            string fileName = "subjectid.txt";
            string filePath = Path.Combine(dataDir, fileName);
            return File.ReadAllText(filePath);
        }


    }
}
