#region

using System;
using System.Collections.Generic;
using System.Windows.Input;

using PatternPal.Extension.Grpc;
using PatternPal.Extension.Stores;
using PatternPal.Protos;

#endregion

namespace PatternPal.Extension.ViewModels
{
    /// <summary>
    /// The view model for a Step by Step implementation.
    /// </summary>
    public class StepByStepInstructionsViewModel : ViewModel
    {
        private readonly IList< Instruction > _instructions;
        private readonly InstructionSet _instructionSet;

        /// <summary>
        /// Command to navigate home.
        /// </summary>
        public ICommand NavigateHomeCommand { get; }

        /// <summary>
        /// Files to check for each step.
        /// </summary>
        public List< string > FilePaths { get; set; }

        /// <summary>
        /// The mode in which Step by Step operates.
        /// </summary>
        public StepByStepModes Mode { get; }

        /// <summary>
        /// The <see cref="Protos.Recognizer"/> which is being used.
        /// </summary>
        public Recognizer Recognizer { get; }

        /// <summary>
        /// The current <see cref="Instruction"/>.
        /// </summary>
        public Instruction CurrentInstruction => _instructions[ _currentInstructionNumber - 1 ];

        private int _currentInstructionNumber;

        /// <summary>
        /// The index of the current <see cref="Instruction"/>.
        /// </summary>
        public int CurrentInstructionNumber
        {
            get => _currentInstructionNumber;
            private set
            {
                _currentInstructionNumber = value;
                OnPropertyChanged(nameof( Title ));
            }
        }

        /// <summary>
        /// Tries to select the previous instruction.
        /// </summary>
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

        /// <summary>
        /// Tries to select the next instruction.
        /// </summary>
        public bool TrySelectNextInstruction()
        {
            if (_currentInstructionNumber == _instructionSet.NumberOfInstructions)
            {
                return false;
            }

            CurrentInstructionNumber++;
            if (_currentInstructionNumber >= _instructions.Count)
            {
                try
                {
                    Instruction value = _instructions[_currentInstructionNumber-1];
                }
                catch
                {
                    _instructions.Add(
                        GetInstructionById(
                            CurrentInstructionNumber - 1,
                            Recognizer));
                }
            }
            OnPropertyChanged(nameof( CurrentInstruction ));
            return true;
        }

        /// <summary>
        /// Whether this has <see cref="Instruction"/> is not the first <see cref="Instruction"/>.
        /// </summary>
        public bool HasPreviousInstruction => _currentInstructionNumber > 1;

        /// <summary>
        /// Whether this has <see cref="Instruction"/> is not the last <see cref="Instruction"/>.
        /// </summary>
        public bool HasNextInstruction => _currentInstructionNumber < _instructionSet.NumberOfInstructions;

        /// <summary>
        /// Title that is shown on the top of the screen. Contains the current instruction number out of the number of total instructions
        /// </summary>
        public override string Title => $"{_instructionSet.Name} {CurrentInstructionNumber}/{_instructionSet.NumberOfInstructions}";

        /// <summary>
        /// Creates a new instance of the <see cref="StepByStepInstructionsViewModel"/> class.
        /// </summary>
        /// <param name="navigationStore">The <see cref="NavigationStore"/> to use for navigation commands.</param>
        /// <param name="recognizer">The <see cref="Protos.Recognizer"/> to use.</param>
        /// <param name="mode">The <see cref="StepByStepModes"/> in which to run Step by Step.</param>
        /// <param name="filePaths">The files to check.</param>
        public StepByStepInstructionsViewModel(
            NavigationStore navigationStore,
            Recognizer recognizer,
            StepByStepModes mode,
            List< string > filePaths)
        {
            NavigateHomeCommand = new NavigateCommand< HomeViewModel >(
                navigationStore,
                () => new HomeViewModel(navigationStore));
            Recognizer = recognizer;

            GetInstructionSetResponse instructionSetResponse =
                GrpcHelper.StepByStepClient.GetInstructionSet(
                    new GetInstructionSetRequest
                    {
                        Recognizer = recognizer
                    });
            _instructionSet = instructionSetResponse.SelectedInstructionset;

            _instructions = new List< Instruction >((int)_instructionSet.NumberOfInstructions)
                            {
                                GetInstructionById(
                                    CurrentInstructionNumber,
                                    recognizer),
                            };
            CurrentInstructionNumber = 1;

            Mode = mode;
            FilePaths = filePaths;
        }

        /// <summary>
        /// Get an <see cref="Instruction"/> from an id.
        /// </summary>
        /// <param name="instructionId">The id.</param>
        /// <param name="recognizer">The <see cref="Protos.Recognizer"/> from which to get the instruction.</param>
        private Instruction GetInstructionById(
            int instructionId,
            Recognizer recognizer)
        {
            GetInstructionByIdResponse response =
                GrpcHelper.StepByStepClient.GetInstructionById(
                    new GetInstructionByIdRequest
                    {
                        InstructionNumber = (uint)instructionId,
                        Recognizers = recognizer
                    }
                );
            return response.Instruction;
        }
    }
}
