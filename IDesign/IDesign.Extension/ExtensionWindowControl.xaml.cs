namespace IDesign.Extension
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
    /// Interaction logic for ExtensionWindowControl.
    /// </summary>
    public partial class ExtensionWindowControl : UserControl
    {
        public List<DesignPattern> DesignPatterns { get; set; }
        public bool Loading { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionWindowControl"/> class.
        /// </summary>
        public ExtensionWindowControl()
        {
            this.InitializeComponent();
            AddPatterns();
            Loading = false;
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
                    Thread.Sleep(100);
                }
            });

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