using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Utils;

namespace PatternPal.SyntaxTree.Models.Members.Constructor
{
    /// <inheritdoc cref="IConstructor"/>
    public class Constructor : AbstractNode, IConstructor
    {
        private readonly ConstructorDeclarationSyntax _constructor;
        private readonly IClass _parent;

        public Constructor(ConstructorDeclarationSyntax node, IClass parent) : base(node, parent?.GetRoot())
        {
            _constructor = node;
            _parent = parent;
        }

        /// <inheritdoc />
        public ConstructorDeclarationSyntax GetConstructorDeclarationSyntax()
        {
            return _constructor;
        }

        /// <inheritdoc />
        public override string GetName()
        {
            return _constructor.Identifier.ToString();
        }

        /// <inheritdoc />
        public IEnumerable<IModifier> GetModifiers()
        {
            return _constructor.Modifiers.ToModifiers();
        }

        /// <inheritdoc />
        public IEnumerable<TypeSyntax> GetParameters()
        {
            return _constructor.ParameterList.ToParameters();
        }

        /// <inheritdoc />
        public string GetConstructorType()
        {
            return _constructor.Identifier.ToString();
        }

        /// <inheritdoc />
        public IMethod AsMethod()
        {
            return new ConstructorMethod(this);
        }

        /// <inheritdoc />
        public CSharpSyntaxNode GetBody()
        {
            return (CSharpSyntaxNode)_constructor.Body ?? _constructor.ExpressionBody;
        }

        /// <inheritdoc cref="IConstructor.GetParent"/>
        public IClass GetParent()
        {
            return _parent;
        }

        /// <inheritdoc />
        public SyntaxNode GetReturnType()
        {
            return GetParent().GetSyntaxNode();
        }

        /// <inheritdoc />
        IEntity IChild<IEntity>.GetParent() { return GetParent(); }

        public override string ToString()
        {
            return $"{GetName()}()";
        }
    }
}
