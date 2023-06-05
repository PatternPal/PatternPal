#region

using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using PatternPal.Extension.Commands;
using PatternPal.Extension.Grpc;
using PatternPal.Extension.Resources;
using PatternPal.Extension.Stores;
using PatternPal.Protos;

#endregion

namespace PatternPal.Extension.ViewModels
{
    public class StepByStepListViewModel : ViewModel
    {
        public override string Title => ExtensionUIResources.StepByStepTitle;

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateStepByStepInstructionsCommand { get; }
        public ICommand NavigateContinueStepByStepInstructionsCommand { get; }
        public IList< Recognizer > InstructionSetList { get; set; }
        public InstructionSet SelectedInstructionSet { get; set;  }

        public StepByStepListViewModel(
            NavigationStore navigationStore)
        {
            NavigateHomeCommand = new NavigateCommand< HomeViewModel >(
                navigationStore,
                () => new HomeViewModel(navigationStore));
            
            // Next button 
            NavigateStepByStepInstructionsCommand =
                new NavigateCommand< StepByStepInstructionsViewModel >(
                    navigationStore,
                    () => new StepByStepInstructionsViewModel(
                        navigationStore,
                        SelectedInstructionSet,
                        false));
            
            // Continue button
            NavigateContinueStepByStepInstructionsCommand =
                new NavigateCommand<StepByStepInstructionsViewModel>(
                    navigationStore,
                    () => new StepByStepInstructionsViewModel(
                        navigationStore,
                        SelectedInstructionSet,
                        true));

            GetInstructionSetsResponse response = 
                GrpcHelper.StepByStepClient.GetInstructionSets(new GetInstructionSetsRequest());

            List<Recognizer> temFilteredRecognizers = response.Recognizers.Where(r => r is Recognizer.Singleton).ToList();
            // Dropdown menu in the SBS viewmodel
            InstructionSetList = temFilteredRecognizers;

            Recognizer test = InstructionSetList.FirstOrDefault();

            GetInstructionSetResponse response2 =
                GrpcHelper.StepByStepClient.GetInstructionSet(new GetInstructionSetRequest
                {
                    SelectedCombobox = test 
                });

            SelectedInstructionSet = response2.SelectedInstructionset;
        }
    }
}
