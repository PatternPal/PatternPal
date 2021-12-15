using System.Linq;
using System.Windows;
using System.Windows.Controls;
using IDesign.Extension.ViewModels;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace IDesign.Extension.UserControls
{
    /// <summary>
    ///     Interaction logic for PatternCheckbox.xaml
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
            {
                return;
            }

            var service = Package.GetGlobalService(typeof(IVsWebBrowsingService)) as IVsWebBrowsingService;
            service.Navigate("https://en.wikipedia.org/wiki/" + wikiPage, 0, out _);
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            var designPatternViewModels = listBox.Items.OfType<DesignPatternViewModel>().ToList();

            if (designPatternViewModels.All(x => x.IsChecked))
            {
                (((Parent as Grid).Children[2] as Border).Child as CheckBox).IsChecked = true;
            }
            else if (designPatternViewModels.All(x => !x.IsChecked))
            {
                (((Parent as Grid).Children[2] as Border).Child as CheckBox).IsChecked = false;
            }
        }
    }
}
