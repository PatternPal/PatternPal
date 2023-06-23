#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using EnvDTE;
using EnvDTE80;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.WindowsAPICodePack.Dialogs;

using PatternPal.Extension.Grpc;
using PatternPal.Extension.Resources;
using PatternPal.Extension.Stores;
using PatternPal.Protos;

#endregion

namespace PatternPal.Extension.ViewModels
{
    /// <summary>
    /// The view model for the Step by Step page.
    /// </summary>
    public class StepByStepListViewModel : ViewModel
    {
        /// <inheritdoc />
        public override string Title => ExtensionUIResources.StepByStepTitle;

        /// <summary>
        /// Command to navigate home.
        /// </summary>
        public ICommand NavigateHomeCommand { get; }

        /// <summary>
        /// Command to navigate to the first step.
        /// </summary>
        public ICommand NavigateStepByStepInstructionsCommand { get; }

        /// <summary>
        /// Command to allow user to select new file before going to next step.
        /// </summary>
        public ICommand NavigateContinueStepByStepInstructionsCommand { get; }

        /// <summary>
        /// Selectable Step by Step implementations.
        /// </summary>
        public IList< Recognizer > InstructionSetList { get; set; }

        /// <summary>
        /// The selected Step by Step implementation.
        /// </summary>
        public Recognizer SelectedInstructionSet { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="StepByStepListViewModel"/> class.
        /// </summary>
        /// <param name="navigationStore">The <see cref="NavigationStore"/> to use for navigation commands.</param>
        public StepByStepListViewModel(
            NavigationStore navigationStore)
        {
            NavigateHomeCommand = new NavigateCommand< HomeViewModel >(
                navigationStore,
                () => new HomeViewModel(navigationStore));

            // Next button 
            NavigateStepByStepInstructionsCommand =
                new SBScommand< StepByStepInstructionsViewModel >(
                    navigationStore,
                    () => new StepByStepInstructionsViewModel(
                        navigationStore,
                        SelectedInstructionSet,
                        StepByStepModes.New,
                        CreateNewWorkFileInSolution()));

            // Continue button
            NavigateContinueStepByStepInstructionsCommand =
                new SBScommand<StepByStepInstructionsViewModel>(
                    navigationStore,
                    () => new StepByStepInstructionsViewModel(
                        navigationStore,
                        SelectedInstructionSet,
                        StepByStepModes.Continue,
                        ContinueButtonBehavior()));

            // Obtain the available instruction sets for Step-By-Step.
            GetInstructionSetsResponse instructionSetsResponse =
                GrpcHelper.StepByStepClient.GetInstructionSets(new GetInstructionSetsRequest());

            InstructionSetList = instructionSetsResponse.Recognizers;
            SelectedInstructionSet = InstructionSetList.FirstOrDefault();
        }

        /// <summary>
        /// Handles letting a user select a file with which to continue the Step by Step implementation.
        /// </summary>
        private List< string > ContinueButtonBehavior()
        {
            List< string > result = new List< string >();

            using (CommonOpenFileDialog ofd = new CommonOpenFileDialog
                   {
                       Multiselect = true,
                       Filters = { new CommonFileDialogFilter(
                           "cs files",
                           "cs") }
            })
            {
                CommonFileDialogResult res = ofd.ShowDialog();
                if (res == CommonFileDialogResult.Ok)
                {
                    result = ofd.FileNames.ToList();
                }
                else
                {
                    System.Windows.MessageBox.Show("No files were provided!");
                }
            }

            ThreadHelper.ThrowIfNotOnUIThread();
            // Obtain the currently running visual studio instance.
            DTE dte = (DTE)Package.GetGlobalService(typeof( SDTE ));
            foreach (string file in result)
            {
                dte.ItemOperations.OpenFile(file);
            }

            return result;
        }

        /// <summary>
        /// Prompts the user to add a file to the opened solution and adds it to a project or 
        /// creates a project.
        /// </summary>
        private List< string > CreateNewWorkFileInSolution()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Obtain the currently running visual studio instance.
            DTE dte = (DTE)Package.GetGlobalService(typeof( SDTE ));

            if (VsShellUtilities.PromptYesNo(
                "Do you want to add a file to the current solution?",
                "",
                OLEMSGICON.OLEMSGICON_QUERY,
                (IVsUIShell)Package.GetGlobalService(typeof( IVsUIShell ))))
            {
                if (dte.Solution.IsOpen)
                {
                    string filePath;

                    // The solution is empty.
                    if (dte.Documents.Count == 0)
                    {
                        // Create a project based on a template from VS2022 add it to the 
                        // solution. Then iterate over the projects of the solution and 
                        // add a file.
                        string projectName = "NewProject";
                        string projectPath = Path.Combine(
                            Path.GetDirectoryName(dte.Solution.FullName),
                            projectName);

                        // Obtain the template path for a project.
                        DTE2 dte2 = (DTE2)Package.GetGlobalService(typeof(SDTE));
                        Solution2 soln = dte2.Solution as Solution2;
                        string templatePath = soln.GetProjectTemplate("ConsoleApplication.zip", "CSharp");

                        // Add project to the solution and add a new file to that project.
                        dte.Solution.AddFromTemplate(
                            templatePath,
                            projectPath,
                            projectName,
                            false);
                        Project project = dte.Solution.Projects.Item(1);
                        filePath =
                            Path.Combine(
                                projectPath,
                                "NewFile.cs");
                        using (FileStream fs = File.Create(filePath)) { }
                        project.ProjectItems.AddFromFile(filePath);
                    }
                    else
                    {
                        Project project = dte.Solution.Projects.Item(1);
                        filePath = Path.Combine(
                            Path.GetDirectoryName(dte.Solution.FullName),
                            project.Name,
                            "NewFile.cs");
                        using (FileStream fs = File.Create(filePath)) { }
                        project.ProjectItems.AddFromFile(filePath);
                    }
                    dte.ItemOperations.OpenFile(filePath);
                    return new List<string> { filePath };
                }

                MessageBox.Show("There is no solution open");
                return new List< string >();
            }
            else
            {
                // Call what you would do otherwise when there is no solution loaded.
                List< string > result = CreateNewWorkFile(out string filePath);
                if (filePath == string.Empty)
                {
                    MessageBox.Show("No save location was provided");
                    return result;
                }

                dte.ItemOperations.OpenFile(filePath);
                return result;
            }
        }

        /// <summary>
        /// Prompts the user to provide a filepath and creates a new .cs file and opens it.
        /// </summary>
        private List< string > CreateNewWorkFile(
            out string filePath)
        {
            // Save location for project provided by user
            string folderPath = string.Empty;

            using (CommonOpenFileDialog fdb = new CommonOpenFileDialog
                                              {
                                                  IsFolderPicker = true,
                                              })
            {
                if (fdb.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    folderPath = fdb.FileName;
                }
                else
                {
                    filePath = string.Empty;
                    return new List< string >();
                }
            }

            filePath =
                Path.Combine(
                    folderPath,
                    "NewFile.cs");
            if (folderPath == string.Empty)
            {
                return new List< string >();
            }
            using (FileStream fs = File.Create(filePath)) { }
            return new List< string >
            {
                       filePath
                   };
        }
    }
}
