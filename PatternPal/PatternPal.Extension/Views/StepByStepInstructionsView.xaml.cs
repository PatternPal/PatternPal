#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

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
        private readonly Protos.PatternPal.PatternPalClient _client;
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
            _client = new Protos.PatternPal.PatternPalClient(GrpcChannelHelper.Channel);
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
                                      if (this.DataContext == null)
                                      {
                                          return;
                                      }

                                      _viewModel = (StepByStepInstructionsViewModel)(this.DataContext);

                                      CheckIfNextPreviousButtonsAvailable();
                                  };
        }

        private void CheckIfCheckIsAvailable()
        {
            ClassSelection.Visibility = _viewModel.CurrentInstruction.ShowFileSelector
                ? Visibility.Visible
                : Visibility.Hidden;
            CheckImplementationButton.IsEnabled = !_viewModel.CurrentInstruction.ShowFileSelector || ClassSelection.SelectedItem != null;

            NextInstructionButton.IsEnabled = false;
            ExpanderResults.ResultsView.ItemsSource = new List< PatternResultViewModel >();
        }

        private void CheckImplementationButton_OnClick(
            object sender,
            RoutedEventArgs e)
        {
            NextInstructionButton.IsEnabled = false;

            CheckInstructionRequest request = new CheckInstructionRequest
                                              {
                                                  InstructionSetName = _viewModel.InstructionSet.Name,
                                                  InstructionId = _viewModel.CurrentInstructionNumber - 1,
                                                  SelectedItem = _viewModel.SelectedcbItem,
                                              };

            LoadProject();

            foreach (Project project in Projects)
            {
                foreach (Document document in project.Documents)
                {
                    request.Documents.Add(document.FilePath);
                }
            }

            try
            {
                Protos.PatternPal.PatternPalClient client = new Protos.PatternPal.PatternPalClient(GrpcChannelHelper.Channel);
                RecognizerResult result = client.CheckInstruction(request);

                List< PatternResultViewModel > viewModels = new List< PatternResultViewModel >
                                                            {
                                                                new PatternResultViewModel(result)
                                                                {
                                                                    Expanded = true
                                                                }
                                                            };

                bool correct = result.Result.Results.All(c => c.FeedbackType == FeedbackType.FeedbackCorrect);

                ExpanderResults.ResultsView.ItemsSource = viewModels;

                if (!correct)
                    return;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return;
            }

            NextInstructionButton.IsEnabled = true;
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

        private void OnDropDownOpened(
            object sender,
            EventArgs e)
        {
            _viewModel.cbItems.Clear();
            LoadProject();

            GetSelectableClassesRequest request = new GetSelectableClassesRequest();
            foreach (Project project in Projects)
            {
                foreach (Document document in project.Documents)
                {
                    request.Documents.Add(document.FilePath);
                }
            }

            GetSelectableClassesResponse response = _client.GetSelectableClasses(request);
            foreach (string selectableClass in response.SelectableClasses)
            {
                _viewModel.cbItems.Add(selectableClass);
            }
        }

        private void OnSelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            CheckImplementationButton.IsEnabled = true;
            NextInstructionButton.IsEnabled = false;
        }
    }
}
