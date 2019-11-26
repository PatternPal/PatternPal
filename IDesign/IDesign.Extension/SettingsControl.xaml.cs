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

        /// <summary>
        ///     Initializes a new instance of the SettingsControl class.
        /// </summary>
        /// <param name="designPatterns"></param>
        public SettingsControl(List<DesignPatternViewModel> designPatterns, bool isActiveDoc)
        {
            InitializeComponent();

            if (isActiveDoc)
                radio1.IsChecked = isActiveDoc;
            else
                radio2.IsChecked = !isActiveDoc;

            DesignPatterns = designPatterns;
            listBox.DataContext = DesignPatterns;
        }
    }
}