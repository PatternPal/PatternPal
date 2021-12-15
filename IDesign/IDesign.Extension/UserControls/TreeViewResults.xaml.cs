using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using IDesign.Extension.ViewModels;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;

namespace IDesign.Extension.UserControls
{
    /// <summary>
    ///     Interaction logic for TreeViewResults.xaml
    /// </summary>
    public partial class TreeViewResults : UserControl
    {
        public TreeViewResults()
        {
            InitializeComponent();
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            var viewItem = sender as TreeViewItem;

            if (!(viewItem?.DataContext is CheckResultViewModel viewModel))
            {
                return;
            }

            var element = viewModel.Result.GetElement();
            if (element == null)
            {
                return;
            }

            var node = element.GetSyntaxNode();
            if (node == null)
            {
                return;
            }

            SelectNodeInEditor(node, element.GetRoot().GetSource());
        }

        /// <summary>
        ///     Clicking on the node brings you to the right document.
        /// </summary>
        private void SelectNodeInEditor(SyntaxNode node, string file)
        {
            try
            {
                var tm = (IVsTextManager)Package.GetGlobalService(typeof(SVsTextManager));
                var cm = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
                var ws = (Workspace)cm.GetService<VisualStudioWorkspace>();
                var did = ws.CurrentSolution.GetDocumentIdsWithFilePath(file);
                ws.OpenDocument(did.FirstOrDefault());
                tm.GetActiveView(1, null, out var av);
                var sp = node.GetLocation().GetMappedLineSpan().StartLinePosition;
                var ep = node.GetLocation().GetMappedLineSpan().EndLinePosition;
                av.SetSelection(sp.Line, sp.Character, ep.Line, ep.Character);
            }
            catch (Exception e)
            {
                _ = e.Message;
            }
        }
    }
}
