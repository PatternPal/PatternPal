using IDesign.Extension.ViewModels;
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
            var wikiPage = (e.OriginalSource as Button).DataContext.ToString();

            if (wikiPage == null || wikiPage == string.Empty)
                return;

            IVsWindowFrame ppFrame;

            var service = Package.GetGlobalService(typeof(IVsWebBrowsingService)) as IVsWebBrowsingService;

            service.Navigate("https://en.wikipedia.org/wiki/" + wikiPage, 0, out ppFrame);
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            var designPatternViewModels = listBox.Items.OfType<DesignPatternViewModel>().ToList();

            if (designPatternViewModels.All(x => x.IsChecked))
            {
                (((this.Parent as Grid).Children[1] as Border).Child as CheckBox).IsChecked = true;
            }
            else if (designPatternViewModels.All(x => !x.IsChecked))
            {
                (((this.Parent as Grid).Children[1] as Border).Child as CheckBox).IsChecked = false;
            }
        }
    }
}
