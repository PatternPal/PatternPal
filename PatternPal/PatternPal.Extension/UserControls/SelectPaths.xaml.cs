#region

using System.Windows;
using System.Windows.Controls;

#endregion

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

        private void ActiveDocument_Checked(
            object sender,
            RoutedEventArgs e)
        {
            if (null != ProjectSelection)
            {
                ProjectSelection.Visibility = Visibility.Collapsed;
            }
        }

        private void Project_Checked(
            object sender,
            RoutedEventArgs e)
        {
            if (null != ProjectSelection)
            {
                ProjectSelection.Visibility = Visibility.Visible;
            }
        }
    }
}
