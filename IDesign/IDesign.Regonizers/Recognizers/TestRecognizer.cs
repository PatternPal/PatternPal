using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models.Checks;
using IDesign.Recognizers.Models.Checks.Members;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Models;

namespace IDesign.Recognizers.Recognizers
{
    public class TestRecognizer : IRecognizer
    {
        public IResult Recognize(IEntity node)
        {
            var result = new Result();

            var methodCheck = new ModifierCheck(Modifiers.Private.Not(), Modifiers.Static);

            var resultResults = node.GetAllMethods().Select(methodCheck.Check).ToList();
            result.Results = resultResults;
            result.RelatedSubTypes.Add(node, "Test");
            
            return result;
        }
    }
}
