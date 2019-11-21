using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Checks
{
    public static class FieldChecks
    {
        /// <summary>
        /// Return a boolean based on if the given field is an expected type
        /// </summary>
        /// <param name="fieldSyntax">The field witch it should check</param>
        /// <param name="type">The expected type</param>
        /// <returns></returns>
        /// 
        public static bool CheckPropertyType(this FieldDeclarationSyntax fieldSyntax, string type)
        {
            return fieldSyntax.Declaration.Type.ToString() == type;
        }
    }
}
