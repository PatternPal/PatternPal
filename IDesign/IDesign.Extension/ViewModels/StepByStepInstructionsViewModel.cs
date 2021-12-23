using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IDesign.Extension.Commands;
using IDesign.Extension.Stores;
using IDesign.StepByStep.Abstractions;
using IDesign.StepByStep.Models;

namespace IDesign.Extension.ViewModels
{
    public class StepByStepInstructionsViewModel : ViewModel
    {
        public IInstructionSet InstructionSet { get; }

        public Instruction CurrentInstruction => InstructionSet.Instructions.FirstOrDefault();
        public string CurrentInstructionText
        {
            get
            {
                var ins = CurrentInstruction;
                if (ins != null)
                {
                    return ins.InstructionText;
                }

                return "Instruction unavailable";
            }
        }

        public override string Title => InstructionSet.Name;

        public ICommand NavigateHomeCommand { get;  }

        public StepByStepInstructionsViewModel(NavigationStore navigationStore, IInstructionSet instructionSet)
        {
            NavigateHomeCommand = new NavigateCommand<HomeViewModel>(navigationStore, () => new HomeViewModel(navigationStore));
            InstructionSet = instructionSet;
        }
    }
}
