using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EnvDTE;
using IDesign.Core;
using IDesign.Core.Models;
using IDesign.Extension.ViewModels;
using IDesign.Recognizers.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Project = Microsoft.CodeAnalysis.Project;
using Task = System.Threading.Tasks.Task;

namespace IDesign.Extension
{
    /// <summary>
    ///     Interaction logic for ExtensionWindowControl.
    /// </summary>
    public partial class ExtensionWindowControl : UserControl, IVsSolutionEvents
    {
        private UInt32 _SolutionEventsCookie;
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtensionWindowControl" /> class.
        /// </summary>
        public ExtensionWindowControl()
        {
            InitializeComponent();
            AddViewModels();
            Loading = false;
            Dispatcher.VerifyAccess();
            Dte = Package.GetGlobalService(typeof(SDTE)) as DTE;
            var ss = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
            ss.AdviseSolutionEvents(this, out _SolutionEventsCookie);
        }

        private void SolutionEventsOnOpened()
        {
            var project = Dte.ActiveSolutionProjects;
        }

        public List<DesignPatternViewModel> ViewModels { get; set; }
        public Dictionary<SyntaxTree, string> SyntaxTreeSources { get; set; }
        public List<string> Paths { get; set; }
        public List<Project> Projects { get; set; }
        public bool Loading { get; set; }
        public DTE Dte { get; }

        /// <summary>
        ///     Adds all the existing designpatterns in a list.
        /// </summary>
        private void AddViewModels()
        {
            ViewModels = new List<DesignPatternViewModel>();

            foreach (var pattern in RecognizerRunner.designPatterns)
                ViewModels.Add(new DesignPatternViewModel(pattern.Name, pattern));

            listBox.DataContext = ViewModels;

            var height = ViewModels.Count * 30;

            if (height > 3 * 30)
                height = 3 * 30;

            Grid.RowDefinitions[1].Height = new GridLength(height);
        }

        private void CreateResultViewModels(IEnumerable<RecognitionResult> results)
        {
            var viewModels = new List<ClassViewModel>();

            foreach (var result in results)
            {
                var classViewModel = viewModels.FirstOrDefault(x => x.EntityNode == result.EntityNode);
                if (classViewModel == null)
                {
                    classViewModel = new ClassViewModel(result.EntityNode);
                    viewModels.Add(classViewModel);
                }

                var resultViewModel = new ResultViewModel(result);
                classViewModel.Results.Add(resultViewModel);
            }

            //Here you signal the UI thread to execute the action:
            Dispatcher?.BeginInvoke(new Action(() =>
            {
                // - Change your UI information here
                ResultsView.ItemsSource = viewModels;
            }), null);
        }

        /// <summary>
        ///     Gets current active document path.
        /// </summary>
        private void GetCurrentPath()
        {
            Paths = new List<string>();
            if (Dte.ActiveDocument != null)
                Paths.Add(Dte.ActiveDocument.FullName);
        }

        /// <summary>
        ///     Gets all paths in the solution.
        /// </summary>
        private void GetAllPaths()
        {
            Paths = new List<string>();
            var selectedI = ProjectSelection.SelectedIndex;
            Paths.AddRange(Projects[selectedI].Documents.Select(x => x.FilePath));
        }

        private void ChoosePath()
        {
            if ((bool)radio1.IsChecked)
                GetCurrentPath();
            else
                GetAllPaths();
        }


        private void selectNodeInEditor(SyntaxNode n, string file)
        {
            try
            {
                var cm = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
                var tm = (IVsTextManager)Package.GetGlobalService(typeof(SVsTextManager));
                var ws = (Workspace)cm.GetService<VisualStudioWorkspace>();
                var did = ws.CurrentSolution.GetDocumentIdsWithFilePath(file);
                ws.OpenDocument(did.FirstOrDefault());
                tm.GetActiveView(1, null, out var av);
                var sp = n.GetLocation().GetMappedLineSpan().StartLinePosition;
                var ep = n.GetLocation().GetMappedLineSpan().EndLinePosition;
                av.SetSelection(sp.Line, sp.Character, ep.Line, ep.Character);
            }
            catch
            {
            }
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
                    SyntaxTreeSources = runner.CreateGraph(Paths);
                    var results = runner.Run(SelectedPatterns);
                    CreateResultViewModels(results);
                }
                catch
                {
                    //@TODO User friendly error handling
                }
            });

            statusBar.Value = 0;
            Loading = false;
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            var viewItem = sender as TreeViewItem;

            var viewModel = viewItem?.DataContext as CheckResultViewModel;
            if (viewModel == null) return;

            var node = viewModel.Result.GetSyntaxNode();
            selectNodeInEditor(node, SyntaxTreeSources[node.SyntaxTree]);
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

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            var cm = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            var ws = (Workspace)cm.GetService<VisualStudioWorkspace>();
            Projects = ws.CurrentSolution.Projects.ToList();
            ProjectSelection.ItemsSource = Projects;
            ProjectSelection.SelectedIndex = 0;
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

    }
}