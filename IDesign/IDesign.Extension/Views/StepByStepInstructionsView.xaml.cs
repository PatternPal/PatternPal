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

namespace IDesign.Extension.Views
{
    /// <summary>
    /// Interaction logic for StepByStepInstructionsView.xaml
    /// </summary>
    public partial class StepByStepInstructionsView : UserControl
    {
        public StepByStepInstructionsView()
        {
            InitializeComponent();
        }

        public bool firstTime = true;

        private void CheckButton_OnClick(object sender, RoutedEventArgs e)
        {
            FeedbackMessage.Visibility = Visibility.Visible;
            CheckNextButton.Content = "Next";
            if (!firstTime)
            {
                InstructionText.Text = "Maak een klasse aan die de zojuist gemaakte 'behaviour' interface implementeert.";
                CheckNextButton.Content = "Check";
                FeedbackMessage.Visibility = Visibility.Hidden;
            }
            
            firstTime = false;
        }
    }
}
