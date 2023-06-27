#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using PatternPal.Extension.Commands;
using PatternPal.Extension.Grpc;
using PatternPal.Extension.ViewModels;
using PatternPal.Protos;

#endregion

namespace PatternPal.Extension.Views
{
    /// <summary>
    /// Interaction logic for StepByStepInstructionsView.xaml
    /// </summary>
    public partial class StepByStepInstructionsView : IVsSolutionEvents,
                                                      IVsRunningDocTableEvents
    {
        private StepByStepInstructionsViewModel _viewModel;

        public StepByStepInstructionsView()
        {
            InitializeComponent();
            InitializeViewModelAndButtons();

            Dispatcher.VerifyAccess();
            LoadProject();
            IVsRunningDocumentTable rdt = (IVsRunningDocumentTable)Package.GetGlobalService(typeof( SVsRunningDocumentTable ));
            rdt.AdviseRunningDocTableEvents(
                this,
                out _);
            IVsSolution ss = (IVsSolution)Package.GetGlobalService(typeof( SVsSolution ));
            ss.AdviseSolutionEvents(
                this,
                out _);
        }

        private List< Project > Projects { get; set; }

        /// <summary>
        /// Activated after clicking on the previous instruction button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviousInstructionButton_OnClick(
            object sender,
            RoutedEventArgs e)
        {
            if (_viewModel.TrySelectPreviousInstruction())
            {
                correctTextBlock.Visibility = Visibility.Hidden;
                CheckIfNextPreviousButtonsAvailable();
            }
        }

        /// <summary>
        /// Activated after clicking on the next instruction button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextInstructionButton_OnClick(
            object sender,
            RoutedEventArgs e)
        {
            if (_viewModel.TrySelectNextInstruction())
            {
                correctTextBlock.Visibility = Visibility.Hidden;
                CheckIfNextPreviousButtonsAvailable();
            }
        }

        /// <summary>
        /// Checks if the next and previous instruction buttons should be visible/hidden
        /// </summary>
        private void CheckIfNextPreviousButtonsAvailable()
        {
            CheckIfCheckIsAvailable();
            NextInstructionButton.Visibility = _viewModel.HasNextInstruction
                ? Visibility.Visible
                : Visibility.Hidden;
            PreviousInstructionButton.Visibility = _viewModel.HasPreviousInstruction
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void InitializeViewModelAndButtons()
        {
            DataContextChanged += delegate
                                  {
                                      // only bind events when dataContext is set, not unset
                                      if (DataContext == null)
                                      {
                                          return;
                                      }

                                      _viewModel = (StepByStepInstructionsViewModel)(DataContext);
                                      CheckIfNextPreviousButtonsAvailable();
                                  };
        }

        /// <summary>
        /// Checks if the check implementation button is able to be clicked
        /// </summary>
        private void CheckIfCheckIsAvailable()
        {
            CheckImplementationButton.IsEnabled = _viewModel.FilePaths.Any();

            NextInstructionButton.IsEnabled = false;
            ExpanderResults.ResultsView.ItemsSource = new List< PatternResultViewModel >();
        }

        /// <summary>
        /// Given the current step checks whether any of the files provided have implemented
        /// the check.
        /// </summary>
        private void CheckImplementationButton_OnClick(
            object sender,
            RoutedEventArgs e)
        {
            NextInstructionButton.IsEnabled = false;

            CheckInstructionRequest request = new CheckInstructionRequest
                                              {
                                                  InstructionNumber = _viewModel.CurrentInstructionNumber - 1,
                                                  Recognizer = _viewModel.Recognizer
                                              };

            foreach (string file in _viewModel.FilePaths)
            {
                request.Documents.Add(file);
            }

            try
            {
                CheckInstructionResponse response = GrpcHelper.StepByStepClient.CheckInstruction(request);
                SubscribeEvents.OnStepByStepCheck(
                    request.Recognizer.ToString(),
                    request.InstructionNumber,
                    response.Result);

                if (response.RecognizeResult == null)
                {
                    correctTextBlock.Visibility = Visibility.Visible;
                    correctTextBlock.Text = "Incorrect";
                    correctTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                    return;
                }

                ExpanderResults.ResultsView.ItemsSource = new[ ]
                                                          {
                                                              new PatternResultViewModel(response.RecognizeResult)
                                                              {
                                                                  Expanded = true
                                                              }
                                                          };

                if (response.Result)
                {
                    NextInstructionButton.IsEnabled = true;
                }
            }
            catch (Exception)
            {
                GrpcHelper.ShowErrorMessage("Check of step gave an exception");
                return;
            }
        }

        #region IVsSolutionEvents

        public int OnAfterOpenProject(
            IVsHierarchy pHierarchy,
            int fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(
            IVsHierarchy pHierarchy,
            int fRemoving,
            ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(
            IVsHierarchy pHierarchy,
            int fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(
            IVsHierarchy pStubHierarchy,
            IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(
            IVsHierarchy pRealHierarchy,
            ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(
            IVsHierarchy pRealHierarchy,
            IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        ///     Gets all the projects after opening solution.
        /// </summary>
        public int OnAfterOpenSolution(
            object pUnkReserved,
            int fNewSolution)
        {
            LoadProject();
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(
            object pUnkReserved,
            ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(
            object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(
            object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        #endregion

        /// <summary>
        /// Obtains the current solution projects
        /// </summary>
        private void LoadProject()
        {
            IComponentModel cm = (IComponentModel)Package.GetGlobalService(typeof( SComponentModel ));
            Workspace ws = cm.GetService< VisualStudioWorkspace >();
            Projects = ws.CurrentSolution.Projects.ToList();
        }

        #region IVsRunningDocTableEvents3 implementation

        int IVsRunningDocTableEvents.OnAfterFirstDocumentLock(
            uint docCookie,
            uint dwRDTLockType,
            uint dwReadLocksRemaining,
            uint dwEditLocksRemaining
        )
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnBeforeLastDocumentUnlock(
            uint docCookie,
            uint dwRDTLockType,
            uint dwReadLocksRemaining,
            uint dwEditLocksRemaining
        )
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnAfterSave(
            uint docCookie)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnAfterAttributeChange(
            uint docCookie,
            uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnBeforeDocumentWindowShow(
            uint docCookie,
            int fFirstShow,
            IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnAfterDocumentWindowHide(
            uint docCookie,
            IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}
