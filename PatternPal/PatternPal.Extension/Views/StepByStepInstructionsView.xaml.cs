#region

using System;
using System.Collections.Generic;
using System.IO;
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
                // TODO decrement the instruction id in service.
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
                // TODO increase the instruction id in service.
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

                                      if (!_viewModel.SBScontinue)
                                      {
                                          ClassSelection.Visibility = Visibility.Hidden;
                                      }

                                      CheckIfNextPreviousButtonsAvailable();
                                  };
        }

        /// <summary>
        /// Checks if the check implementation button is able to be clicked
        /// </summary>
        private void CheckIfCheckIsAvailable()
        {
            // TODO remove classselection visibility from view
            if (_viewModel.SBScontinue)
            {
                CheckImplementationButton.IsEnabled = ClassSelection.SelectedItem != null;
            }
            else
            {
                CheckImplementationButton.IsEnabled = 
                    _viewModel.CurrentInstruction.FileId != "" || !_viewModel.SBScontinue;
            }
            
            NextInstructionButton.IsEnabled = false;
            ExpanderResults.ResultsView.ItemsSource = new List< PatternResultViewModel >();
        }

        /// <summary>
        /// Given the current step checks whether the implementation 
        /// </summary>
        private void CheckImplementationButton_OnClick(
            object sender,
            RoutedEventArgs e)
        {
            NextInstructionButton.IsEnabled = false;

            if (_viewModel.SBScontinue)
            {
                CheckInstructionRequest request = new CheckInstructionRequest
                {
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
                    RecognizeResult result = GrpcHelper.StepByStepClient.CheckInstruction(request).Result;

                    List<PatternResultViewModel> viewModels = new List<PatternResultViewModel>
                    {
                        new PatternResultViewModel(result)
                        {
                            Expanded = true
                        }
                    };

                    bool correct = result.Results.All(c => c.FeedbackType == CheckResult.Types.FeedbackType.FeedbackCorrect);

                    ExpanderResults.ResultsView.ItemsSource = viewModels;

                    if (!correct)
                        return;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return;
                }
            }
            else
            {
                // When not continuing set the filepath for the steps to the file just created
                SetFilePathResponse response =
                    GrpcHelper.StepByStepClient.SetNewFilePath(new SetFilePathRequest
                    {
                        FilePath = _viewModel.FilePath
                    });

                // TODO check logic
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

        /// <summary>
        /// Obtains all the selectable classes for the instruction step for SBS
        /// </summary>
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

            GetSelectableClassesResponse response = GrpcHelper.StepByStepClient.GetSelectableClasses(request);

            foreach (string selectableClass in response.SelectableClasses)
            {
                _viewModel.cbItems.Add(selectableClass);
            }
            //foreach (Project current in Projects)
            //{
            //    IEnumerable<Document> test = current.Documents;
            //    foreach(Document document in test)
            //    {
            //        _viewModel.cbItems.Add(document.Name);
            //    }
            //}
        }

        /// <summary>
        /// Enables the implementation check button and disables the next instruction button
        /// </summary>
        private void OnSelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            CheckImplementationButton.IsEnabled = true;
            NextInstructionButton.IsEnabled = false;
        }
    }
}
