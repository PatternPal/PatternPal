using IDesign.Core;
using System.Collections.Generic;
using System.Windows.Controls;

namespace IDesign.Extension
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public List<DesignPattern> DesignPatterns { get; set; }

        /// <summary>
        /// Initializes a new instance of the SettingsControl class.
        /// </summary>
        /// <param name="designPatterns"></param>
        public SettingsControl(List<DesignPattern> designPatterns)
        {
            InitializeComponent();
            DesignPatterns = designPatterns;
            listBox.DataContext = DesignPatterns;
        }
    }
}
