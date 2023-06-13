#region

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using PatternPal.Extension.Commands;
using PatternPal.Extension.Grpc;
using PatternPal.Extension.Stores;
using PatternPal.Protos;

#endregion

namespace PatternPal.Extension.ViewModels
{
    // TODO comment
    public class StepByStepInstructionsViewModel : ViewModel
    {
        public ICommand NavigateHomeCommand { get; }

        public List<string> FilePaths { get; set; }

        public StepByStepModes Mode { get; }

        public Recognizer Recognizer { get; }

        public ObservableCollection< string > cbItems { get; set; } =
            new ObservableCollection< string >();

        public string SelectedcbItem { get; set; }

        public InstructionSet InstructionSet { get; set; }

        private IList< Instruction > _instructions;

        public Instruction CurrentInstruction => _instructions[ _currentInstructionNumber - 1 ];


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
            OnPropertyChanged(nameof( CurrentInstruction ));
            return true;
        }

        public bool TrySelectNextInstruction()
        {
            if (_currentInstructionNumber == InstructionSet.NumberOfInstructions)
            {
                return false;
            }

            CurrentInstructionNumber++;
            if (_currentInstructionNumber >= _instructions.Count
                || null == _instructions[ _currentInstructionNumber ])
            {
                _instructions.Add(GetInstructionById(CurrentInstructionNumber - 1, Recognizer));
            }

            OnPropertyChanged(nameof( CurrentInstruction ));
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
            Recognizer recognizer,
            StepByStepModes mode,
            List<string> filePaths)
        {
            NavigateHomeCommand = new NavigateCommand<HomeViewModel>(
                navigationStore,
                () => new HomeViewModel(navigationStore));
            Recognizer = recognizer;

            GetInstructionSetResponse instructionSetResponse =
                GrpcHelper.StepByStepClient.GetInstructionSet(new GetInstructionSetRequest { Recognizer = recognizer});
            InstructionSet = instructionSetResponse.SelectedInstructionset;
            

            _instructions = new List<Instruction>((int)InstructionSet.NumberOfInstructions)
            {
                GetInstructionById(CurrentInstructionNumber, recognizer),
            };
            CurrentInstructionNumber = 1;

            Mode = mode;
            FilePaths = filePaths;
        }

        private Instruction GetInstructionById(
            int instructionId,
            Recognizer recognizer)
        {
            GetInstructionByIdResponse response =
                GrpcHelper.StepByStepClient.GetInstructionById(new GetInstructionByIdRequest
                {
                    InstructionNumber = (uint)instructionId,
                    Recognizers = recognizer
                }
                );
            return response.Instruction;
        }
    }
}
