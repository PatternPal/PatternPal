using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for PatternCheckbox.xaml
    /// </summary>
    public partial class PatternCheckbox : UserControl
    {
        public PatternCheckbox()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var url = (e.OriginalSource as Button).DataContext.ToString();

            if (url == null || url == string.Empty)
                return;

            IVsWindowFrame ppFrame;

            var service = Package.GetGlobalService(typeof(IVsWebBrowsingService)) as IVsWebBrowsingService;

            service.Navigate(url, 0, out ppFrame);
        }
    }
}
