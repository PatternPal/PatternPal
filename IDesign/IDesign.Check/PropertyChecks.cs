using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Checks
{
   public static class PropertyChecks
    {
        /// <summary>
        /// Function thats checks the type of a property
        /// </summary>
        /// <param name="propertySyntax">The property witch it should check</param>
        /// <param name="type">The expected type</param>
        /// <returns>
        /// Return a boolean based on if the given property is an expected type
        /// </returns>
        public static bool CheckPropertyType(this PropertyDeclarationSyntax propertySyntax, string type)
        {
            return propertySyntax.Type.ToString() == type;
        }
    }
}
