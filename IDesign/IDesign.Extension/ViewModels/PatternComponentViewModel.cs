using System.Collections.Generic;
using System.Linq;
using SyntaxTree.Abstractions.Entities;

namespace IDesign.Extension.ViewModels {
    public class PatternComponentViewModel {
        public string Name { get; set; }

        public List<IEntity> EntityNodes { get; set; } = new List<IEntity>();

        public PatternComponentViewModel(string name) {
            Name = name;
        }

        public List<EntityNodeViewModel> EntityNodeViewModels =>
            EntityNodes.Select(x => new EntityNodeViewModel(x)).ToList();
    }
}
