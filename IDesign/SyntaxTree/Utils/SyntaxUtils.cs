using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Models;

namespace SyntaxTree.Utils {
    public static class SyntaxUtils {
        public static IEnumerable<IModifier> ToModifiers(this SyntaxTokenList tokens) {
            return tokens.Select(s => new SyntaxModifier(s)).ToList();
        }
        
        public static IEnumerable<TypeSyntax> ToParameters(this ParameterListSyntax list) {
            return list.Parameters.Select(s => s.Type);
        }
    }
}
