using System.Collections.Generic;
using System.Windows.Controls;

namespace IDesign.Extension
{
    /// <summary>
    ///     Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public List<DesignPatternViewModel> DesignPatterns { get; set; }
        public bool IsActiveDoc { get; set; }

        /// <summary>
        ///     Initializes a new instance of the SettingsControl class.
        /// </summary>
        /// <param name="designPatterns"></param>
        public SettingsControl()
        {
            InitializeComponent();
            this.Loaded += SettingsControl_Loaded;
        }

        private void SettingsControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (IsActiveDoc)
                radio1.IsChecked = IsActiveDoc;
            else
                radio2.IsChecked = !IsActiveDoc;

            listBox.DataContext = DesignPatterns;
        }
    }
}