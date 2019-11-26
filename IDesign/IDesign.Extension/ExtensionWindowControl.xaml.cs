using EnvDTE;
using IDesign.Core;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
<<<<<<< HEAD
=======
using EnvDTE;
using IDesign.Core;
using IDesign.Recognizers.Abstractions;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
>>>>>>> develop
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
        public List<DesignPatternViewModel> DesignPatternViewModels { get; set; }
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
            DesignPatternViewModels = new List<DesignPatternViewModel>();

            foreach (DesignPattern pattern in RecognizerRunner.designPatterns)
            {
                DesignPatternViewModels.Add(new DesignPatternViewModel(pattern.Name, pattern));
            }
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
<<<<<<< HEAD
        private void GetPaths()
        {
            void GetPathsRecursive(ProjectItems items)
            {
                foreach (ProjectItem item in items)
                {
                    if (item.ProjectItems.Count > 0)
                        GetPathsRecursive(item.ProjectItems);

                    if (item.Name.EndsWith(".cs"))
                        Paths.Add((string)item.Properties.Item("FullPath").Value);
                }
            }

=======
        private void GetAllPaths()
        {   
>>>>>>> develop
            Paths = new List<string>();
            FileManager manager = new FileManager();
            if (Dte.Solution.Count > 0)
                Paths = manager.GetAllCsFilesFromDirectory(Path.GetDirectoryName(Dte.Solution.FullName));
        }

        private void ChoosePath()
        {
            if (IsActiveDoc)
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

            if (Loading || DesignPatternViewModels.Count == 0 || Paths.Count == 0)
                return;

            RecognizerRunner runner = new RecognizerRunner();
            List<IResult> results = runner.Run(Paths, DesignPatternViewModels.Where(x => x.IsChecked).Select(x => x.Pattern).ToList());

            listView.ItemsSource = results;

            Loading = true;
            statusBar.Value = 0;
            var progress = new Progress<int>(value => statusBar.Value = value);

            await Task.Run(() =>
            {
                for (var i = 0; i <= 100; i++)
                {
                    ((IProgress<int>)progress).Report(i);
                    Thread.Sleep(100);
                }
            });

            statusBar.Value = 0;
            Loading = false;
        }

        /// <summary>
        ///     Handles click on the analyse_button by displaying the settings window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Button(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsControl(DesignPatternViewModels, IsActiveDoc);

            var window = new Window
            {
                Title = "Settings",
                Content = settingsWindow,
                SizeToContent = SizeToContent.WidthAndHeight
            };
            window.ShowDialog();

            IsActiveDoc = (bool) settingsWindow.radio1.IsChecked;
            DesignPatternViewModels = settingsWindow.DesignPatterns;
        }
    }
}