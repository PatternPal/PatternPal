#region

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using PatternPal.Extension.ViewModels;

#endregion

namespace PatternPal.Extension.UserControls
{
    /// <summary>
    ///     Interaction logic for PatternCheckbox.xaml
    /// </summary>
    public partial class PatternCheckbox
    {
        public PatternCheckbox()
        {
            InitializeComponent();
        }

        private void CheckBox_Changed(
            object sender,
            RoutedEventArgs e)
        {
            List< DesignPatternViewModel > designPatternViewModels = listBox.Items.OfType< DesignPatternViewModel >().ToList();

            CheckBox checkbox = ((Parent as Grid)?.Children[ 1 ] as Border)?.Child as CheckBox;
            if (null == checkbox)
            {
                return;
            }

            if (designPatternViewModels.All(x => x.IsChecked))
            {
                checkbox.IsChecked = true;
            }
            else
            {
                if (designPatternViewModels.All(x => !x.IsChecked))
                {
                    checkbox.IsChecked = false;
                }
            }
        }
    }
}
