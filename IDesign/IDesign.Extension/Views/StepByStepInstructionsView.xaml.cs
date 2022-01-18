using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EnvDTE;
using IDesign.Core;
using IDesign.Extension.ViewModels;
using IDesign.Recognizers.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IDesign.StepByStep.Abstractions;
using SyntaxTree;
using SyntaxTree.Abstractions.Entities;
using Project = Microsoft.CodeAnalysis.Project;

namespace IDesign.Extension.Views
{
    /// <summary>
    /// Interaction logic for StepByStepInstructionsView.xaml
    /// </summary>
    public partial class StepByStepInstructionsView : UserControl, IVsSolutionEvents, IVsRunningDocTableEvents
    {
        private StepByStepInstructionsViewModel _viewModel;

        public StepByStepInstructionsView()
        {
            InitializeComponent();
            InitializeViewModelAndButtons();
            
            Dispatcher.VerifyAccess();
            LoadProject();
            Dte = Package.GetGlobalService(typeof(SDTE)) as DTE;
            var rdt = (IVsRunningDocumentTable)Package.GetGlobalService(typeof(SVsRunningDocumentTable));
            rdt.AdviseRunningDocTableEvents(this, out _);
            var ss = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
            ss.AdviseSolutionEvents(this, out _);
        }

        private List<string> Paths { get; set; }
        private List<Project> Projects { get; set; }
        private DTE Dte { get; }

        /// <summary>
        /// Activated after clicking on the previous instruction button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviousInstructionButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_viewModel.CurrentInstruction.Previous != null)
            {
                _viewModel.CurrentInstruction = _viewModel.CurrentInstruction.Previous;
                _viewModel.CurrentInstructionNumber--;
                CheckIfNextPreviousButtonsAvailable();
            }
        }

        /// <summary>
        /// Activated after clicking on the next instruction button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextInstructionButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_viewModel.CurrentInstruction.Next != null)
            {
                _viewModel.CurrentInstruction = _viewModel.CurrentInstruction.Next;
                _viewModel.CurrentInstructionNumber++;
                CheckIfNextPreviousButtonsAvailable();
            }
        }

        /// <summary>
        /// Checks if the next and previous instruction buttons should be visible/hidden
        /// </summary>
        private void CheckIfNextPreviousButtonsAvailable()
        {
            NextInstructionButton.Visibility =
                _viewModel.CurrentInstruction.Next != null ? Visibility.Visible : Visibility.Hidden;
            PreviousInstructionButton.Visibility = _viewModel.CurrentInstruction.Previous != null
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

        private void CheckImplementationButton_OnClick(object sender, RoutedEventArgs e)
        {
            LoadProject();

            var graph = new SyntaxGraph();

            foreach (var project in Projects)
            {
                foreach (var document in project.Documents)
                {
                    var text = FileManager.MakeStringFromFile(document.FilePath);
                    graph.AddFile(text, document.FilePath);
                }
            }
            
            graph.CreateGraph();

            var instruction = _viewModel.CurrentInstruction.Value;
            if (instruction is IFileSelector fileSelector)
            {
                _viewModel.State[fileSelector.FileId] = graph.GetAll().FirstOrDefault().Value;
                /*graph.GetAll().TryGetValue("testProjectyay.Duck", out IEntity value1);
                graph.GetAll().TryGetValue("testProjectyay.IBehaviour", out IEntity value2);
                graph.GetAll().TryGetValue("testProjectyay.VeryCoolBehaviour", out IEntity value3);
                graph.GetAll().TryGetValue("testProjectyay.VeryCoolDuck", out IEntity value4);
                _viewModel.State["strategy.abstract"] = value1;
                _viewModel.State["strategy.interface"] = value2;
                _viewModel.State["strategy.interface.subclass"] = value3;
                _viewModel.State["strategy.abstract.subclass"] = value4;*/
            }

            foreach (var check in instruction.Checks)
            {
                var result = check.Correct(_viewModel.State);
                if (result.GetFeedbackType() == FeedbackType.Incorrect)
                {
                    Trace.WriteLine("incorrect");
                    return;
                }
            }
        }
        
        

        #region IVsSolutionEvents
        
        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        ///     Gets all the projects after opening solution.
        /// </summary>
        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            LoadProject();
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }
        
        #endregion

        private void LoadProject()
        {
            var cm = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            var ws = (Workspace)cm.GetService<VisualStudioWorkspace>();
            Projects = ws.CurrentSolution.Projects.ToList();
        }
        
        #region IVsRunningDocTableEvents3 implementation

        int IVsRunningDocTableEvents.OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType,
            uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType,
            uint dwReadLocksRemaining,
            uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnAfterSave(uint docCookie)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}
