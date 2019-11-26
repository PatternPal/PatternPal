using System.Linq;
using IDesign.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Checks
{
    public static class MethodChecks
    {
        /// <summary>
        ///     Function thats checks the returntype of a method
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <param name="returnType">The expected return type</param>
        /// <returns></returns>
        public static bool CheckReturnType(this IMethod methodSyntax, string returnType)
        {
            return methodSyntax.GetReturnType().IsEqual(returnType);
        }


        /// <summary>
        ///     Return a boolean based on if the given method is creational
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <returns></returns>
        public static bool CheckCreationalFunction(this IMethod methodSyntax)
        {
            return methodSyntax.GetBody().DescendantNodes().OfType<ObjectCreationExpressionSyntax>().Any();
        }

        /// <summary>
        ///     Return a boolean based on if the given member has an expected modifier
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <param name="modifier">The expected modifier</param>
        /// <returns></returns>
        public static bool CheckModifier(this IMethod methodSyntax, string modifier)
        {
            return methodSyntax.GetModifiers().Where(x => x.ToString().IsEqual(modifier)).Any();
        }

        /// <summary>
        ///     Return a boolean based on if the given method creates an object with the given type
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <param name="creationType">The expected creational type</param>
        /// <returns></returns>
        public static bool CheckCreationType(this IMethod methodSyntax, string creationType)
        {
            var creations = methodSyntax.GetBody().DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
            foreach (var creationExpression in creations)
            {
                var name = creationExpression.Type as IdentifierNameSyntax;
                if (name != null && name.Identifier.ToString().IsEqual(creationType)) return true;
            }

            return false;
        }

        /// <summary>
        ///     Return a boolean based on if the given method returns a object it creates
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <returns></returns>
        public static bool CheckReturnTypeSameAsCreation(this IMethod methodSyntax)
        {
            return methodSyntax.CheckCreationType(methodSyntax.GetReturnType());
        }
    }
}