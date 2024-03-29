﻿#region

using System;
using System.Windows.Documents;
using System.Windows.Input;
using PatternPal.Extension.Stores;
using PatternPal.Extension.ViewModels;

#endregion

/// <summary>
/// Represents a command that navigates to a view model of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the view model to navigate to.</typeparam>
public class NavigateCommand<T> : ICommand
    where T : ViewModel
{
    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    #pragma warning disable CS0067
    public event EventHandler CanExecuteChanged;
    #pragma warning restore CS0067

    /// <summary>
    /// Gets the navigation store used to manage the current view model.
    /// </summary>
    private NavigationStore NavigationStore { get; }

    /// <summary>
    /// Gets the function used to create the view model to navigate to.
    /// </summary>
    private Func<T> GetViewModel { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigateCommand{T}"/> class.
    /// </summary>
    /// <param name="navigationStore">The navigation store used to manage the current view model.</param>
    /// <param name="getViewModel">The function used to create the view model to navigate to.</param>
    public NavigateCommand(
        NavigationStore navigationStore,
        Func<T> getViewModel)
    {
        NavigationStore = navigationStore;
        GetViewModel = getViewModel;
    }

    /// <summary>
    /// Determines whether the command can be executed.
    /// </summary>
    /// <param name="parameter">The command parameter.</param>
    /// <returns>True</returns>
    public bool CanExecute(object parameter)
    {
        return true;
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="parameter">The command parameter.</param>
    public void Execute(object parameter)
    {
        T viewModel = GetViewModel();
        NavigationStore.CurrentViewModel = viewModel;
    }
}

public class SBScommand<T> : ICommand
where T : ViewModel
{
    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    #pragma warning disable CS0067
    public event EventHandler CanExecuteChanged;
    #pragma warning restore CS0067

    /// <summary>
    /// Gets the navigation store used to manage the current view model.
    /// </summary>
    private NavigationStore NavigationStore { get; }

    /// <summary>
    /// Gets the function used to create the view model to navigate to.
    /// </summary>
    private Func<T> GetViewModel { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigateCommand{T}"/> class.
    /// </summary>
    /// <param name="navigationStore">The navigation store used to manage the current view model.</param>
    /// <param name="getViewModel">The function used to create the view model to navigate to.</param>
    public SBScommand(
        NavigationStore navigationStore,
        Func<T> getViewModel)
    {
        NavigationStore = navigationStore;
        GetViewModel = getViewModel;
    }

    /// <summary>
    /// Determines whether the command can be executed.
    /// </summary>
    /// <param name="parameter">The command parameter.</param>
    /// <returns>True</returns>
    public bool CanExecute(object parameter)
    {
        return true;
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="parameter">The command parameter.</param>
    public void Execute(object parameter)
    {
        T viewModel = GetViewModel();

        // Only proceed to the instructions view when a valid file was provided by the user.
        if (viewModel is StepByStepInstructionsViewModel stepByStepInstructionsViewModel)
        {
            if (stepByStepInstructionsViewModel.FilePaths.Count > 0)
            {
                NavigationStore.CurrentViewModel = viewModel;
            }
        }
    }
}
