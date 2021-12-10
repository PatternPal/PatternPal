using System.Windows;
using System.Windows.Controls;

namespace IDesign.Extension
{
    /// <summary>
    /// Interaction logic for HomeWindowControl.xaml
    /// </summary>
    public partial class HomeWindowControl : UserControl
    {
        public HomeWindowControl()
        {
            InitializeComponent();
        }

        private void HomeWindowControl_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ButtonsPanel.Orientation = e.NewSize.Width >= 570 ? Orientation.Horizontal : Orientation.Vertical;
        }
    }
}
