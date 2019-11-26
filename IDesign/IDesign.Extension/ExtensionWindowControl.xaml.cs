namespace IDesign.Extension
{
    using EnvDTE;
    using IDesign.Core;
    using Microsoft.VisualStudio.Shell.Interop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ExtensionWindowControl.
    /// </summary>
    public partial class ExtensionWindowControl : UserControl
    {
        public bool IsActiveDoc { get; set; }
        public List<DesignPatternViewModel> DesignPatternViewModels { get; set; }
        public List<string> Paths { get; set; }
        public bool Loading { get; set; }
        public DTE Dte { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionWindowControl"/> class.
        /// </summary>
        public ExtensionWindowControl()
        {
            this.InitializeComponent();
            AddViewModels();
            IsActiveDoc = true;
            Loading = false;
            Dispatcher.VerifyAccess();
            Dte = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SDTE)) as DTE;
        }

        /// <summary>
        /// Adds all the existing designpatterns in a list.
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
        /// Gets current active document path.
        /// </summary>
        private void GetCurrentPath()
        {
            Paths = new List<string>();
            Paths.Add(Dte.ActiveDocument.FullName);
        }

        /// <summary>
        /// Gets all paths in the solution.
        /// </summary>
        private void GetAllPaths()
        {
            Paths = new List<string>();
            FileManager manager = new FileManager();
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
        /// Handles click on the analyse_button by displaying the tool window.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private async void Analyse_Button(object sender, RoutedEventArgs e)
        {
            ChoosePath();

            if (Loading || DesignPatternViewModels.Count == 0 || Paths.Count == 0)
                return;

            RecognizerRunner runner = new RecognizerRunner();
            runner.Run(Paths, DesignPatternViewModels.Where(x => x.IsChecked).Select(x => x.Pattern).ToList());

            listView.ItemsSource = DesignPatternViewModels.Where(x => x.IsChecked).Select(x => x.Pattern);

            Loading = true;
            statusBar.Value = 0;
            var progress = new Progress<int>(value => statusBar.Value = value);

            await Task.Run(() =>
            {
                for (int i = 0; i <= 100; i++)
                {
                    ((IProgress<int>)progress).Report(i);
                    System.Threading.Thread.Sleep(100);
                }
            });

            statusBar.Value = 0;
            Loading = false;
        }

        /// <summary>
        /// Handles click on the analyse_button by displaying the settings window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Button(object sender, RoutedEventArgs e)
        {
            SettingsControl settingsWindow = new SettingsControl(DesignPatternViewModels, IsActiveDoc);

            System.Windows.Window window = new System.Windows.Window
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