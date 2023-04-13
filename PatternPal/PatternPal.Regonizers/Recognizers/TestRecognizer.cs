using System.Linq;
using PatternPal.Recognizers.Abstractions;
using PatternPal.Recognizers.Models.Checks;
using PatternPal.Recognizers.Models.Checks.Members;
using PatternPal.Recognizers.Models.ElementChecks;
using PatternPal.Recognizers.Models.Output;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Models;

namespace PatternPal.Recognizers.Recognizers
{
    public class TestRecognizer : IRecognizer
    {
        public IResult Recognize(IEntity node)
        {
            var result = new Result();

            var methodCheck = new ModifierCheck(Modifier.Private.Not(), Modifier.Static);

            var resultResults = node.GetAllMethods().Select(methodCheck.Check).ToList();
            result.Results = resultResults;
            result.RelatedSubTypes.Add(node, "Test");
            
            return result;
        }
    }
}
