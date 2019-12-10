﻿using Microsoft.CodeAnalysis;
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
            return methodSyntax.GetModifiers().Where(x => x.ToString().IsEqual(modifier)).Any();
        }

        /// <summary>
        ///     Return a boolean based on if the given method creates an object with the given type.
        /// </summary>
        /// <param name="methodSyntax">The method witch it should check</param>
        /// <param name="creationType">The expected creational type</param>
        /// <returns></returns>
        public static bool CheckCreationType(this IMethod methodSyntax, string creationType)
        {
            var creations = methodSyntax.GetBody().DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
            foreach (var creationExpression in creations)
                if (creationExpression.Type is IdentifierNameSyntax name && name.Identifier.ToString().IsEqual(creationType))
                    return true;

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

        public static bool CheckMethodIdentifier(this IMethod methodSyntax, string name)
        {
            return (methodSyntax.GetName().Equals(name) && methodSyntax.GetType() == typeof(Method));
        }

        public static bool CheckMethodModifiers(this IMethod methodSyntax, SyntaxTokenList modifiers)
        {
            return methodSyntax.GetModifiers() == modifiers;
        }

        public static bool CheckMethodParameters(this IMethod methodSyntax, string parameters)
        {
            var result = methodSyntax.GetParameter().ToString().Equals(parameters);
            return methodSyntax.GetParameter().ToString().Equals(parameters);
        }

        public static bool MethodComparing(this IMethod methodSyntax , IMethod compareMethod)
        {
            return (methodSyntax.CheckMethodIdentifier(compareMethod.GetName())
                && methodSyntax.CheckMethodParameters(compareMethod.GetParameter().ToString())
                && methodSyntax.CheckReturnType(compareMethod.GetReturnType()));
        }
    }
}
