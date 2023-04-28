using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SyntaxTree.Abstractions.Members;

namespace PatternPal.Recognizers.Checks
{
    public static class MethodChecks
    {
        public static bool CheckCreationType(this IMethod methodSyntax, string creationType)
        {
            var body = methodSyntax.GetBody();

            if (body == null) return false;

            var creations = body.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();

            return creations
                .Where(
                    creationExpression => creationExpression.Type is IdentifierNameSyntax name &&
                                          name.Identifier.ToString().CheckIfTwoStringsAreEqual(creationType)
                )
                .Select(creationExpression => new { })
                .Any();
        }
    }
}
