using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using PatternPal.Extension.Commands;
using PatternPal.Extension.Stores;
using PatternPal.StepByStep;
using PatternPal.StepByStep.Abstractions;


namespace PatternPal.Extension.ViewModels
{
    public class StepByStepListViewModel : ViewModel
    {
        public override string Title => Resources.ExtensionUIResources.StepByStepTitle;

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateStepByStepInstructionsCommand { get; }
        public List<IInstructionSet> InstructionSetList { get; set; }
        public IInstructionSet SelectedInstructionSet { get; set; }

        public StepByStepListViewModel(NavigationStore navigationStore)
        {
            NavigateHomeCommand = new NavigateCommand<HomeViewModel>(navigationStore, () => new HomeViewModel(navigationStore));
            NavigateStepByStepInstructionsCommand =
                new NavigateCommand<StepByStepInstructionsViewModel>(navigationStore,
                    () => new StepByStepInstructionsViewModel(navigationStore, SelectedInstructionSet));
            InstructionSetList = InstructionSetsCreator.InstructionSets.ToList();
            SelectedInstructionSet = InstructionSetList?.FirstOrDefault();
        }
    }
}
