using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Entities;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Utils;

namespace PatternPal.SyntaxTree.Models.Members.Constructor
{
    public class Constructor : AbstractNode, IConstructor
    {
        private readonly ConstructorDeclarationSyntax _constructor;
        private readonly IClass _parent;

        public Constructor(ConstructorDeclarationSyntax node, IClass parent) : base(node, parent?.GetRoot())
        {
            _constructor = node;
            _parent = parent;
        }

        public override string GetName()
        {
            return _constructor.Identifier.ToString();
        }

        public IEnumerable<IModifier> GetModifiers()
        {
            return _constructor.Modifiers.ToModifiers();
        }

        public IEnumerable<TypeSyntax> GetParameters()
        {
            return _constructor.ParameterList.ToParameters();
        }

        public string GetConstructorType()
        {
            return _constructor.Identifier.ToString();
        }

        public IEnumerable<string> GetArguments()
        {
            return _constructor.Initializer?.ArgumentList.Arguments.ToList()
                .Select(x => x.ToString());
        }

        public IMethod AsMethod()
        {
            return new ConstructorMethod(this);
        }

        public CSharpSyntaxNode GetBody()
        {
            return (CSharpSyntaxNode)_constructor.Body ?? _constructor.ExpressionBody;
        }

        public IClass GetParent()
        {
            return _parent;
        }

        IEntity IChild<IEntity>.GetParent() { return GetParent(); }

        public override string ToString()
        {
            return $"{GetName()}()";
        }
    }
}
