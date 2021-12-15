using System.Windows;
using System.Windows.Controls;

namespace IDesign.Extension.Views
{
    /// <summary>
    ///     Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
        }

        private void HomeView_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ButtonsPanel.Orientation = e.NewSize.Width >= 570 ? Orientation.Horizontal : Orientation.Vertical;
        }
    }
}
