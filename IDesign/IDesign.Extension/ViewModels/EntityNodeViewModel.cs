using SyntaxTree.Abstractions.Entities;

namespace IDesign.Extension.ViewModels {
    public class EntityNodeViewModel {
        public IEntity EntityNode { get; internal set; }

        public EntityNodeViewModel(IEntity entityNode) {
            EntityNode = entityNode;
        }

        public string Name => EntityNode.GetName();
    }
}
