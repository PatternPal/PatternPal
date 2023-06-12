#region

using System;
using System.Windows.Controls;
using System.Windows.Input;

using Community.VisualStudio.Toolkit;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;

using PatternPal.Extension.Grpc;
using PatternPal.Extension.ViewModels;
using PatternPal.Protos;

#endregion

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

        private void Control_OnMouseDoubleClick(
            object sender,
            MouseButtonEventArgs e)
        {
            Label label = sender as Label;
            if (!(label?.DataContext is CheckResultViewModel viewModel))
            {
                return;
            }

            MatchedNode matchedNode = viewModel.Result.MatchedNode;
            if (null == matchedNode)
            {
                return;
            }

            ThreadHelper.JoinableTaskFactory.Run(
                async delegate
                {
                    try
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        DocumentView documentView = await VS.Documents.OpenViaProjectAsync(matchedNode.Path);
                        if (documentView == null)
                        {
                            GrpcHelper.ShowErrorMessage("Failed to open file");
                            return;
                        }

                        ITextSnapshot snapshot = documentView.TextBuffer?.CurrentSnapshot;
                        if (snapshot == null)
                        {
                            GrpcHelper.ShowErrorMessage("Failed to get snapshot");
                            return;
                        }

                        documentView.TextView?.Selection.Select(
                            new SnapshotSpan(
                                snapshot,
                                matchedNode.Start,
                                matchedNode.Length),
                            false);
                    }
                    catch (Exception exception)
                    {
                        GrpcHelper.ShowErrorMessage(exception.Message);
                    }
                });
        }
    }
}
