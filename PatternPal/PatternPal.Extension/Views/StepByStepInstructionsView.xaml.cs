#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using EnvDTE;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using PatternPal.Extension.Grpc;
using PatternPal.Extension.Model;
using PatternPal.Extension.ViewModels;
using PatternPal.Protos;
using PatternPal.StepByStep.Abstractions;

using SyntaxTree;
using SyntaxTree.Abstractions.Entities;

using Document = Microsoft.CodeAnalysis.Document;
using Project = Microsoft.CodeAnalysis.Project;

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
            Dte = Package.GetGlobalService(typeof( SDTE )) as DTE;
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

        private List< string > Paths { get; set; }
        private List< Project > Projects { get; set; }
        private DTE Dte { get; }

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

        private Dictionary< string, string > keyed = new Dictionary< string, string >();

        private void CheckIfCheckIsAvailable()
        {
            ClassSelection.Visibility = _viewModel.CurrentInstruction.ShowFileSelector
                ? Visibility.Visible
                : Visibility.Hidden;
            if (_viewModel.CurrentInstruction.ShowFileSelector)
            {
                ClassSelection.SelectedItem =
                    keyed.ContainsKey(_viewModel.CurrentInstruction.FileId)
                        ? keyed[ _viewModel.CurrentInstruction.FileId ]
                        : null;
                CheckImplementationButton.IsEnabled = ClassSelection.SelectedItem != null;
            }

            NextInstructionButton.IsEnabled = false;
            ExpanderResults.ResultsView.ItemsSource = new List< PatternResultViewModel >();
        }

        private void CheckImplementationButton_OnClick(
            object sender,
            RoutedEventArgs e)
        {
            NextInstructionButton.IsEnabled = false;
            SyntaxGraph graph = CreateGraph(false);

            if (_viewModel.CurrentInstruction.ShowFileSelector)
            {
                if (_viewModel.SelectedcbItem == null)
                    return;
                _viewModel.State[ _viewModel.CurrentInstruction.FileId ] = graph.GetAll()[ _viewModel.SelectedcbItem ];
            }

            IInstructionState state = _createState(graph);
            List< PatternResultViewModel > viewModels = new List< PatternResultViewModel >
                                                        {
                                                            new PatternResultViewModel(
                                                                new RecognizerResult
                                                                {
                                                                    DetectedPattern = _viewModel.InstructionSet.Name,
                                                                    //Result = new Result
                                                                    //{
                                                                    //    Results = instruction.Checks.Select(c => c.Correct(state)).ToList()
                                                                    //}
                                                                }
                                                            )
                                                            {
                                                                Expanded = true
                                                            }
                                                        };

            bool correct = false; //viewModels[0].Result.Result.GetResults().All(c => c.GetFeedbackType() == FeedbackType.Correct);

            ExpanderResults.ResultsView.ItemsSource = viewModels;

            if (!correct)
                return;

            //Save all changed state to the state between instructions, only when all is successful
            foreach (KeyValuePair< string, IEntity > pair in state)
            {
                keyed[ pair.Key ] = pair.Value?.GetFullName();
            }

            NextInstructionButton.IsEnabled = true;
        }

        private IInstructionState _createState(
            SyntaxGraph graph)
        {
            InstructionState state = new InstructionState();
            foreach (KeyValuePair< string, string > pair in keyed)
            {
                state[ pair.Key ] = pair.Value == null
                    ? null
                    : graph.GetAll()[ pair.Value ];
            }

            return state;
        }

        private SyntaxGraph CreateGraph(
            bool fill = true)
        {
            LoadProject();

            GetSelectableClassesRequest request = new GetSelectableClassesRequest();

            foreach (Project project in Projects)
            {
                foreach (Document document in project.Documents)
                {
                    request.Documents.Add(document.FilePath);
                }
            }

            if (fill)
            {
                GetSelectableClassesResponse response = _client.GetSelectableClasses(request);
                _viewModel.cbItems.Clear();
                foreach (string selectableClass in response.SelectableClasses)
                {
                    _viewModel.cbItems.Add(selectableClass);
                }
            }

            return null;
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
            Workspace ws = (Workspace)cm.GetService< VisualStudioWorkspace >();
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

            if (_viewModel.CurrentInstruction.ShowFileSelector)
            {
                keyed[ _viewModel.CurrentInstruction.FileId ] = _viewModel.SelectedcbItem;
                NextInstructionButton.IsEnabled = false;
            }
        }
    }
}
