using SyntaxTree.Abstractions.Entities;

namespace PatternPal.Extension.ViewModels
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
