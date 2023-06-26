#region

using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

#endregion

namespace PatternPal.Extension.ViewModels
{
    /// <summary>
    /// View model which represents selectable projects or the active document.
    /// </summary>
    public class SelectPathsViewModel : ViewModel
    {
        /// <inheritdoc />
        public override string Title => "Select Paths";

        /// <summary>
        /// List of projects which can be selected.
        /// </summary>
        public IList< string > SelectableProjects { get; set; }

        /// <summary>
        /// If the project option has been selected, this contains the selected project.
        /// </summary>
        public string SelectedProject { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="SelectPathsViewModel"/> class.
        /// </summary>
        /// <param name="projects">The projects which are selectable.</param>
        public SelectPathsViewModel(
            IEnumerable< Project > projects)
        {
            SelectableProjects = projects.Select(project => project.Name).ToList();
            SelectedProject = SelectableProjects.FirstOrDefault();
        }
    }
}
