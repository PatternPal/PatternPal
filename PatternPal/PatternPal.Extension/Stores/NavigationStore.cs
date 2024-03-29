﻿#region

using System;
using System.Collections.Generic;
using PatternPal.Extension.ViewModels;

#endregion

namespace PatternPal.Extension.Stores
{
    public class NavigationStore
    {
        private ViewModel _currentViewModel { get; set; }

        /// <summary>
        /// The viewmodel that is currently active
        /// </summary>
        public ViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                if (_currentViewModel != null)
                {
                    ViewModelHistory.Push(_currentViewModel);
                }

                _currentViewModel = value;
                CurrentViewModelChanged?.Invoke();
            }
        }

        /// <summary>
        /// Event that fires when the current viewmodel is changed
        /// </summary>
        public event Action CurrentViewModelChanged;

        /// <summary>
        /// Stack that contains the history of visited viewmodels.
        /// When a back button is pressed, the top viewmodel is removed from viewmodelhistory
        /// </summary>
        protected Stack<ViewModel> ViewModelHistory = new Stack<ViewModel>();

        /// <summary>
        /// Used by the back command to retrieve the viewmodel that the user should go back to
        /// </summary>
        /// <returns></returns>
        public ViewModel Back()
        {
            if (ViewModelHistory.Count > 0)
            {
                if (CurrentViewModel.GetType() == typeof(StepByStepListViewModel))
                    ExtensionWindowPackage.CurrentMode = Mode.Default;
                _currentViewModel = ViewModelHistory.Pop();
                CurrentViewModelChanged?.Invoke();
                return _currentViewModel;
            }
            return null;
        }
    }
}
