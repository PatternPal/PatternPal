using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;
using SyntaxTree.Utils;

namespace SyntaxTree.Models.Members.Constructor {
    public class Constructor : AbstractNode, IConstructor {
        private readonly ConstructorDeclarationSyntax _constructor;
        private readonly IClass _parent;

        public Constructor(ConstructorDeclarationSyntax node, IClass parent) : base(node, parent.GetRoot()) {
            _constructor = node;
            _parent = parent;
        }

        public override string GetName() => _constructor.Identifier.ToString();

        public IEnumerable<IModifier> GetModifiers() => _constructor.Modifiers.ToModifiers();

        public IEnumerable<TypeSyntax> GetParameters() => _constructor.ParameterList.ToParameters();

        public string GetConstructorType() => _constructor.Identifier.ToString();

        public IEnumerable<string> GetArguments() {
            return _constructor.Initializer?.ArgumentList.Arguments.ToList()
                .Select(x => x.ToString());
        }

        public IMethod AsMethod() => new ConstructorMethod(this);

        public CSharpSyntaxNode GetBody() => (CSharpSyntaxNode) _constructor.Body ?? _constructor.ExpressionBody;

        public IClass GetParent() => _parent;

        public override string ToString() => $"{GetName()}()";
    }
}
