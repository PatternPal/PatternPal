﻿using System.Windows.Input;
using PatternPal.Extension.Commands;
using PatternPal.Extension.Resources;
using PatternPal.Extension.Stores;

namespace PatternPal.Extension.ViewModels
{
    public class ConsentViewModel : ViewModel
    {
        public override string Title => Resources.ExtensionUIResources.ExtensionName;

        public ICommand NavigateHomeCommand { get; }

        public ConsentViewModel(NavigationStore navigationStore)
        {
            NavigateHomeCommand = new NavigateCommand<HomeViewModel>(navigationStore,
                                () => new HomeViewModel(navigationStore));
        }

    }
}
