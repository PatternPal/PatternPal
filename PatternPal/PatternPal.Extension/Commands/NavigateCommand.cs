#region

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using PatternPal.Extension.Stores;
using PatternPal.Extension.ViewModels;
using Process = System.Diagnostics.Process;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

#endregion

namespace PatternPal.Extension.Commands
{
    public class NavigateCommand<T> : ICommand
        where T : ViewModel
    {
        private NavigationStore NavigationStore { get; }
        private Func<T> GetViewModel { get; }

#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

        public NavigateCommand(
            NavigationStore navigationStore,
            Func<T> getViewModel)
        {
            NavigationStore = navigationStore;
            GetViewModel = getViewModel;
        }

        public bool CanExecute(
            object parameter)
        {
            return true;
        }

        public void Execute(
            object parameter)
        {
            T viewModel = GetViewModel();
            NavigationStore.CurrentViewModel = viewModel;
            ThreadHelper.ThrowIfNotOnUIThread();

            // Start a new project when the user wants to learn to implement the selected pattern
            // from the combobox.
            if (viewModel.GetType() == typeof(StepByStepInstructionsViewModel))
            {
                StepByStepInstructionsViewModel stepByStepViewModel =
                    viewModel as StepByStepInstructionsViewModel;

                // User has pressed 'next'.
                if (!stepByStepViewModel.SBScontinue)
                {
                    // Obtain the currently running visual studio instance.
                    DTE dte = (DTE)Package.GetGlobalService(typeof(SDTE));

                    // If there is a solution, prompt the user if the file is to be added to it.
                    if (dte.Solution.Count != 0)
                    {
                        StepByStepInstructionsViewModel newViewModel =
                            CreateNewWorkFileInSolution(stepByStepViewModel);
                        NavigationStore.CurrentViewModel = newViewModel;
                    }
                    // Otherwise just create a simple .cs file.
                    else
                    {
                        StepByStepInstructionsViewModel newViewModel = 
                            CreateNewWorkFile(stepByStepViewModel);
                        NavigationStore.CurrentViewModel = newViewModel;
                    }
                }
            }
        }
        
        /// <summary>
        /// Prompts the user to add a file to the opened solution and adds it to a project or 
        /// creates a project.
        /// </summary>
        /// <param name="stepByStepModel"></param>
        /// <returns></returns>
        public StepByStepInstructionsViewModel CreateNewWorkFileInSolution(
            StepByStepInstructionsViewModel stepByStepModel)
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
                string filePath = string.Empty;
                // If there are no projects in the solution create one.
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

                    string fileName = "NewFile.cs";
                    string fileContent = "";
                    filePath =
                        System.IO.Path.Combine(
                            System.IO.Path.GetDirectoryName(csTemplateProject.FullName),
                            fileName);
                    System.IO.File.WriteAllText(filePath, fileContent);
                    csTemplateProject.ProjectItems.AddFromFile(filePath);
                }
                else
                {
                    Project project = dte.Solution.Projects.Item(1);
                    string fileName = "NewFile.cs";
                    string fileContent = "";
                    filePath =
                        System.IO.Path.Combine(
                            System.IO.Path.GetDirectoryName(project.FullName),
                            fileName);
                    System.IO.File.WriteAllText(filePath, fileContent);
                    project.ProjectItems.AddFromFile(filePath);
                }
                dte.ItemOperations.OpenFile(filePath);
                stepByStepModel.FilePath = filePath;
                return stepByStepModel;
            }
            else
            {
                // Call what you would do otherwise when there is no solution like in
                StepByStepInstructionsViewModel newViewModel = 
                    CreateNewWorkFile(stepByStepModel);
                return newViewModel;
            }
        }

        /// <summary>
        /// Prompts the user to provide a filepath and creates a new .cs file and opens it.
        /// </summary>
        /// <param name="stepByStepModel"></param>
        /// <returns></returns>
        public StepByStepInstructionsViewModel CreateNewWorkFile(
            StepByStepInstructionsViewModel stepByStepModel)
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
            string fileContent = "using System;\r\n\r\npublic class MyClass\r\n{\r\n	public void MyClass()\r\n	{\r\n	}\r\n}";
            string filePath =
                Path.Combine(
                    folderPath,
                    fileName);
            File.WriteAllText(filePath, fileContent);
            dte.ItemOperations.OpenFile(filePath);

            stepByStepModel.FilePath = filePath;
            return stepByStepModel;
        }
    }
}
