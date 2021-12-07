using IDesign.Recognizers.Abstractions;

namespace IDesign.Extension.ViewModels
{
    public class EntityNodeViewModel
    {
        public EntityNodeViewModel(IEntityNode entityNode)
        {
            EntityNode = entityNode;
        }

        public IEntityNode EntityNode { get; internal set; }

        public string Name => EntityNode.GetName();
    }
}
