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
        public IList< InstructionSet > InstructionSetList { get; set; }
        public InstructionSet SelectedInstructionSet { get; set;  }

        public StepByStepListViewModel(
            NavigationStore navigationStore)
        {
            NavigateHomeCommand = new NavigateCommand< HomeViewModel >(
                navigationStore,
                () => new HomeViewModel(navigationStore));
            NavigateStepByStepInstructionsCommand =
                new NavigateCommand< StepByStepInstructionsViewModel >(
                    navigationStore,
                    () => new StepByStepInstructionsViewModel(
                        navigationStore,
                        SelectedInstructionSet));

            Protos.PatternPal.PatternPalClient client = new Protos.PatternPal.PatternPalClient(GrpcChannelHelper.Channel);
            GetInstructionSetsResponse response = client.GetInstructionSets(new GetInstructionSetsRequest());

            InstructionSetList = response.InstructionSets;
            SelectedInstructionSet = InstructionSetList?.FirstOrDefault();
        }
    }
}
