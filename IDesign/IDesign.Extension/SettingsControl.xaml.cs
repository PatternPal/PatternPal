using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IDesign.Extension
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public List<DesignPattern> DesignPatterns { get; set; }

        public SettingsControl(List<DesignPattern> designPatterns)
        {
            InitializeComponent();
            DesignPatterns = designPatterns;
            listBox.DataContext = DesignPatterns;
        }

        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            var pattern = sender as DesignPattern;
            pattern.IsChecked = false;
        }
    }
}
