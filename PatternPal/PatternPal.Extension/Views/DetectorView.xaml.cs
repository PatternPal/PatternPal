using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EnvDTE;
using PatternPal.Core;
using PatternPal.Core.Models;
using PatternPal.Extension.ViewModels;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Project = Microsoft.CodeAnalysis.Project;

namespace PatternPal.Extension.Views
{
    /// <summary>
    ///     Interaction logic for DetectorView.xaml
    /// </summary>
    public partial class DetectorView : UserControl, IVsSolutionEvents, IVsRunningDocTableEvents
    {
        private readonly SummaryFactory SummaryFactory = new SummaryFactory();

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
            Dte = Package.GetGlobalService(typeof(SDTE)) as DTE;
            var rdt = (IVsRunningDocumentTable)Package.GetGlobalService(typeof(SVsRunningDocumentTable));
            rdt.AdviseRunningDocTableEvents(this, out _);
            var ss = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
            ss.AdviseSolutionEvents(this, out _);
        }

        private List<DesignPatternViewModel> ViewModels { get; set; }
        private List<string> Paths { get; set; }
        private List<Project> Projects { get; set; }
        private bool Loading { get; set; }
        private DTE Dte { get; }
        private List<RecognitionResult> Results { get; set; }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        ///     Gets all the projects after opening solution.
        /// </summary>
        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            LoadProject();
            SelectPaths.ProjectSelection.ItemsSource = Projects;
            SelectPaths.ProjectSelection.SelectedIndex = 0;
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }


        /// <summary>
        ///     Adds all the existing designpatterns in a list.
        /// </summary>
        private void AddViewModels()
        {
            ViewModels = (from pattern in RecognizerRunner.DesignPatterns
                select new DesignPatternViewModel(pattern.Name, pattern, pattern.WikiPage)).ToList();

            PatternCheckbox.listBox.DataContext = ViewModels;

            var maxHeight = 3 * 30;
            var height = Math.Min(ViewModels.Count * 30, maxHeight);

            Grid.RowDefinitions[3].Height = new GridLength(height);
        }

        private void CreateResultViewModels(IEnumerable<RecognitionResult> results)
        {
            var viewModels = new List<PatternResultViewModel>();
            foreach (var patterns in from item in RecognizerRunner.DesignPatterns
                     let patterns = results.Where(x => x.Pattern.Equals(item))
                     where patterns.Count() > 0
                     select patterns)
            {
                viewModels.AddRange(patterns.OrderBy(x => x.Result.GetScore())
                    .Select(x => new PatternResultViewModel(x)));
            }

            // - Change your UI information here
             ExpanderResults.ResultsView.ItemsSource = viewModels;
        }


        private List<string> GetCurrentPath()
        {
            var result = new List<string>();

            if ((bool)SelectPaths.radio1.IsChecked && Dte.ActiveDocument != null)
            {
                result.Add(Dte.ActiveDocument.FullName);
            }

            return result;
        }

        private void GetAllPaths()
        {
            Paths = new List<string>();
            var selectedI = SelectPaths.ProjectSelection.SelectedIndex;
            if (selectedI != -1)
            {
                Paths.AddRange(Projects[selectedI].Documents.Select(x => x.FilePath));
            }
        }

        private void ChoosePath()
        {
            GetAllPaths();
        }

        private void SaveAllDocuments()
        {
            Dte.Documents.SaveAll();
        }


        private void LoadProject()
        {
            var cm = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            var ws = (Workspace)cm.GetService<VisualStudioWorkspace>();
            Projects = ws.CurrentSolution.Projects.ToList();
        }

        /// <summary>
        ///     Handles click on the analyse_button by displaying the tool window.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification =
            "Default event handler naming pattern")]
        private async void Analyse_Button(object sender, RoutedEventArgs e)
        {
            SaveAllDocuments();
            Analyse();
        }

        private void SelectProjectFromFile(string path = null)
        {
            var list = Projects.SelectMany(
                project => project.Documents.Where(doc => doc.FilePath == path)
                    .Select(doc => new { doc, index = Projects.IndexOf(project) })
                    .Select(@t => @t.index)
            ).ToList();
            
            if (list.Count == 0) return;
            SelectPaths.ProjectSelection.SelectedIndex = list.First();
        }

        private async Task Analyse()
        {
            LoadProject();
            var cur = GetCurrentPath().FirstOrDefault();
            SelectProjectFromFile(cur);
            ChoosePath();
            var SelectedPatterns = ViewModels.Where(x => x.IsChecked).Select(x => x.Pattern).ToList();

            if (!Loading && Paths.Count != 0 && SelectedPatterns.Count != 0)
            {
                if ((bool)SelectPaths.radio1.IsChecked)
                {
                    CheckSwitch.IsChecked = true;
                }
                else
                {
                    CheckSwitch.IsChecked = false;
                }

                var runner = new RecognizerRunner();
                Loading = true;
                statusBar.Value = 0;
                var progress = new Progress<RecognizerProgress>(value =>
                {
                    statusBar.Value = value.CurrentPercentage;
                    ProgressStatusBlock.Text = value.Status;
                });

                IProgress<RecognizerProgress> iprogress = progress;
                runner.OnProgressUpdate += (o, recognizerProgress) => iprogress.Report(recognizerProgress);

                await Task.Run(() =>
                {
                    try
                    {
                        runner.CreateGraph(Paths);
                        Results = runner.Run(SelectedPatterns);

                        //Here you signal the UI thread to execute the action:
                        Dispatcher?.BeginInvoke(new Action(() =>
                        {
                            var results = Results;
                            var allResults = Results;
                            if ((bool)SelectPaths.radio1.IsChecked)
                            {
                                results = results.Where(x => x.FilePath == cur).ToList();
                                Results = results;

                                SummaryRow.Height = GridLength.Auto;
                            }
                            else
                            {
                                SummaryRow.Height = new GridLength(0);
                            }

                            if (!(bool)CheckSwitch.IsChecked)
                            {
                                results = Results.Where(x => x.Result.GetScore() >= 80).ToList();
                            }

                            SummaryControl.Text = SummaryFactory.CreateSummary(results, allResults);
                            CreateResultViewModels(results);
                            ResetUI();
                        }));
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    ;
                });
            }
        }

        private void CheckSwitch_Checked(object sender, RoutedEventArgs e)
        {
            if (Results == null)
            {
                return;
            }

            CreateResultViewModels(Results);
        }

        private void CheckSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Results == null)
            {
                return;
            }

            var results = Results.Where(x => x.Result.GetScore() >= 80).ToList();
            CreateResultViewModels(results);
        }

        private void ResetUI()
        {
            statusBar.Value = 0;
            Loading = false;
            ProgressStatusBlock.Text = "";
        }

        private void SelectAll_Checked(object sender, RoutedEventArgs e)
        {
            var designPatternViewModels = PatternCheckbox.listBox.Items.OfType<DesignPatternViewModel>().ToList();

            for (var i = 0; i < designPatternViewModels.Count(); i++)
            {
                designPatternViewModels[i].IsChecked = true;
            }
        }

        private void SelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            var designPatternViewModels = PatternCheckbox.listBox.Items.OfType<DesignPatternViewModel>().ToList();

            for (var i = 0; i < designPatternViewModels.Count(); i++)
            {
                designPatternViewModels[i].IsChecked = false;
            }
        }

        #region IVsRunningDocTableEvents3 implementation

        int IVsRunningDocTableEvents.OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType,
            uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType,
            uint dwReadLocksRemaining,
            uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnAfterSave(uint docCookie)
        {
            Analyse();
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}
