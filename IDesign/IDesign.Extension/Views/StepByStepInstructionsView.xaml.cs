using System;
using System.Windows;
using System.Windows.Controls;

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
            InitializeViewModelAndButtons();
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
                CheckIfNextPreviousButtonsAvailable();
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
                CheckIfNextPreviousButtonsAvailable();
            }
        }

        /// <summary>
        /// Checks if the next and previous instruction buttons should be visible/hidden
        /// </summary>
        private void CheckIfNextPreviousButtonsAvailable()
        {
            NextInstructionButton.Visibility =
                _viewModel.CurrentInstruction.Next != null ? Visibility.Visible : Visibility.Hidden;
            PreviousInstructionButton.Visibility = _viewModel.CurrentInstruction.Previous != null
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void InitializeViewModelAndButtons()
        {
            DataContextChanged += delegate
            {
                // only bind events when dataContext is set, not unset
                if (this.DataContext == null)
                {
                    return;
                }

                _viewModel = (StepByStepInstructionsViewModel)(this.DataContext);

                CheckIfNextPreviousButtonsAvailable();

            };
        }

        private void CheckImplementationButton_OnClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
