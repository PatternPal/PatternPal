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
using Microsoft.VisualStudio.Threading;
using System.Threading;

namespace PatternPal.Extension.Commands
{

    /// <summary>
    /// A static class which is responsible for subscribing logged events.
    /// </summary>
    public static class SubscribeEvents
    {
        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        private static PatternPalExtensionPackage _package;

        private static DTE _dte;

        private static string _sessionId;

        private static string _pathToUserDataFolder;

        private static CancellationToken _cancellationToken;

        /// <summary>
        /// Initializes the preparation for the subscription of the logged events. 
        /// </summary>
        /// <param name="dte"></param>
        /// <param name="package"> The PatternPal package itself. </param>
        public static void Initialize(
            DTE dte,
            PatternPalExtensionPackage package, CancellationToken cancellationToken)
        {
            _dte = dte;
            ThreadHelper.ThrowIfNotOnUIThread();
            _package = package;
            _pathToUserDataFolder = Path.Combine(_package.UserLocalDataPath.ToString(), "Extensions", "Team PatternPal",
                "PatternPal.Extension", "UserData");
            _cancellationToken = cancellationToken;
            SaveSubjectId();


            // These events are not handled with an event listener, and thus need to be checked separately whether logging is enabled.
            if (_package.DoLogData)
            {
                OnSessionStart();
            }
        }

        /// <summary>
        /// Subscribes the event handlers for logging data.
        /// </summary>
        public static async Task SubscribeEventHandlersAsync()
        {
            await _package.JoinableTaskFactory.SwitchToMainThreadAsync(_cancellationToken);
            // Code that interacts with UI elements goes here
            _dte.Events.BuildEvents.OnBuildDone += OnCompileDone;
        }


        /// <summary>
        /// Unsubscribes the event handlers for logging data.
        /// </summary>
        public static async Task UnsubscribeEventHandlersAsync()
        {

            await _package.JoinableTaskFactory.SwitchToMainThreadAsync(_cancellationToken);
            _dte.Events.BuildEvents.OnBuildDone -= OnCompileDone;
        }

        /// <summary>
        /// The event handler for handling the Compile Event. The given parameters are part of the event listener input and among other things necessary to give the right output message.
        /// </summary>
        /// <param name="Scope"></param>
        /// <param name="Action"></param>
        private static void OnCompileDone(
            vsBuildScope Scope,
            vsBuildAction Action)
        {

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
                OnCompileError(request);
            }
        }

        /// <summary>
        /// The event handler for handling the Session.Start Event. When a new session starts, a (new) sessionID is generated .
        /// </summary>
        private static void OnSessionStart()
        {
            _sessionId = Guid.NewGuid().ToString();

            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtSessionStart;
            LogProviderService.LogProviderServiceClient client =
                new LogProviderService.LogProviderServiceClient(GrpcHelper.Channel);
            LogEventResponse response = client.LogEvent(request);
        }

        /// <summary>
        /// The event handler for handling the Session.End Event. This method needs to be called from the package logic, hence the given internal modifier.
        /// </summary>
        internal static void OnSessionEnd()
        {
            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtSessionEnd;
            LogProviderService.LogProviderServiceClient client =
                new LogProviderService.LogProviderServiceClient(GrpcHelper.Channel);
            LogEventResponse response = client.LogEvent(request);
        }

        /// <summary>
        /// The event handler for handling the Compile.Error Event. This is strictly a complementary event to the regular Compile event.
        /// </summary>
        private static void OnCompileError(LogEventRequest parent)
        {
            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtCompileError;
            request.ParentEventId = parent.EventId;
            LogProviderService.LogProviderServiceClient client =
                new LogProviderService.LogProviderServiceClient(GrpcHelper.Channel);
            LogEventResponse response = client.LogEvent(request);
        }

        /// <summary>
        /// Creates a standard log format with set fields that are always the same. Consequently, it is used by all other specific logs.
        /// </summary>
        /// <returns>The standard log format.</returns>
        private static LogEventRequest CreateStandardLog()
        {
            return new LogEventRequest { SubjectId = GetSubjectId(), SessionId = _sessionId };
        }

        /// <summary>
        /// Saves the SubjectId of the user, if not set already, as a GUID.
        /// It creates a folder and a file for this at the UserLocalDataPath in the PatternPal Extension folder as this place is unique per user.
        /// </summary>
        private static void SaveSubjectId()
        {

            // A SubjectID is only ever generated once per user. If the directory already exists, the SubjectID was already set.
            if (Directory.Exists(_pathToUserDataFolder))
            {
                return;
            }

            Directory.CreateDirectory(_pathToUserDataFolder);
            string fileName = "subjectid.txt";
            string filePath = Path.Combine(_pathToUserDataFolder, fileName);
            string fileContents = Guid.NewGuid().ToString();
            File.WriteAllText(filePath, fileContents);
        }

        /// <summary>
        /// Reads the SubjectID from a local file.
        /// </summary>
        /// <returns>The SubjectID - It returns the contents of the local file.</returns>
        private static string GetSubjectId()
        {
            Directory.CreateDirectory(_pathToUserDataFolder);
            string fileName = "subjectid.txt";
            string filePath = Path.Combine(_pathToUserDataFolder, fileName);
            return File.ReadAllText(filePath);
        }
    }
}
