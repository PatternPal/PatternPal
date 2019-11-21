﻿using Microsoft.CodeAnalysis;
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
    }
}
