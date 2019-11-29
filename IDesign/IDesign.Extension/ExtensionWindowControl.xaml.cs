using EnvDTE;
using IDesign.Core;
using IDesign.Recognizers.Abstractions;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using IDesign.Extension.ViewModels;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.TextManager.Interop;
using Task = System.Threading.Tasks.Task;
using Thread = System.Threading.Thread;
using Window = System.Windows.Window;

namespace IDesign.Extension
{
    /// <summary>
    ///     Interaction logic for ExtensionWindowControl.
    /// </summary>
    public partial class ExtensionWindowControl : UserControl
    {
        public bool IsActiveDoc { get; set; }
        public List<string> Paths { get; set; }
        public bool Loading { get; set; }
        public DTE Dte { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtensionWindowControl" /> class.
        /// </summary>
        public ExtensionWindowControl()
        {
            InitializeComponent();
            AddViewModels();
            IsActiveDoc = true;
            Loading = false;
            Dispatcher.VerifyAccess();
            Dte = Package.GetGlobalService(typeof(SDTE)) as DTE;
        }

        /// <summary>
        ///     Adds all the existing designpatterns in a list.
        /// </summary>
        private void AddViewModels()
        {
            var viewModels = new List<DesignPatternViewModel>();

            foreach (var pattern in RecognizerRunner.designPatterns)
            {
                viewModels.Add(new DesignPatternViewModel(pattern.Name, pattern));
            }

            SettingsControl.DesignPatterns = viewModels;
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

                foreach (var suggestion in result.Result.GetSuggestions())
                {
                    resultViewModel.Suggestions.Add(new SuggestionViewModel(suggestion, result.EntityNode));
                }
            }
            //Here you signal the UI thread to execute the action:
            this.Dispatcher?.BeginInvoke(new Action(() =>
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
            FileManager manager = new FileManager();
            if (Dte.Solution.Count > 0)
                Paths = manager.GetAllCsFilesFromDirectory(Path.GetDirectoryName(Dte.Solution.FullName));
        }

        private void ChoosePath()
        {
            if (SettingsControl.radio1.IsChecked != null && SettingsControl.radio1.IsChecked.Value)
                GetCurrentPath();
            else
                GetAllPaths();
        }

        private void selectNodeInEditor(SyntaxNode n, string file)
        {
            try
            {
                var cm = (IComponentModel) Package.GetGlobalService(typeof(SComponentModel));
                var tm = (IVsTextManager) Package.GetGlobalService(typeof(SVsTextManager));
                var ws = (Workspace) cm.GetService<VisualStudioWorkspace>();
                var did = ws.CurrentSolution.GetDocumentIdsWithFilePath(file);
                ws.OpenDocument(did.FirstOrDefault());
                tm.GetActiveView(1, null, out var av);
                var sp = n.GetLocation().GetMappedLineSpan().StartLinePosition;
                var ep = n.GetLocation().GetMappedLineSpan().EndLinePosition;
                av.SetSelection(sp.Line, sp.Character, ep.Line, ep.Character);
            }
            catch
            {
                return;
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

            if (Loading || SettingsControl.DesignPatterns.Count == 0 || Paths.Count == 0)
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
                var results = runner.Run(Paths, SettingsControl.DesignPatterns.Where(x => x.IsChecked).Select(x => x.Pattern).ToList());
                CreateResultViewModels(results);
            });

            statusBar.Value = 0;
            Loading = false;
        }

        private void ResultsView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var s = sender;
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            var viewItem = sender as TreeViewItem;
            if (viewItem == null) return;
            var viewModel = viewItem.DataContext as SuggestionViewModel;
            if (viewModel == null) return;

            selectNodeInEditor(viewModel.Suggestion.GetSyntaxNode(), viewModel.Node.GetSourceFile());
        }
    }
}