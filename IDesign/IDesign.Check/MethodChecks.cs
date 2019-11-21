using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace IDesign.Checks
{
    public static class MethodChecks
    {
        /// <summary>
        /// Return a boolean based on if the given method returns the expected type
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <param name="returnType">The expected return type</param>
        /// <returns></returns>
        public static bool CheckReturnType(this MethodDeclarationSyntax methodSyntax, string returnType)
        {
            return methodSyntax.ReturnType.ToString() == returnType;
        }

        /// <summary>
        /// Return a boolean based on if the given method is creational
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <returns></returns>
        public static bool CheckCreationalFunction(this MethodDeclarationSyntax methodSyntax)
        { 
            return methodSyntax.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().Any();
        }
    }
}
