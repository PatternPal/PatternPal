namespace IDesign.Extension
{
    using EnvDTE;
    using Microsoft.VisualStudio.Shell.Interop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
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
        public List<DesignPattern> DesignPatterns { get; set; }
        public List<string> Paths { get; set; }
        public bool Loading { get; set; }
        public DTE Dte { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionWindowControl"/> class.
        /// </summary>
        public ExtensionWindowControl()
        {
            this.InitializeComponent();
            AddPatterns();
            Loading = false;
            Dispatcher.VerifyAccess();
            Dte = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SDTE)) as DTE;
        }

        /// <summary>
        /// Adds all the existing designpatterns in a list.
        /// </summary>
        private void AddPatterns()
        {
            DesignPatterns = new List<DesignPattern>
            {
                new DesignPattern("Singleton"),
                new DesignPattern("State"),
                new DesignPattern("Strategy"),
                new DesignPattern("Factory"),
                new DesignPattern("Decorator"),
                new DesignPattern("Adapter")
            };
        }

        /// <summary>
        /// Gets current active document path.
        /// </summary>
        private void GetCurrentPath()
        {
            Paths = new List<string>();
            Paths.Add(Dte.ActiveDocument.Path);
        }

        /// <summary>
        /// Gets all paths in the solution.
        /// </summary>
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

            Paths = new List<string>();

            foreach (Project project in Dte.Solution.Projects)
            {
                GetPathsRecursive(project.ProjectItems);
            }
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
            if (Loading)
                return;

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

            GetPaths();
            listView.ItemsSource = DesignPatterns.Where(x => x.IsChecked);

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
            SettingsControl settingsWindow = new SettingsControl(DesignPatterns);

            System.Windows.Window window = new System.Windows.Window
            {
                Title = "Settings",
                Content = settingsWindow,
                SizeToContent = SizeToContent.WidthAndHeight
            };
            window.ShowDialog();

            DesignPatterns = settingsWindow.DesignPatterns;
        }
    }
}