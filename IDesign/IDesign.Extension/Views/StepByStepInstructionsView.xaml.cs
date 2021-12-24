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
using IDesign.Extension.ViewModels;

namespace IDesign.Extension.Views
{
    /// <summary>
    /// Interaction logic for StepByStepInstructionsView.xaml
    /// </summary>
    public partial class StepByStepInstructionsView : UserControl
    {
        private StepByStepInstructionsViewModel _viewModel;

        public StepByStepInstructionsView()
        {
            InitializeComponent();
            DataContextChanged += delegate
            {
                // only bind events when dataContext is set, not unset
                if (this.DataContext == null)
                {
                    return;
                }
                _viewModel = (StepByStepInstructionsViewModel)(this.DataContext);
            };
        }

        /// <summary>
        /// Activated after clicking on the previous instruction button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviousInstructionButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_viewModel.CurrentInstruction.Previous != null)
            {
                _viewModel.CurrentInstruction = _viewModel.CurrentInstruction.Previous;
                _viewModel.CurrentInstructionNumber--;
            }
        }

        /// <summary>
        /// Activated after clicking on the next instruction button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextInstructionButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_viewModel.CurrentInstruction.Next != null)
            {
                _viewModel.CurrentInstruction = _viewModel.CurrentInstruction.Next;
                _viewModel.CurrentInstructionNumber++;
            }
                

        }
    }
}
