using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Entities;
using PatternPal.SyntaxTree.Abstractions.Root;
using PatternPal.SyntaxTree.Models;
using PatternPal.SyntaxTree.Models.Entities;

namespace PatternPal.SyntaxTree.Utils
{
    /// <summary>
    /// Used as an adapter to adapt the Roslyn library.
    /// </summary>
    public static class SyntaxUtils
    {
        /// <summary>
        /// Gets the <see cref="Modifier"/>s out of a <see cref="SyntaxTokenList"/>.
        /// </summary>
        public static IEnumerable<IModifier> ToModifiers(this SyntaxTokenList tokens)
        {
            return tokens.Select(s => new SyntaxModifier(s));
        }

        /// <summary>
        /// Gets the parameters out of a <see cref="ParameterListSyntax"/>.
        /// </summary>
        public static IEnumerable<TypeSyntax> ToParameters(this ParameterListSyntax list)
        {
            return list.Parameters.Select(s => s.Type);
        }

        /// <summary>
        /// Transforms a <see cref="TypeDeclarationSyntax"/> into an <see cref="IEntity"/>.
        /// </summary>
        public static IEntity ToEntity(this TypeDeclarationSyntax syntax, IEntitiesContainer parent)
        {
            switch (syntax)
            {
                case ClassDeclarationSyntax cls: return new Class(cls, parent);
                case InterfaceDeclarationSyntax inter: return new Interface(inter, parent);
            }

            Console.Error.WriteLine($"Entity type: {syntax.Kind()} is not yet supported!");
            return null;
        }
    }
}
