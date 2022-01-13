using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using IDesign.Extension.Commands;
using IDesign.Extension.Model;
using IDesign.Extension.Stores;
using IDesign.StepByStep.Abstractions;

namespace IDesign.Extension.ViewModels
{
    public class StepByStepInstructionsViewModel : ViewModel
    {
        public ICommand NavigateHomeCommand { get; }

        public IInstructionSet InstructionSet { get; }

        private LinkedListNode<IInstruction> _currentInstruction;

        public IInstructionState State = new InstructionState();


        public LinkedListNode<IInstruction> CurrentInstruction
        {
            get => _currentInstruction;
            set
            {
                _currentInstruction = value;
                OnPropertyChanged(nameof(CurrentInstruction));
            }
        }

        private int _currentInstructionNumber;
        public int CurrentInstructionNumber
        {
            get => _currentInstructionNumber;
            set
            {
                _currentInstructionNumber = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        /// <summary>
        /// Title that is shown on the top of the screen. Contains the current instruction number out of the number of total instructions
        /// </summary>
        public override string Title => $"{InstructionSet.Name} {CurrentInstructionNumber}/{InstructionSet.Instructions.Count()}";

        public StepByStepInstructionsViewModel(NavigationStore navigationStore, IInstructionSet instructionSet)
        {
            NavigateHomeCommand = new NavigateCommand<HomeViewModel>(navigationStore, () => new HomeViewModel(navigationStore));
            InstructionSet = instructionSet;

            CurrentInstruction = new LinkedList<IInstruction>(InstructionSet.Instructions).First;
            CurrentInstructionNumber = 1;
        }
    }
}
