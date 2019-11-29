﻿using EnvDTE;
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
using IDesign.Extension.ViewModels;
using Task = System.Threading.Tasks.Task;
using Thread = System.Threading.Thread;
using Window = System.Windows.Window;
using Microsoft.VisualStudio.ProjectSystem;

namespace IDesign.Extension
{
    /// <summary>
    ///     Interaction logic for ExtensionWindowControl.
    /// </summary>
    public partial class ExtensionWindowControl : UserControl
    {
        public bool IsActiveDoc { get; set; }
        public List<DesignPatternViewModel> ViewModels { get; set; }
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
            ViewModels = new List<DesignPatternViewModel>();

            foreach (var pattern in RecognizerRunner.designPatterns)
            {
                ViewModels.Add(new DesignPatternViewModel(pattern.Name, pattern));
            }

            listBox.DataContext = ViewModels;
            Grid.RowDefinitions[0].Height = new GridLength(ViewModels.Count * 20);
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
                    resultViewModel.Suggestions.Add(new SuggestionViewModel(suggestion));
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
            if (radio1.IsChecked != null)
                GetCurrentPath();
            else
                GetAllPaths();
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
            List<DesignPattern> SelectedPatterns = ViewModels.Where(x => x.IsChecked).Select(x => x.Pattern).ToList();

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
                var results = runner.Run(Paths, SelectedPatterns);
                CreateResultViewModels(results);
            });

            statusBar.Value = 0;
            Loading = false;
        }

    }
}