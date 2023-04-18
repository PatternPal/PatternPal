using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Root;
using SyntaxTree.Models;
using SyntaxTree.Models.Entities;

namespace SyntaxTree.Utils
{
    public static class SyntaxUtils
    {
        public static IEnumerable<IModifier> ToModifiers(this SyntaxTokenList tokens)
        {
            return tokens.Select(s => new SyntaxModifier(s));
        }

        public static IEnumerable<TypeSyntax> ToParameters(this ParameterListSyntax list)
        {
            return list.Parameters.Select(s => s.Type);
        }

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
