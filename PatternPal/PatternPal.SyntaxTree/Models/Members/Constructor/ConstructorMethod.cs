using Microsoft.CodeAnalysis.CSharp;
using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Abstractions.Root;

namespace PatternPal.SyntaxTree.Models.Members.Constructor
{
    /// <summary>
    /// An <see cref="IConstructor"/> rapped as a <see cref="IMethod"/>.
    /// </summary>
    public class ConstructorMethod : IMethod
    {
        // The constructor rapped.
        public readonly IConstructor constructor;

        public ConstructorMethod(IConstructor constructor) { this.constructor = constructor; }

        /// <inheritdoc />
        public string GetName()
        {
            return constructor.GetName();
        }

        /// <inheritdoc />
        public SyntaxNode GetSyntaxNode()
        {
            return constructor.GetSyntaxNode();
        }

        /// <inheritdoc />
        public IRoot GetRoot()
        {
            return constructor.GetRoot();
        }

        /// <inheritdoc />
        public IEnumerable<IModifier> GetModifiers()
        {
            return constructor.GetModifiers();
        }

        /// <inheritdoc />
        public IEnumerable<TypeSyntax> GetParameters()
        {
            return constructor.GetParameters();
        }

        /// <inheritdoc />
        public CSharpSyntaxNode GetBody()
        {
            return constructor.GetBody();
        }

        /// <inheritdoc />
        public IEntity GetParent()
        {
            return constructor.GetParent();
        }

        /// <inheritdoc />
        public SyntaxNode GetReturnType()
        {
            return GetParent().GetSyntaxNode();
        }

        public override string ToString()
        {
            return constructor.ToString();
        }
    }
}
