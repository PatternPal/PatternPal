using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Utils;

namespace SyntaxTree.Models.Constructor {
    public class Constructor : IConstructor {
        public Constructor(ConstructorDeclarationSyntax constructor) {
            _constructor = constructor;
        }

        private readonly ConstructorDeclarationSyntax _constructor;

        public string GetName() { return _constructor.Identifier.ToString(); }

        public SyntaxNode GetSyntaxNode() { return _constructor; }

        public IEnumerable<IModifier> GetModifiers() { return _constructor.Modifiers.ToModifiers(); }

        public IEnumerable<TypeSyntax> GetParameters() {
            return _constructor.ParameterList.ToParameters();
        }

        public string GetConstructorType() {
            return _constructor.Identifier.ToString();
        }

        public IEnumerable<string> GetArguments() {
            return _constructor.Initializer?.ArgumentList.Arguments.ToList()
                .Select(x => x.ToString());
        }

        //TODO de
        public IMethod AsMethod() { throw new System.NotImplementedException(); }

        public CSharpSyntaxNode GetBody() { return (CSharpSyntaxNode) _constructor.Body ?? _constructor.ExpressionBody; }
        
        public override string ToString() { return $"{GetName()}()"; }
    }
}
