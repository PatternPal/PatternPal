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
    public partial class Collabsable : UserControl
    {
        public Collabsable()
        {
            InitializeComponent();
        }

        public string TitleText
        {
            get { return (string)GetValue(TitleTextProperty); }
            set { SetValue(TitleTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyVarColor1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleTextProperty =
            DependencyProperty.Register("TitleText", typeof(string), typeof(Collabsable), new UIPropertyMetadata(null));

    }
}
