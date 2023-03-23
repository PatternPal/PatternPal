#region

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using PatternPal.Extension.Commands;
using PatternPal.Extension.Grpc;
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

        public IInstructionState State = new InstructionState();

        public ObservableCollection< string > cbItems { get; set; } =
            new ObservableCollection< string >();

        public string SelectedcbItem { get; set; }

        private IList< Instruction > _instructions;

        public Instruction CurrentInstruction
        {
            get => _instructions[ _currentInstructionNumber - 1 ];
            //set
            //{
            //    _currentInstruction = value;
            //    OnPropertyChanged(nameof( CurrentInstruction ));
            //}
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

        public bool TrySelectPreviousInstruction()
        {
            if (_currentInstructionNumber == 1)
            {
                return false;
            }

            CurrentInstructionNumber--;
            return true;
        }

        public bool TrySelectNextInstruction()
        {
            if (_currentInstructionNumber == InstructionSet.NumberOfInstructions)
            {
                return false;
            }

            CurrentInstructionNumber++;
            if (null == _instructions[ _currentInstructionNumber ])
            {
                _instructions.Add(GetInstructionById(CurrentInstructionNumber));
            }

            return true;
        }

        public bool HasPreviousInstruction => _currentInstructionNumber > 1;
        public bool HasNextInstruction => _currentInstructionNumber < InstructionSet.NumberOfInstructions;

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
                () => new HomeViewModel(navigationStore));
            InstructionSet = instructionSet;

            _instructions = new List< Instruction >((int)InstructionSet.NumberOfInstructions)
                            {
                                GetInstructionById(CurrentInstructionNumber),
                            };
            CurrentInstructionNumber = 1;
        }

        private Instruction GetInstructionById(
            int instructionId)
        {
            Protos.PatternPal.PatternPalClient client = new Protos.PatternPal.PatternPalClient(GrpcChannelHelper.Channel);
            GetInstructionByIdResponse response = client.GetInstructionById(
                new GetInstructionByIdRequest
                {
                    InstructionSetName = InstructionSet.Name,
                    InstructionId = instructionId,
                });

            return response.Instruction;
        }
    }
}
