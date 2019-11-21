using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections;
using System.Linq;

namespace IDesign.Checks
{
    public static class MethodChecks
    {
        /// <summary>
        /// Function thats checks the returntype of a method
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <param name="returnType">The expected return type</param>
        /// <returns>
        /// Return a boolean based on if the given method returns the expected type
        /// </returns>
        public static bool CheckReturnType(this MethodDeclarationSyntax methodSyntax, string returnType)
        {
            return methodSyntax.ReturnType.ToString().IsEqual(returnType);
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

        /// <summary>
        /// Return a boolean based on if the given method creates an object with the given type
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <param name="creationType">The expected creational type</param>
        /// <returns></returns>
        /// 
        public static bool CheckCreationType(this MethodDeclarationSyntax methodSyntax, string creationType)
        {
            var creations = methodSyntax.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
            foreach(var creationExpression in creations)
            {
                var name = creationExpression.Type as IdentifierNameSyntax;
                if(name != null && name.Identifier.ToString() == creationType)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return a boolean based on if the given method returns a object it creates
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <returns></returns>
        /// 
        public static bool CheckReturnTypeSameAsCreation(this MethodDeclarationSyntax methodSyntax)
        {
            return methodSyntax.CheckCreationType(methodSyntax.ReturnType.ToString());
        }
    }
}

