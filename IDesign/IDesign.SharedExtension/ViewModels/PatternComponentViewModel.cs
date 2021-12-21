using System.Collections.Generic;
using System.Linq;
using SyntaxTree.Abstractions.Entities;

namespace IDesign.Extension.ViewModels
{
    public class PatternComponentViewModel
    {
        public PatternComponentViewModel(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public List<IEntity> EntityNodes { get; set; } = new List<IEntity>();

        public List<EntityNodeViewModel> EntityNodeViewModels =>
            EntityNodes.Select(x => new EntityNodeViewModel(x)).ToList();
    }
}
