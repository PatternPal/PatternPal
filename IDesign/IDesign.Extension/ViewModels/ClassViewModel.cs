using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;

namespace IDesign.Extension.ViewModels
{
    public class ClassViewModel
    {
        public ClassViewModel(IEntityNode entityNode)
        {
            EntityNode = entityNode;
        }

        public IEntityNode EntityNode { get; set; }
        public string ClassName => EntityNode.GetName();

        public ResultViewModel BestMatch => Results.OrderByDescending(x => x.Score).FirstOrDefault();
        public List<ResultViewModel> Results { get; set; } = new List<ResultViewModel>();
    }
}