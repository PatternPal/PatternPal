using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace IDesign.Recognizers
{
    public static class MethodChecks
    {
        /// <summary>
        ///     Function thats checks the returntype of a method.
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <param name="returnType">The expected return type</param>
        /// <returns></returns>
        public static bool CheckReturnType(this IMethod methodSyntax, string returnType)
        {
            return methodSyntax.GetReturnType().IsEqual(returnType);
        }

        /// <summary>
        ///     Return a boolean based on if the given method is creational.
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <returns></returns>
        public static bool CheckCreationalFunction(this IMethod methodSyntax)
        {
            return methodSyntax.GetBody().DescendantNodes().OfType<ObjectCreationExpressionSyntax>().Any();
        }

        /// <summary>
        ///     Return a boolean based on if the given member has an expected modifier.
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <param name="modifier">The expected modifier</param>
        /// <returns></returns>
        public static bool CheckModifier(this IMethod methodSyntax, string modifier)
        {
            return methodSyntax.GetModifiers().Any(x => x.ToString().IsEqual(modifier));
        }

        /// <summary>
        ///     Return a boolean based on if the given method creates an object with the given type.
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <param name="creationType">The expected creational type</param>
        /// <returns></returns>
        public static bool CheckCreationType(this IMethod methodSyntax, string creationType)
        {
            if (methodSyntax.GetBody() != null)
            {
                var creations = methodSyntax.GetBody().DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
                foreach (var creationExpression in creations)
                    if (creationExpression.Type is IdentifierNameSyntax name && name.Identifier.ToString().IsEqual(creationType))
                        return true;
            }

            return false;
        }

        /// <summary>
        ///     Return a boolean based on if the given method returns a object it creates.
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <returns></returns>
        public static bool CheckReturnTypeSameAsCreation(this IMethod methodSyntax)
        {
            return methodSyntax.CheckCreationType(methodSyntax.GetReturnType());
        }

        //helper functions
        /// <summary>
        ///     Return al list of all  types that this function makes as strings.
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <returns>all types that are created</returns>
        public static IEnumerable<string> GetCreatedTypes(this IMethod methodSyntax)
        {
            var result = new List<string>();
            var creations = methodSyntax.GetBody().DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
            foreach (var creation in creations)
            {
                var identifiers = creation.DescendantNodes().OfType<IdentifierNameSyntax>();
                result.AddRange(identifiers.Select(y => y.Identifier.ToString()));
            }
            return result;
        }

        /// <summary>
        ///     Function thats checks if the parameters exist.
        /// </summary>
        /// <param name="methodSyntax">The methodsyntax witch it should check</param>
        /// <param name="parameters">The expected parameters</param>
        /// <returns></returns>
        public static bool CheckParameters(this IMethod methodSyntax, IEnumerable<string> parameters)
        {
            return parameters.Any(x => methodSyntax.GetParameterTypes().Any(y => x.Equals(y)));
        }

        /// <summary>
        ///     Function thats checks if the methods exist.
        /// </summary>
        /// <param name="methodSyntax">The methodsyntax witch it should check</param>
        /// <param name="methods">The expected methods</param>
        /// <returns></returns>
        public static bool CheckName(this IMethod methodSyntax, IEnumerable<IMethod> methods)
        {
            return methods.Any(x => x.GetName().Equals(methodSyntax.GetName()));
        }

        /// <summary>
        ///     Function thats checks if the argument exist.
        /// </summary>
        /// <param name="methodSyntax">The methodsyntax witch it should check</param>
        /// <param name="argument">The expected argument</param>
        /// <returns></returns>
        public static bool CheckArguments(this IMethod methodSyntax, string argument)
        {
            var parameters = methodSyntax.GetParameters().Where(y => y.Type.ToString().Equals(argument));

            if (parameters.Count() < 1)
                return false;

            return methodSyntax.GetArguments().Any(x => x.Equals(parameters.First().Identifier.ToString()));
        }
    }
}
