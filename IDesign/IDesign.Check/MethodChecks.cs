using IDesign.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        /// <returns></returns>
        public static bool CheckReturnType(this IMethod method, string returnType)
        {
            return method.GetReturnType().ToString().IsEqual(returnType);
        }


        /// 
        /// <summary>
        /// Return a boolean based on if the given method is creational
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <returns></returns>
        public static bool CheckCreationalFunction(this IMethod methodSyntax)
        {
            return methodSyntax.GetBody().DescendantNodes().OfType<ObjectCreationExpressionSyntax>().Any();
        }

        /// <summary>
        /// Return a boolean based on if the given member has an expected modifier
        /// </summary>
        /// <param name="membersyntax">The member witch it should check</param>
        /// <param name="modifier">The expected modifier</param>
        /// <returns></returns>
        public static bool CheckMemberModifier(this IMethod method, string modifier)
        {
            return method.GetModifiers().Where(x => x.ToString().IsEqual(modifier)).Any();
        }
    }
}
