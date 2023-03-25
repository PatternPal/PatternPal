#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using EnvDTE;

using Grpc.Core;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

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
    public partial class DetectorView : UserControl,
                                        IVsSolutionEvents,
                                        IVsRunningDocTableEvents
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtensionWindowControl" /> class.
        /// </summary>
        public DetectorView()
        {
            InitializeComponent();
            AddViewModels();
            Loading = false;
            Dispatcher.VerifyAccess();
            LoadProject();
            SelectAll.IsChecked = true;
            SelectPaths.ProjectSelection.ItemsSource = Projects;
            SelectPaths.ProjectSelection.SelectedIndex = 0;
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
        private List< string > Paths { get; set; }
        private List< Project > Projects { get; set; }
        private bool Loading { get; set; }
        private DTE Dte { get; }
        private List< RecognizeResult > Results { get; set; }

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
            SelectPaths.ProjectSelection.ItemsSource = Projects;
            SelectPaths.ProjectSelection.SelectedIndex = 0;
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
            ViewModels = new List< DesignPatternViewModel >();
            foreach (object value in Enum.GetValues(typeof( Recognizer )))
            {
                ViewModels.Add(new DesignPatternViewModel((Recognizer)value));
            }

            PatternCheckbox.listBox.DataContext = ViewModels;

            const int maxHeight = 3 * 30;
            int height = Math.Min(
                ViewModels.Count * 30,
                maxHeight);

            Grid.RowDefinitions[ 3 ].Height = new GridLength(height);
        }

        private void CreateResultViewModels(
            IEnumerable< RecognizeResult > results)
        {
            List< PatternResultViewModel > viewModels = new List< PatternResultViewModel >();

            foreach (RecognizeResult result in results)
            {
                viewModels.Add(new PatternResultViewModel(result));
            }

            // - Change your UI information here
            ExpanderResults.ResultsView.ItemsSource = viewModels;
        }

        private void SaveAllDocuments()
        {
            Dte.Documents.SaveAll();
        }

        private void LoadProject()
        {
            IComponentModel cm = (IComponentModel)Package.GetGlobalService(typeof( SComponentModel ));
            Workspace ws = cm.GetService< VisualStudioWorkspace >();
            Projects = ws.CurrentSolution.Projects.ToList();
        }

        /// <summary>
        ///     Handles click on the analyse_button by displaying the tool window.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void Analyse_Button(
            object sender,
            RoutedEventArgs e)
        {
            SaveAllDocuments();
            Analyse();
        }

        private async Task Analyse()
        {
            RecognizeRequest request = new RecognizeRequest();

            // TODO CV: Handle error cases
            if (SelectPaths.ActiveDocument.IsChecked.HasValue
                && SelectPaths.ActiveDocument.IsChecked.Value)
            {
                if (null != Dte.ActiveDocument)
                {
                    request.File = Dte.ActiveDocument.FullName;
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
                        request.Project = Projects[ selectedProjectIdx ].FilePath;
                    }
                }
            }

            foreach (DesignPatternViewModel designPatternViewModel in ViewModels)
            {
                if (!designPatternViewModel.IsChecked)
                {
                    continue;
                }

                request.Recognizers.Add(designPatternViewModel.Recognizer);
            }

            IAsyncStreamReader< RecognizeResponse > responseStream = GrpcHelper.RecognizerClient.Recognize(request).ResponseStream;

            IList< RecognizeResult > results = new List< RecognizeResult >();
            while (await responseStream.MoveNext())
            {
                results.Add(responseStream.Current.Result);
            }

            CreateResultViewModels(results);
            SummaryControl.Text = "Recognizer is finished";
            ResetUI();
        }

        private void CheckSwitch_Checked(
            object sender,
            RoutedEventArgs e)
        {
            if (Results == null)
            {
                return;
            }

            //CreateResultViewModels(Results);
        }

        private void CheckSwitch_Unchecked(
            object sender,
            RoutedEventArgs e)
        {
            if (Results == null)
            {
                return;
            }

            List< RecognizeResult > results = Results.Where(x => x.Score >= 80).ToList();
            //CreateResultViewModels(results);
        }

        private void ResetUI()
        {
            //statusBar.Value = 0;
            Loading = false;
            ProgressStatusBlock.Text = "";
        }

        private void SelectAll_Checked(
            object sender,
            RoutedEventArgs e)
        {
            List< DesignPatternViewModel > designPatternViewModels = PatternCheckbox.listBox.Items.OfType< DesignPatternViewModel >().ToList();

            for (int i = 0;
                 i < designPatternViewModels.Count();
                 i++)
            {
                designPatternViewModels[ i ].IsChecked = true;
            }
        }

        private void SelectAll_Unchecked(
            object sender,
            RoutedEventArgs e)
        {
            List< DesignPatternViewModel > designPatternViewModels = PatternCheckbox.listBox.Items.OfType< DesignPatternViewModel >().ToList();

            for (int i = 0;
                 i < designPatternViewModels.Count();
                 i++)
            {
                designPatternViewModels[ i ].IsChecked = false;
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
            Analyse();
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
