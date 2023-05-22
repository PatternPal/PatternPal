#region 

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using PatternPal.Extension.Grpc;
using EnvDTE;
using EnvDTE80;
using PatternPal.Protos;
using System.Threading;

#endregion

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

        private static DebuggerEvents _dteDebugEvents;

        private static SolutionEvents _dteSolutionEvents;

        private static BuildEvents _dteBuildEvents;

        private static string _sessionId;

        private static bool _unhandledExceptionThrown;

        public static string SessionId
        {
            get { return _sessionId; }
            set { _sessionId = value; }
        }

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
            _dteDebugEvents = _dte.Events.DebuggerEvents;
            _dteSolutionEvents = _dte.Events.SolutionEvents;
            _dteBuildEvents = _dte.Events.BuildEvents;
            _package = package;
            _pathToUserDataFolder = Path.Combine(_package.UserLocalDataPath.ToString(), "Extensions", "Team PatternPal",
                "PatternPal.Extension", "UserData");
            _cancellationToken = cancellationToken;
            SaveSubjectId();

            // This activates the DoLogData, necessary here in Initialize to kickstart the Session and Project Open events.
            bool _ = _package.DoLogData;
        }

        /// <summary>
        /// Subscribes the event handlers for logging data.
        /// Be careful with calling this method. This method should only be called once.
        /// </summary>
        public static async Task SubscribeEventHandlersAsync()
        {
            await _package.JoinableTaskFactory.SwitchToMainThreadAsync(_cancellationToken);
            // Code that interacts with UI elements goes here
            _dteBuildEvents.OnBuildDone += OnCompileDone;
            _dteSolutionEvents.Opened += OnProjectOpen;
            _dteSolutionEvents.BeforeClosing += OnProjectClose;
            _dteDebugEvents.OnEnterBreakMode +=
                OnExceptionUnhandled; // OnEnterBreakMode is triggered for both breakpoints as well as exceptions, with the reason parameter specifying this.
            _dteDebugEvents.OnEnterDesignMode += OnDebugProgram;
        }


        /// <summary>
        /// Unsubscribes the event handlers for logging data.
        /// </summary>
        public static async Task UnsubscribeEventHandlersAsync()
        {
            await _package.JoinableTaskFactory.SwitchToMainThreadAsync(_cancellationToken);
            _dteBuildEvents.OnBuildDone -= OnCompileDone;
            _dteSolutionEvents.Opened -= OnProjectOpen;
            _dteSolutionEvents.BeforeClosing -= OnProjectClose;
            _dteDebugEvents.OnEnterBreakMode -= OnExceptionUnhandled;
            _dteDebugEvents.OnEnterDesignMode -= OnDebugProgram;
        }


        #region Events

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

            LogEventRequest request = CreateStandardLog();
            string pathSolutionFullName = _dte.Solution.FullName;
            string pathSolutionFile = _dte.Solution.FileName;
            // Distinguish a sln file or just a csproj file to be opened
            if (pathSolutionFullName == "")
            {
                // A csproj file was opened
                Array startupProjects = (Array)_dte.Solution.SolutionBuild.StartupProjects;
                request.CodeStateSection = (string)startupProjects.GetValue(0);
            }
            else
            {
                string pathSolutionDirectory = Path.GetDirectoryName(_dte.Solution.FullName);

                request.CodeStateSection = GetRelativePath(pathSolutionDirectory, pathSolutionFile);
            }

            request.EventType = EventType.EvtCompile;
            request.CompileResult = outputMessage;

            LogProviderService.LogProviderServiceClient client =
                new LogProviderService.LogProviderServiceClient(GrpcHelper.Channel);
            LogEventResponse response = client.LogEvent(request);

            // When the compilation was an error, a Compile Error log needs to be send.
            if (_dte.Solution.SolutionBuild.LastBuildInfo != 0)
            {
                Window window = _dte.Windows.Item(WindowKinds.vsWindowKindErrorList);
                ErrorList errorListWindow = (ErrorList)window.Selection;

                for (int i = 1; i <= errorListWindow.ErrorItems.Count; i++)
                {
                    ErrorItem errorItem = errorListWindow.ErrorItems.Item(i);
                    string errorType = errorItem.ErrorLevel.ToString();
                    string errorMessage = errorItem.Description;
                    string errorSourceLocation = String.Concat("Text:" + errorItem.Line);

                    // The relative path of the source file for the error is required for the ProgSnap format
                    string projectFolderName =
                        Path.GetDirectoryName(_dte.Solution.Projects.Item(errorItem.Project).FullName);
                    string codeStateSection = GetRelativePath(projectFolderName, errorItem.FileName);

                    OnCompileError(request, errorType, errorMessage, errorSourceLocation, codeStateSection);
                }
            }
        }

        /// <summary>
        /// The event handler for handling the Session.Start Event. When a new session starts, a (new) sessionID is generated .
        /// </summary>
        internal static void OnSessionStart()
        {
            SessionId = Guid.NewGuid().ToString();
           
            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtSessionStart;

            LogEventResponse response = PushLog(request);
        }

        /// <summary>
        /// The event handler for handling the Session.End Event. This method needs to be called from the package logic, hence the given internal modifier.
        /// </summary>
        internal static void OnSessionEnd()
        {
            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtSessionEnd;

            LogEventResponse response = PushLog(request);
        }

        /// <summary>
        /// The event handler for handling the Compile.Error Event. This is strictly a complementary event to the regular Compile event for each separate error.
        /// </summary>
        private static void OnCompileError(LogEventRequest parent, string compileMessagetype, string compileMessageData,
            string sourceLocation, string codeStateSection)
        {
            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtCompileError;
            request.ParentEventId = parent.EventId;
            request.CompileMessageType = compileMessagetype;
            request.CompileMessageData = compileMessageData;
            request.SourceLocation = sourceLocation;
            request.CodeStateSection = codeStateSection;

            LogEventResponse response = PushLog(request);
        }

        /// <summary>
        /// The event handler for handling the Project.Open Event.
        /// </summary>
        internal static void OnProjectOpen()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string test = _dte.Solution.FullName;

            LogEachProject(EventType.EvtProjectOpen);
        }

        /// <summary>
        /// The event handler for handling the Project.Close Event.
        /// </summary>
        internal static void OnProjectClose()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            LogEachProject(EventType.EvtProjectClose);
        }

        /// <summary>
        /// The event handler for handling the Debug.Program Event.
        /// </summary>
        private static void OnDebugProgram(dbgEventReason reason)
        {
            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtDebugProgram;
            request.ExecutionId = Guid.NewGuid().ToString();

            if (_unhandledExceptionThrown)
            {
                request.ExecutionResult = ExecutionResult.ExtError;
                _unhandledExceptionThrown = false;
            }
            else
            {
                request.ExecutionResult = ExecutionResult.ExtSucces;
            }

            LogEventResponse response = PushLog(request);
        }

        #endregion

        /// <summary>
        /// Creates a standard log format with set fields that are always generated. Consequently, it is used by all other specific logs.
        /// </summary>
        /// <returns>The standard log format.</returns>
        private static LogEventRequest CreateStandardLog()
        {
            return new LogEventRequest
            {
                EventId = Guid.NewGuid().ToString(), SubjectId = GetSubjectId(), SessionId = _sessionId
            };
        }

        /// <summary>
        /// Sends the log request to the background service in order to be processed to the server.
        /// </summary>
        private static LogEventResponse PushLog(LogEventRequest request)
        {
            LogProviderService.LogProviderServiceClient client =
                new LogProviderService.LogProviderServiceClient(GrpcHelper.Channel);

            return client.LogEvent(request);
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
            // A SubjectID is only ever generated once per user. If the directory already exists, the SubjectID was already set.

            string fileName = "subjectid.txt";
            string filePath = Path.Combine(_pathToUserDataFolder, fileName);
            return File.ReadAllText(filePath);
        }

        // TODO Separate utility because of duplication with extension
        /// <summary>
        /// Gets the relative path when given an absolute directory path and a filename.
        /// </summary>
        /// <param name="relativeTo"> The absolute path to the root folder</param>
        /// <param name="path">The absolute path to the specific file</param>
        public static string GetRelativePath(string relativeTo, string path)
        {
            Uri uri = new Uri(relativeTo);
            string rel = Uri.UnescapeDataString(uri.MakeRelativeUri(new Uri(path)).ToString())
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (rel.Contains(Path.DirectorySeparatorChar.ToString()) == false)
            {
                rel = $".{Path.DirectorySeparatorChar}{rel}";
            }

            return rel;
        }

        /// <summary>
        /// Cycles through all active projects and log the given event for each of these projects.
        /// </summary>
        private static void LogEachProject(EventType eventType)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Projects projects = _dte.Solution.Projects;

            foreach (Project project in projects)
            {
                if (project == null)
                {
                    continue;
                }

                LogEventRequest request = CreateStandardLog();
                request.EventType = eventType;

                // Since LogEachProject is only called by either Project.Open or Project.Close,
                // we include the full CodeState.
                request.ProjectId = project.UniqueName;
                request.FilePath = Path.GetDirectoryName(project.FullName);

                LogEventResponse response = PushLog(request);
            }
        }

        /// <summary>
        /// Event handler for when an exception is unhandled. This is used to determine in the user's last debug session
        /// whether there were any unhandled exceptions. Although creating a separate method might seem redundant
        /// for the actual logic used here, it is necessary for the adding and removing from any used event listeners.
        /// </summary>
        public static void OnExceptionUnhandled(dbgEventReason reason, ref dbgExecutionAction executionAction)
        {
            if (reason == dbgEventReason.dbgEventReasonExceptionNotHandled)
            {
                _unhandledExceptionThrown = true;
            }
        }
    }
}
