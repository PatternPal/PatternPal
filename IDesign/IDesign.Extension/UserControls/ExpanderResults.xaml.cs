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
    ///     Interaction logic for ExpanderResults.xaml
    /// </summary>
    public partial class ExpanderResults : UserControl
    {
        public ExpanderResults()
        {
            InitializeComponent();
        }
    }
}
