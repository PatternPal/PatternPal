#region

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using PatternPal.Extension.Commands;
using PatternPal.Extension.Grpc;
using PatternPal.Extension.Resources;
using PatternPal.Extension.Stores;
using PatternPal.Protos;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

#endregion

namespace PatternPal.Extension.ViewModels
{
    public class StepByStepListViewModel : ViewModel
    {
        public override string Title => ExtensionUIResources.StepByStepTitle;

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateStepByStepInstructionsCommand { get; }
        public ICommand NavigateContinueStepByStepInstructionsCommand { get; }
        public IList< Recognizer > InstructionSetList { get; set; }
        public Recognizer SelectedInstructionSet { get; set;  }

        public StepByStepListViewModel(
            NavigationStore navigationStore)
        {
            NavigateHomeCommand = new NavigateCommand< HomeViewModel >(
                navigationStore,
                () => new HomeViewModel(navigationStore));
            
            // Next button 
            NavigateStepByStepInstructionsCommand =
                new NavigateCommand< StepByStepInstructionsViewModel >(
                    navigationStore,
                    () =>
                    {
                        return new StepByStepInstructionsViewModel(
                            navigationStore,
                            SelectedInstructionSet,
                            StepByStepModes.New,
                            CreateNewWorkFileInSolution());
                    });
            
            // Continue button
            NavigateContinueStepByStepInstructionsCommand =
                new NavigateCommand<StepByStepInstructionsViewModel>(
                    navigationStore,
                    () =>
                    {
                        return new StepByStepInstructionsViewModel(
                            navigationStore,
                            SelectedInstructionSet,
                            StepByStepModes.Continue,
                            ContinueButtonBehavior());
                    });

            GetInstructionSetsResponse inStructionSetsResponse = 
                GrpcHelper.StepByStepClient.GetInstructionSets(new GetInstructionSetsRequest());

            InstructionSetList = inStructionSetsResponse.Recognizers;
            SelectedInstructionSet = InstructionSetList.FirstOrDefault();
        }

        private List<string> ContinueButtonBehavior()
        {
            List<string> result = new List<string>();

            OpenFileDialog ofd = new OpenFileDialog() { Multiselect = true, Filter = "cs files (*.cs)|*.cs", };

            while (result.Count == 0)
            {
                bool? res = ofd.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    result = ofd.FileNames.ToList();
                }
                else
                {
                    MessageBox.Show("No files were provided!");
                    ofd.ShowDialog();
                }
            }

            return result;
        }

        /// <summary>
        /// Prompts the user to add a file to the opened solution and adds it to a project or 
        /// creates a project.
        /// </summary>
        /// <param name="stepByStepModel"></param>
        /// <returns></returns>
        public List<string> CreateNewWorkFileInSolution()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Obtain the currently running visual studio instance.
            DTE dte = (DTE)Package.GetGlobalService(typeof(SDTE));

            if (VsShellUtilities.PromptYesNo(
                            "Do you want to add a file to the current solution?",
                            "",
                            OLEMSGICON.OLEMSGICON_QUERY,
                            (IVsUIShell)Package.GetGlobalService(typeof(IVsUIShell))))
            {
                // If there are no projects in the solution create one.
                List<string> result = CreateNewWorkFile(out string filePath);
                if (dte.Solution.Projects.Count == 0)
                {
                    string projectName = "NewProject";
                    string projectPath = System.IO.Path.Combine(dte.Solution.FullName, projectName);
                    Project csTemplateProject =
                        dte.Solution.AddFromTemplate(
                            "ConsoleApplication",
                            projectPath,
                            projectName,
                            false);

                    csTemplateProject.ProjectItems.AddFromFile(filePath);
                }
                else
                {
                    Project project = dte.Solution.Projects.Item(1);
                    project.ProjectItems.AddFromFile(filePath);
                }
                dte.ItemOperations.OpenFile(filePath);
                return result;
            }
            else
            {
                // Call what you would do otherwise when there is no solution like in
                return CreateNewWorkFile(out string filePath);
            }
        }

        /// <summary>
        /// Prompts the user to provide a filepath and creates a new .cs file and opens it.
        /// </summary>
        /// <param name="stepByStepModel"></param>
        /// <returns></returns>
        public List<string> CreateNewWorkFile(out string filePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Obtain the currently running visual studio instance.
            DTE dte = (DTE)Package.GetGlobalService(typeof(SDTE));

            // Save location for project provided by user
            string folderPath = string.Empty;
            using (FolderBrowserDialog fdb = new FolderBrowserDialog())
            {
                if (fdb.ShowDialog() == DialogResult.OK)
                {
                    folderPath = fdb.SelectedPath;
                }
            }

            string fileName = "NewFile.cs";
            filePath = 
                Path.Combine(
                    folderPath,
                    fileName);
            using (FileStream fs = File.Create(filePath)) { }
            dte.ItemOperations.OpenFile(filePath);
            return new List<string>() { filePath };
        }
    }
}
