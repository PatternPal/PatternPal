#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using EnvDTE;

using Grpc.Core;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;

using PatternPal.Extension.Commands;
using PatternPal.Extension.Grpc;
using PatternPal.Extension.ViewModels;
using PatternPal.Protos;

using Project = Microsoft.CodeAnalysis.Project;

#endregion

namespace PatternPal.Extension.Views
{
    /// <summary>
    ///     Interaction logic for DetectorView.xaml
    /// </summary>
    public partial class RecognizerView : IVsSolutionEvents,
                                          IVsRunningDocTableEvents
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtensionWindowControl" /> class.
        /// </summary>
        public RecognizerView()
        {
            InitializeComponent();
            AddViewModels();
            Dispatcher.VerifyAccess();
            LoadProject();
            SelectAll.IsChecked = true;
            SelectPaths.DataContext = new SelectPathsViewModel(Projects);
            Dte = Package.GetGlobalService(typeof( SDTE )) as DTE;
            IVsRunningDocumentTable rdt = (IVsRunningDocumentTable)Package.GetGlobalService(typeof( SVsRunningDocumentTable ));
            rdt.AdviseRunningDocTableEvents(
                this,
                out _);
            IVsSolution ss = (IVsSolution)Package.GetGlobalService(typeof( SVsSolution ));
            ss.AdviseSolutionEvents(
                this,
                out _);
        }

        private List< DesignPatternViewModel > ViewModels { get; set; }
        private List< Project > Projects { get; set; }
        private DTE Dte { get; }

        public int OnAfterOpenProject(
            IVsHierarchy pHierarchy,
            int fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(
            IVsHierarchy pHierarchy,
            int fRemoving,
            ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(
            IVsHierarchy pHierarchy,
            int fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(
            IVsHierarchy pStubHierarchy,
            IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(
            IVsHierarchy pRealHierarchy,
            ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(
            IVsHierarchy pRealHierarchy,
            IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        ///     Gets all the projects after opening solution.
        /// </summary>
        public int OnAfterOpenSolution(
            object pUnkReserved,
            int fNewSolution)
        {
            LoadProject();
            SelectPaths.DataContext = new SelectPathsViewModel(Projects);
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(
            object pUnkReserved,
            ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(
            object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(
            object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Adds all the existing design patterns to a list.
        /// </summary>
        private void AddViewModels()
        {
            try
            {
                GetSupportedRecognizersResponse response = GrpcHelper.RecognizerClient.GetSupportedRecognizers(new GetSupportedRecognizersRequest());

                ViewModels = new List< DesignPatternViewModel >();
                foreach (Recognizer recognizer in response.Recognizers)
                {
                    ViewModels.Add(new DesignPatternViewModel(recognizer));
                }

                PatternCheckbox.listBox.DataContext = ViewModels;

                const int maxHeight = 3 * 30;
                int height = Math.Min(
                    ViewModels.Count * 30,
                    maxHeight);

                Grid.RowDefinitions[ 3 ].Height = new GridLength(height);
            }
            catch (Exception)
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                GrpcHelper.ShowErrorMessage("Failed to get supported recognizers");
            }

            if (ViewModels == null)
            {
                AnalyzeBtn.IsEnabled = false;
            }
        }

        private void CreateResultViewModels(
            IEnumerable< RecognizeResult > results)
        {
            if (!results.Any())
            {
                NoResultsTextBlock.Visibility = Visibility.Visible;
            }
            ExpanderResults.ResultsView.ItemsSource = results.OrderByDescending(result => result.PercentageCorrectResults).Select(result => new PatternResultViewModel(result)).ToList();
        }

        private void SaveAllDocuments()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Dte.Documents.SaveAll();
        }

        private void LoadProject()
        {
            IComponentModel cm = (IComponentModel)Package.GetGlobalService(typeof( SComponentModel ));
            Workspace ws = cm.GetService< VisualStudioWorkspace >();
            Projects = ws.CurrentSolution.Projects.ToList();
        }

        /// <summary>
        ///     Handles click on the analyze_button by displaying the tool window.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void Analyze_Button(
            object sender,
            RoutedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(AnalyzeAsync).FireAndForget();
        }

        private async Task AnalyzeAsync()
        {
            if (null == Dte)
            {
                return;
            }

            try
            {
                RecognizeRequest request = await ThreadHelper.JoinableTaskFactory.RunAsync(
                    async () =>
                    {
                        // Switch to main thread, which is required to access the `DTE` service.
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        NoResultsTextBlock.Visibility = Visibility.Collapsed;

                        // Disable analyze button.
                        AnalyzeBtn.IsEnabled = false;

                        SaveAllDocuments();

                        RecognizeRequest req = new RecognizeRequest();

                        // Because we want to read properties of `SelectPaths`, which is a UI control,
                        // we need to run this on the main thread.
                        if (SelectPaths.ActiveDocument.IsChecked.HasValue
                            && SelectPaths.ActiveDocument.IsChecked.Value)
                        {
                            if (null != Dte.ActiveDocument)
                            {
                                req.File = Dte.ActiveDocument.FullName;
                            }
                        }
                        else
                        {
                            if (SelectPaths.Project.IsChecked.HasValue
                                && SelectPaths.Project.IsChecked.Value)
                            {
                                int selectedProjectIdx = SelectPaths.ProjectSelection.SelectedIndex;
                                if (selectedProjectIdx != -1)
                                {
                                    req.Project = Projects[ selectedProjectIdx ].FilePath;
                                }
                            }
                        }

                        return req;
                    });

                foreach (DesignPatternViewModel designPatternViewModel in ViewModels)
                {
                    if (!designPatternViewModel.IsChecked)
                    {
                        continue;
                    }

                    request.Recognizers.Add(designPatternViewModel.Recognizer);
                }

                try
                {
                    IAsyncStreamReader< RecognizeResponse > responseStream = GrpcHelper.RecognizerClient.Recognize(request).ResponseStream;

                    IList< RecognizeResult > results = new List< RecognizeResult >();
                    while (await responseStream.MoveNext())
                    {
                        results.Add(responseStream.Current.Result);
                    }

                    JoinableTask loggingTask = ThreadHelper.JoinableTaskFactory.RunAsync(
                        async () =>
                        {
                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                            SubscribeEvents.OnPatternRecognized(
                                request,
                                results);
                        });

                    CreateResultViewModels(results);

                    await loggingTask.JoinAsync();
                }
                catch (Exception)
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    GrpcHelper.ShowErrorMessage("Analysis failed");
                }
            }
            finally
            {
                await ThreadHelper.JoinableTaskFactory.RunAsync(
                    async () =>
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        // Enable analyze button.
                        AnalyzeBtn.IsEnabled = true;
                    });
            }
        }

        private void SelectAll_Checked(
            object sender,
            RoutedEventArgs e)
        {
            List< DesignPatternViewModel > designPatternViewModels = PatternCheckbox.listBox.Items.OfType< DesignPatternViewModel >().ToList();

            foreach (DesignPatternViewModel designPattern in designPatternViewModels)
            {
                designPattern.IsChecked = true;
            }
        }

        private void SelectAll_Unchecked(
            object sender,
            RoutedEventArgs e)
        {
            List< DesignPatternViewModel > designPatternViewModels = PatternCheckbox.listBox.Items.OfType< DesignPatternViewModel >().ToList();

            foreach (DesignPatternViewModel designPattern in designPatternViewModels)
            {
                designPattern.IsChecked = false;
            }
        }

        #region IVsRunningDocTableEvents3 implementation

        int IVsRunningDocTableEvents.OnAfterFirstDocumentLock(
            uint docCookie,
            uint dwRDTLockType,
            uint dwReadLocksRemaining,
            uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnBeforeLastDocumentUnlock(
            uint docCookie,
            uint dwRDTLockType,
            uint dwReadLocksRemaining,
            uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnAfterSave(
            uint docCookie)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnAfterAttributeChange(
            uint docCookie,
            uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnBeforeDocumentWindowShow(
            uint docCookie,
            int fFirstShow,
            IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnAfterDocumentWindowHide(
            uint docCookie,
            IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}
