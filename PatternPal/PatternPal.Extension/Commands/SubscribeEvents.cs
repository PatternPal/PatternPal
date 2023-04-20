using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
using Microsoft.Win32;
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

        private static string _sessionId;

        private static string _pathToFolder;
        public static void Initialize(  
            DTE dte,
            PatternPalExtensionPackage package)
        {
            _dte = dte;
            ThreadHelper.ThrowIfNotOnUIThread();
            _package = package;
            _pathToFolder = Path.Combine(_package.UserLocalDataPath.ToString(), "Extensions", "Team PatternPal",
                "PatternPal.Extension");
            SetSubjectId();
            OnSessionStart();
            _dte.Events.BuildEvents.OnBuildDone += OnBuildDone;
        }

        private static void OnBuildDone(
            vsBuildScope Scope,
            vsBuildAction Action)
        {
            if (!_package.DoLogData)
            {
                return;
            }

            ThreadHelper.ThrowIfNotOnUIThread();
            string outputMessage;
            if (_dte.Solution.SolutionBuild.LastBuildInfo != 0)
            {
                outputMessage = string.Format("Build {0} with errors. See the output window for details.",
                    Action.ToString());

                // As the compilation led to an error, a separate  log is sent with the compile error diagnostics
                // and the specific code section in which the compilation error occurred
            }
            else
            {
                outputMessage = string.Format("Build {0} succeeded.", Action.ToString());
            }


            LogEventRequest request = new LogEventRequest
            {
                SubjectId = GetSubjectId(),
                EventType = EventType.EvtCompile,
                CompileResult = outputMessage,
                SessionId = _sessionId
            };

            LogProviderService.LogProviderServiceClient client =
                new LogProviderService.LogProviderServiceClient(GrpcHelper.Channel);
            LogEventResponse response = client.LogEvent(request);

            // When the compilation was an error, a Compile Error log needs to be send.
            if (_dte.Solution.SolutionBuild.LastBuildInfo != 0)
            {
                CompileError(request);
            }
        }

        private static void OnSessionStart()
        {
            _sessionId = Guid.NewGuid().ToString();

            if (!_package.DoLogData)
            {
                return;
            }

            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtSessionStart;
            LogProviderService.LogProviderServiceClient client =
                new LogProviderService.LogProviderServiceClient(GrpcHelper.Channel);
            LogEventResponse response = client.LogEvent(request);
        }

        internal static void OnSessionEnd()
        {
            if (!_package.DoLogData)
            {
                return;
            }

            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtSessionEnd;
            LogProviderService.LogProviderServiceClient client =
                new LogProviderService.LogProviderServiceClient(GrpcHelper.Channel);
            LogEventResponse response = client.LogEvent(request);
        }

        private static void CompileError(LogEventRequest parent)
        {
            if (!_package.DoLogData)
            {
                return;
            }

            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtCompileError;
            request.ParentEventId = parent.EventId; //Dus the event id kan niet in server generate worden
            LogProviderService.LogProviderServiceClient client =
                new LogProviderService.LogProviderServiceClient(GrpcHelper.Channel);
            LogEventResponse response = client.LogEvent(request);
        }

        private static LogEventRequest CreateStandardLog()
        {
            return new LogEventRequest { SubjectId = GetSubjectId(), SessionId = _sessionId };
        }

        /// <summary>
        /// Sets the SubjectId of the user, if not set already, as a GUID.
        /// It creates a folder and a file for this at the UserLocalDataPath in the PatternPal Extension folder as this place is unique per user.
        /// </summary>
        private static void SetSubjectId()
        {
            if (!_package.DoLogData) return;

            string dataDir = Path.Combine(_pathToFolder, "UserData");
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
        private static string GetSubjectId()
        {
            string dataDir = Path.Combine(_pathToFolder, "UserData");
            Directory.CreateDirectory(dataDir);
            string fileName = "subjectid.txt";
            string filePath = Path.Combine(dataDir, fileName);
            return File.ReadAllText(filePath);
        }
    }
}
