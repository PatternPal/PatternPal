using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Checks
{
    public static class MethodChecks
    {
        public static bool CheckReturnType(this MethodDeclarationSyntax methodSyntax, string returnType)
        {
            return methodSyntax.ReturnType.ToString() == returnType;
        }
    }
}
