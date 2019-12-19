using IDesign.Extension.ViewModels;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
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

namespace IDesign.Extension.UserControls
{
    /// <summary>
    /// Interaction logic for TreeViewResults.xaml
    /// </summary>
    public partial class TreeViewResults : UserControl
    {
        public Dictionary<SyntaxTree, string> SyntaxTreeSources { get; set; }

        public TreeViewResults()
        {
            InitializeComponent();
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            var viewItem = sender as TreeViewItem;

            var viewModel = viewItem?.DataContext as CheckResultViewModel;
            if (viewModel == null) return;

            var node = viewModel.Result.GetSyntaxNode();
            SelectNodeInEditor(node, SyntaxTreeSources[node.SyntaxTree]);
        }

        /// <summary>
        ///     Clicking on the node brings you to the right document.
        /// </summary>
        private void SelectNodeInEditor(SyntaxNode n, string file)
        {
            try
            {
                var tm = (IVsTextManager)Package.GetGlobalService(typeof(SVsTextManager));
                var cm = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
                var ws = (Workspace)cm.GetService<VisualStudioWorkspace>();
                var did = ws.CurrentSolution.GetDocumentIdsWithFilePath(file);
                ws.OpenDocument(did.FirstOrDefault());
                tm.GetActiveView(1, null, out var av);
                var sp = n.GetLocation().GetMappedLineSpan().StartLinePosition;
                var ep = n.GetLocation().GetMappedLineSpan().EndLinePosition;
                av.SetSelection(sp.Line, sp.Character, ep.Line, ep.Character);
            }
            catch (Exception e)
            {
                _ = e.Message;
            }
        }
    }
}
