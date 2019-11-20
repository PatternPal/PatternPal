namespace AnalyzerPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for PluginWindowControl.
    /// </summary>
    public partial class PluginWindowControl : UserControl
    {
        public List<DesignPattern> DesignPatterns { get; set; }
        public bool Loading { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginWindowControl"/> class.
        /// </summary>
        public PluginWindowControl()
        {
            this.InitializeComponent();
            AddPatterns();
            Loading = false;
        }

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
        /// Handles click on the button by displaying a message box.
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
                    Thread.Sleep(100);
                }
            });

            listView.ItemsSource = DesignPatterns.Where(x => x.IsChecked);

            statusBar.Value = 0;
            Loading = false;
        }

        private void Settings_Button(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(DesignPatterns);
            
            Window window = new Window
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