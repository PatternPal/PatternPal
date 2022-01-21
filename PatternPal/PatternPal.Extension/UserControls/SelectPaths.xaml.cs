using System.Windows;
using System.Windows.Controls;

namespace PatternPal.Extension.UserControls
{
    /// <summary>
    ///     Interaction logic for SelectPaths.xaml
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
