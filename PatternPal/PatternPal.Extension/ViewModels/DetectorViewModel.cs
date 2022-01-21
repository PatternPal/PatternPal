using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PatternPal.Extension.Commands;
using PatternPal.Extension.Stores;

namespace PatternPal.Extension.ViewModels
{
    public class DetectorViewModel : ViewModel
    {
        public override string Title => Resources.ExtensionUIResources.DetectorTitle;
        public ICommand NavigateHomeCommand { get; }
        public DetectorViewModel(NavigationStore navigationStore)
        {
            NavigateHomeCommand = new NavigateCommand<HomeViewModel>(navigationStore, () => new HomeViewModel(navigationStore));
        }
    }
}
