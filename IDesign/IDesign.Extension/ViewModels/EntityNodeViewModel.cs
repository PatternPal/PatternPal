using SyntaxTree.Abstractions.Entities;

namespace IDesign.Extension.ViewModels
{
    public class EntityNodeViewModel
    {
        public EntityNodeViewModel(IEntity entityNode)
        {
            EntityNode = entityNode;
        }

        public IEntity EntityNode { get; internal set; }

        public string Name => EntityNode.GetName();
    }
}
