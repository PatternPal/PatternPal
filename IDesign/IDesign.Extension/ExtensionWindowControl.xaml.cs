using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EnvDTE;
using IDesign.Core;
using IDesign.Core.Models;
using IDesign.Extension.ViewModels;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Project = Microsoft.CodeAnalysis.Project;
using Task = System.Threading.Tasks.Task;

namespace IDesign.Extension
{
    /// <summary>
    ///     Interaction logic for ExtensionWindowControl.
    /// </summary>
    public partial class ExtensionWindowControl : UserControl, IVsSolutionEvents
    {
        private readonly UInt32 _SolutionEventsCookie;
        private List<DesignPatternViewModel> ViewModels { get; set; }
        private List<string> Paths { get; set; }
        private List<Project> Projects { get; set; }
        private bool Loading { get; set; }
        private DTE Dte { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtensionWindowControl" /> class.
        /// </summary>
        public ExtensionWindowControl()
        {
            InitializeComponent();
            AddViewModels();
            Loading = false;
            Dispatcher.VerifyAccess();
            LoadProject();
            SelectPaths.ProjectSelection.ItemsSource = Projects;
            SelectPaths.ProjectSelection.SelectedIndex = 0;
            Dte = Package.GetGlobalService(typeof(SDTE)) as DTE;
            var ss = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
            ss.AdviseSolutionEvents(this, out _SolutionEventsCookie);
        }

        /// <summary>
        ///     Adds all the existing designpatterns in a list.
        /// </summary>
        private void AddViewModels()
        {
            ViewModels = new List<DesignPatternViewModel>();

            foreach (var pattern in RecognizerRunner.designPatterns)
                ViewModels.Add(new DesignPatternViewModel(pattern.Name, pattern, pattern.WikiPage));

            PatternCheckbox.listBox.DataContext = ViewModels;
            var height = ViewModels.Count * 30;

            if (height > 3 * 30)
                height = 3 * 30;

            Grid.RowDefinitions[2].Height = new GridLength(height);
        }

        /// <summary>
        ///     Creates the viewmodel for the treeview.
        /// </summary>
        private void CreateResultViewModels(IEnumerable<RecognitionResult> results)
        {
            var viewModels = new List<ResultViewModel>();

            foreach (var item in RecognizerRunner.designPatterns)
            {
                var patterns = results.Where(x => x.Pattern.Equals(item));
                if (patterns.Count() > 0)
                {
                    viewModels.AddRange(patterns.OrderBy(x => x.Result.GetScore()).Select(x => new ResultViewModel(x)));
                }
            }

            //Here you signal the UI thread to execute the action:
            Dispatcher?.BeginInvoke(new Action(() =>
            {
                // - Change your UI information here
                TreeViewResults.ResultsView.ItemsSource = viewModels;
            }), null);
        }

        /// <summary>
        ///     Gets current active document path.
        /// </summary>
        private void ChoosePath()
        {
            Paths = new List<string>();

            if ((bool)SelectPaths.radio1.IsChecked)
            {
                if (Dte.ActiveDocument != null)
                    Paths.Add(Dte.ActiveDocument.FullName);
            }
            else
            {
                LoadProject();
                var selectedI = SelectPaths.ProjectSelection.SelectedIndex;
                Paths.AddRange(Projects[selectedI].Documents.Select(x => x.FilePath));
            }
        }

        private void SaveAllFiles()
        {
            var soln = Dte.Solution;

            for (int i = 1; i <= soln.Projects.Count; i++)
            {
                if (!soln.Projects.Item(i).Saved)
                {
                    soln.Projects.Item(i).Save();
                }
                for (int j = 1; j <= soln.Projects.Item(i).ProjectItems.Count; j++)
                {
                    if (!soln.Projects.Item(i).ProjectItems.Item(j).Saved)
                    {
                        soln.Projects.Item(i).ProjectItems.Item(j).Save();
                    }
                }
            }
        }

        /// <summary>
        ///     Loads the project to get all the available projects.
        /// </summary>
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
            SaveAllFiles();
            ChoosePath();
            var SelectedPatterns = ViewModels.Where(x => x.IsChecked).Select(x => x.Pattern).ToList();

            if (Loading || Paths.Count == 0 || SelectedPatterns.Count == 0)
                return;

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
                    TreeViewResults.SyntaxTreeSources = runner.CreateGraph(Paths);
                    var results = runner.Run(SelectedPatterns);
                    CreateResultViewModels(results);
                }
                catch
                {
                    //@TODO User friendly error handling
                }
            });

            ResetUI();
        }

        private void ResetUI()
        {
            statusBar.Value = 0;
            Loading = false;
            ProgressStatusBlock.Text = "";
        }

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

        private void SelectAll_Checked(object sender, RoutedEventArgs e)
        {
            var designPatternViewModels = PatternCheckbox.listBox.Items.OfType<DesignPatternViewModel>().ToList();

            for (int i = 0; i < designPatternViewModels.Count(); i++)
            {
                designPatternViewModels[i].IsChecked = true;
            }
        }

        private void SelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            var designPatternViewModels = PatternCheckbox.listBox.Items.OfType<DesignPatternViewModel>().ToList();

            for (int i = 0; i < designPatternViewModels.Count(); i++)
            {
                designPatternViewModels[i].IsChecked = false;
            }
        }
    }
}