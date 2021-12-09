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
            ButtonsPanel.Orientation = e.NewSize.Height >= e.NewSize.Width ? Orientation.Vertical : Orientation.Horizontal;
        }
    }
}
