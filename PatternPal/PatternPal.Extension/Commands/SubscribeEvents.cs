#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using PatternPal.Extension.Grpc;
using PatternPal.Protos;

#endregion

namespace PatternPal.Extension.Commands
{
    public enum ExtensionLogStatusCodes
    {
        Available,
        Unavailable,
        Error,
        NoLog
    }
    /// <summary>
    /// A static class which is responsible for subscribing logged event in ProgSnap2 format.
    /// </summary>
    public static class SubscribeEvents
    {
        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        private static ExtensionWindowPackage _package;

        private static DTE _dte;

        private static DebuggerEvents _dteDebugEvents;

        private static DocumentEvents _dteDocumentEvents;

        private static SolutionEvents _dteSolutionEvents;

        private static BuildEvents _dteBuildEvents;

        private static Solution _currentSolution;

        private static FileSystemWatcher _watcher;

        private static bool _unhandledExceptionThrown;

        public static string SessionId { get; set; }

        private static bool _doLog = false;

        private static CancellationToken _cancellationToken;

        private static ExtensionLogStatusCodes _serverStatus = ExtensionLogStatusCodes.NoLog;

        public static ExtensionLogStatusCodes ServerStatus
        {
            get
            {
                if (_doLog) {
                    return _serverStatus;
                }
                return ExtensionLogStatusCodes.NoLog;
            }
            set => _serverStatus = value;
        }

        public static Action ServerStatusChanged = delegate { };



        /// <summary>
        /// Initializes the preparation for the subscription of the logged events. 
        /// </summary>
        /// <param name="dte"></param>
        /// <param name="package"> The PatternPal package itself. </param>
        public static void Initialize(
            DTE dte,
            ExtensionWindowPackage package, CancellationToken cancellationToken)
        {
            _dte = dte;
            ThreadHelper.ThrowIfNotOnUIThread();
            _dteDebugEvents = _dte.Events.DebuggerEvents;
            _dteSolutionEvents = _dte.Events.SolutionEvents;
            _dteBuildEvents = _dte.Events.BuildEvents;
            _currentSolution = _dte.Solution;
            _dteDocumentEvents = _dte.Events.DocumentEvents;
            _package = package;
            _cancellationToken = cancellationToken;

            // We should call OnChangedLoggingPreference to "load" a possibly stored setting. The
            // application always stored with the internal flag set to false, so this will only actually
            // do something when it was stored as true (and subsequently kickstart the logging session).
            OnChangedLoggingPreference(Privacy.Instance);
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
            _dteSolutionEvents.Opened += OnSolutionOpen;
            _dteSolutionEvents.BeforeClosing += OnSolutionClose;
            _dteDebugEvents.OnEnterBreakMode +=
                OnExceptionUnhandled; // OnEnterBreakMode is triggered for both breakpoints as well as exceptions, with the reason parameter specifying this.
            _dteDebugEvents.OnEnterDesignMode += OnDebugProgram;
            _dteDocumentEvents.DocumentSaved += OnDocumentSaved;
        }


        /// <summary>
        /// Unsubscribes the event handlers for logging data.
        /// </summary>
        public static async Task UnsubscribeEventHandlersAsync()
        {
            await _package.JoinableTaskFactory.SwitchToMainThreadAsync(_cancellationToken);
            _dteBuildEvents.OnBuildDone -= OnCompileDone;
            _dteSolutionEvents.Opened -= OnSolutionOpen;
            _dteSolutionEvents.BeforeClosing -= OnSolutionClose;
            _dteDebugEvents.OnEnterBreakMode -= OnExceptionUnhandled;
            _dteDebugEvents.OnEnterDesignMode -= OnDebugProgram;
            _dteDocumentEvents.DocumentSaved -= OnDocumentSaved;
        }

        /// <summary>
        /// Event handler for a changed logging preference in the privacy screen.
        /// </summary>
        /// <param name="obj"></param>
        public static void OnChangedLoggingPreference(Privacy obj)
        {
            // We explicitly check if the value has changed. 
            if (_doLog == obj.DoLogData)
            {
                return;
            }

            _doLog = obj.DoLogData;

            // If the value changes, a new session is either started or ended.
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await (obj.DoLogData
                    ? SubscribeEventHandlersAsync()
                    : UnsubscribeEventHandlersAsync());
            });

            ThreadHelper.ThrowIfNotOnUIThread();
            if (obj.DoLogData)
            {
                OnSessionStart();
                OnSolutionOpen();
            }
            else
            {
                OnSolutionClose();
                OnSessionEnd();
            }
        }

        #region Events

        /// <summary>
        /// The event handler for handling the Compile Event. The given parameters are part of the event listener input and among other things necessary to give the right output message.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="action"></param>
        private static void OnCompileDone(
            vsBuildScope scope,
            vsBuildAction action)
        {
            ThreadHelper.ThrowIfNotOnUIThread(); ;
            string outputMessage = _dte.Solution.SolutionBuild.LastBuildInfo != 0 ? 
                $"Build {action.ToString()} with errors. See the output window for details." : 
                $"Build {action.ToString()} succeeded.";

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
                string pathSolutionDirectory = Path.GetDirectoryName(_currentSolution.FullName);

                request.CodeStateSection = GetRelativePath(pathSolutionDirectory, pathSolutionFile);
            }

            request.EventType = EventType.EvtCompile;
            request.CompileResult = outputMessage;

            LogEventResponse response = PushLog(request);

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
        /// The event handler for handling the File.Create Event. The file watcher detects every file created,
        /// so any event triggers with files other than .cs files are unhandled.    
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="fileSystemEventArgs"></param>
        internal static void OnFileCreate(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            // Only log for the creation of .cs files
            if (Path.GetExtension(fileSystemEventArgs.Name) != ".cs")
            {
                return;
            }

            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtFileCreate;
            request.CodeStateSection = fileSystemEventArgs.Name;
            string projectFullPath = FindContainingCsprojFile(fileSystemEventArgs.FullPath);
            string projectFolderName = Path.GetDirectoryName(projectFullPath);
            request.ProjectId = GetRelativePath(projectFolderName, projectFullPath);

            LogEventResponse response = PushLog(request);
        }

        /// <summary>
        /// The event handler for handling the File.Delete Event. The file watcher detects every file deleted,
        /// so any event triggers with files other than .cs files are unhandled.    
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="fileSystemEventArgs"></param>
        internal static void OnFileDelete(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            // Only log for the creation of .cs files
            if (Path.GetExtension(fileSystemEventArgs.Name) != ".cs")
            {
                return;
            }

            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtFileDelete;
            request.CodeStateSection = fileSystemEventArgs.Name;
            string projectFullPath = FindContainingCsprojFile(fileSystemEventArgs.FullPath);
            string projectFolderName = Path.GetDirectoryName(projectFullPath);
            request.ProjectId = GetRelativePath(projectFolderName, projectFullPath);

            LogEventResponse response = PushLog(request);
        }

        /// <summary>
        /// The event handler for handling the Session.Start Event. When a new session starts, a (new) sessionID is generated.
        /// A new file watcher is also created, as a new session can change the directory the user is working in.
        /// </summary>
        internal static void OnSessionStart()
        {
            //SubjectId = Privacy.Instance.SubjectId;
            SessionId = Guid.NewGuid().ToString();

            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtSessionStart;

            LogEventResponse response = PushLog(request);

            // As a new session has started, the file watcher has to be reset so that the "current solution" is up to date
            SetUpFileWatcher();
        }

        /// <summary>
        /// The event handler for handling the Session.End Event. This method needs to be called from the package logic, hence the given internal modifier.
        /// </summary>
        internal static void OnSessionEnd()
        {
            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtSessionEnd;

            LogEventResponse response = PushLog(request);

            // When a session has ended, no logs are sent anymore so the watcher can be disposed.
            _watcher.Dispose();
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
        internal static void OnSolutionOpen()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            LogEachProject(EventType.EvtProjectOpen);

            // When a user opens a new solution, the watcher has to be setup again to entail the new solution. 
            SetUpFileWatcher();
        }

        /// <summary>
        /// The event handler for handling the Project.Close Event.
        /// </summary>
        internal static void OnSolutionClose()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            LogEachProject(EventType.EvtProjectClose);

            // When a user closes a solution, the watcher has to be disposed as the directory saved is then not up-to-date anymore.
            _watcher.Dispose();
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

        /// <summary>
        /// The event handler for handling the File.Edit Event. This fires at every save of a document regardless whether a change
        /// has been made to the file or not. This remains File.Edit, and not File.Save as during the handling of diffs,
        /// it is determined whether an edit has been made and if so, it is sent with the codebase - therefore: File.Edit.
        /// </summary>
        /// <param name="document">The document that is being saved.</param>
        private static void OnDocumentSaved(Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtFileEdit;
            
            request.CodeStateSection = GetRelativePath(Path.GetDirectoryName(document.FullName), document.FullName);
            request.ProjectId = document.ProjectItem.ContainingProject.UniqueName;
            request.ProjectDirectory = Path.GetDirectoryName(document.ProjectItem.ContainingProject.FullName);
            request.FilePath = document.FullName;

            LogEventResponse response = PushLog(request);
        }

        /// <summary>
        /// The event handler for when recognizing software patterns in the extension.
        /// </summary>
        public static void OnPatternRecognized(RecognizeRequest recognizeRequest,
            IList<RecognizeResult> recognizeResults)
        {
            if (_package == null || !Privacy.Instance.DoLogData)
            { 
              return; 
            }
            
            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtXRecognizerRun;
            string config = recognizeRequest.Recognizers.ToString();

            request.RecognizerConfig = config;
            foreach (RecognizeResult result in recognizeResults)
            {
                request.RecognizerResult += result.ToString();
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
                EventId = Guid.NewGuid().ToString(), SubjectId = Privacy.Instance.SubjectId, SessionId = SessionId
            };
        }

        /// <summary>
        /// Sends the log request to the background service in order to be processed to the server.
        /// </summary>
        private static LogEventResponse PushLog(LogEventRequest request)
        {
            LogProviderService.LogProviderServiceClient client =
                new LogProviderService.LogProviderServiceClient(GrpcHelper.Channel);
            try
            {
                LogEventResponse ler = client.LogEvent(request);
                ServerStatusChanged?.Invoke();
                switch (ler.Status)
                {
                    case LogStatusCodes.LscSuccess:
                        ServerStatus = ExtensionLogStatusCodes.Available;
                        return ler;
                    case LogStatusCodes.LscUnavailable:
                        ServerStatus = ExtensionLogStatusCodes.Unavailable;
                        return ler;
                    case LogStatusCodes.LscFailure:
                    case LogStatusCodes.LscRejected:
                    case LogStatusCodes.LscInvalidArguments:
                    case LogStatusCodes.LscUnknown:
                    default:
                        ServerStatus = ExtensionLogStatusCodes.Error;
                        return ler;
                }
            }
            // Host not found, timeout exception, etc.
            catch (Exception e)
            {
                ServerStatus = ExtensionLogStatusCodes.Unavailable;
                return new LogEventResponse
                {
                    Status = LogStatusCodes.LscUnknown,
                    Message = e.Message
                };
            }
        }

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

                // This catches miscellaneous projects without a defined project.Fullname; the cause of this is unknown since
                // we are using internal-use-only libraries for event catching.
                if (project.FullName == null || !File.Exists((project.FullName)))
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
        /// Sets up a file watcher for File.Create and File.Delete events for the current opened user solution.
        /// </summary>
        private static void SetUpFileWatcher()
        {
            _watcher?.Dispose();

            // Create a new FileSystemWatcher instance
            // TODO We need to explicitely check if that path is not null and handle other cases.
            _watcher = new FileSystemWatcher(Path.GetDirectoryName(_currentSolution.FullName));

            // Set the event handlers
            _watcher.Created += OnFileCreate;
            _watcher.Deleted += OnFileDelete;

            // Enable the FileSystemWatcher to begin watching for changes
            _watcher.EnableRaisingEvents = true;

            // Enable watching for in the subdirectories as well
            _watcher.IncludeSubdirectories = true;
        }

        /// <summary>
        /// Traverse the directory structure to find the .csproj file that contains the given file path.
        /// </summary>
        /// <param name="filePath">The full filepath of the target file. </param>
        /// <returns>The full path of the found .csproj file.</returns>
        private static string FindContainingCsprojFile(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);

            while (!string.IsNullOrEmpty(directory))
            {
                string csprojFile = Directory.GetFiles(directory, "*.csproj").FirstOrDefault();

                if (!string.IsNullOrEmpty(csprojFile))
                    return csprojFile;

                directory = Path.GetDirectoryName(directory);
            }

            return "";
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
