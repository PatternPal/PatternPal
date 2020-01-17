using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using IDesign.Core.Models;
using IDesign.Recognizers.Abstractions;

namespace IDesign.Extension.ViewModels
{
    public class PatternComponentViewModel
    {
        public string Name { get; set; }

        public List<IEntityNode> EntityNodes { get; set;  } = new List<IEntityNode>();

        public PatternComponentViewModel(string name)
        {
            Name = name;
        }


        public List<EntityNodeViewModel> EntityNodeViewModels => EntityNodes.Select(x => new EntityNodeViewModel(x)).ToList();

    }
}