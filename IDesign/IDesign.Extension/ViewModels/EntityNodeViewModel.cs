using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using IDesign.Core.Models;
using IDesign.Recognizers.Abstractions;

namespace IDesign.Extension.ViewModels
{
    public class EntityNodeViewModel
    {
        public IEntityNode EntityNode { get; internal set; }
        public EntityNodeViewModel(IEntityNode entityNode)
        {
            EntityNode = entityNode;
        }

        public string Name => EntityNode.GetName();
    }
}