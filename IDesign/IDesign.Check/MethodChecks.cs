using Microsoft.CodeAnalysis.CSharp.Syntax;

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
    }
}
