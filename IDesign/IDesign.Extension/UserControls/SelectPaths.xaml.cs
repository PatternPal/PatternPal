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

namespace IDesign.Extension.UserControls
{
    /// <summary>
    /// Interaction logic for SelectPaths.xaml
    /// </summary>
    public partial class SelectPaths : UserControl
    {
        public SelectPaths()
        {
            InitializeComponent();
        }

        private void ActiveDocument_Checked(object sender, RoutedEventArgs e)
        {
            RadioGrid.RowDefinitions[1].Height = new GridLength(0);
        }

        private void Project_Checked(object sender, RoutedEventArgs e)
        {
            RadioGrid.RowDefinitions[1].Height = new GridLength(30);
        }
    }
}
