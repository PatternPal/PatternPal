#region

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using PatternPal.Extension.Commands;
using PatternPal.Extension.Model;
using PatternPal.Extension.Stores;
using PatternPal.Protos;
using PatternPal.StepByStep.Abstractions;

#endregion

namespace PatternPal.Extension.ViewModels
{
    public class StepByStepInstructionsViewModel : ViewModel
    {
        public ICommand NavigateHomeCommand { get; }

        public InstructionSet InstructionSet { get; }

        private LinkedListNode< IInstruction > _currentInstruction;

        public IInstructionState State = new InstructionState();

        public ObservableCollection< string > cbItems { get; set; } =
            new ObservableCollection< string >();

        public string SelectedcbItem { get; set; }

        public LinkedListNode< IInstruction > CurrentInstruction
        {
            get => _currentInstruction;
            set
            {
                _currentInstruction = value;
                OnPropertyChanged(nameof( CurrentInstruction ));
            }
        }

        private int _currentInstructionNumber;

        public int CurrentInstructionNumber
        {
            get => _currentInstructionNumber;
            set
            {
                _currentInstructionNumber = value;
                OnPropertyChanged(nameof( Title ));
            }
        }

        /// <summary>
        /// Title that is shown on the top of the screen. Contains the current instruction number out of the number of total instructions
        /// </summary>
        public override string Title => $"{InstructionSet.Name} {CurrentInstructionNumber}/{InstructionSet.NumberOfInstructions}";

        public StepByStepInstructionsViewModel(
            NavigationStore navigationStore,
            InstructionSet instructionSet)
        {
            NavigateHomeCommand = new NavigateCommand< HomeViewModel >(
                navigationStore,
                () => new HomeViewModel(navigationStore)
            );
            InstructionSet = instructionSet;

            //CurrentInstruction = new LinkedList< IInstruction >(InstructionSet.Instructions).First;
            CurrentInstructionNumber = 1;
        }
    }
}
