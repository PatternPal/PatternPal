using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using PatternPal.Extension.ViewModels;
using Microsoft.CodeAnalysis;

namespace PatternPal.Extension.UserControls
{
    /// <summary>
    ///     Interaction logic for ExpanderResults.xaml
    /// </summary>
    public partial class ExpanderResults : UserControl
    {
        public ExpanderResults()
        {
            InitializeComponent();
        }

        //TODO: Handle clicking on a node to bring the user to the right document
        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem viewItem = sender as TreeViewItem;

            if (!(viewItem?.DataContext is CheckResultViewModel viewModel))
            {
                return;
            }

            //var element = viewModel.Result.GetElement();
            //if (element == null)
            //{
            //    return;
            //}

            //var node = element.GetSyntaxNode();
            //if (node == null)
            //{
            //    return;
            //}

            //SelectNodeInEditor(node, element.GetRoot().GetSource());
        }
    }
}
