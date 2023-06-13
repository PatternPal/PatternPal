#region

using Microsoft.CodeAnalysis.CSharp;
using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Abstractions.Root;

#endregion

namespace PatternPal.SyntaxTree.Models.Members.Constructor
{
    /// <summary>
    /// An <see cref="IConstructor"/> rapped as a <see cref="IMethod"/>.
    /// </summary>
    public class ConstructorMethod : IMethod
    {
        // The constructor rapped.
        public readonly IConstructor Constructor;

        public ConstructorMethod(IConstructor constructor) { this.Constructor = constructor; }

        /// <inheritdoc />
        public string GetName()
        {
            return Constructor.GetName();
        }

        /// <inheritdoc />
        public SyntaxNode GetSyntaxNode()
        {
            return Constructor.GetSyntaxNode();
        }

        /// <inheritdoc />
        public IRoot GetRoot()
        {
            return Constructor.GetRoot();
        }

        /// <inheritdoc />
        public IEnumerable<IModifier> GetModifiers()
        {
            return Constructor.GetModifiers();
        }

        /// <inheritdoc />
        public IEnumerable<TypeSyntax> GetParameters()
        {
            return Constructor.GetParameters();
        }

        /// <inheritdoc />
        public CSharpSyntaxNode GetBody()
        {
            return Constructor.GetBody();
        }

        /// <inheritdoc />
        public IEntity GetParent()
        {
            return Constructor.GetParent();
        }

        /// <inheritdoc />
        public SyntaxNode GetReturnType()
        {
            return GetParent().GetSyntaxNode();
        }

        public override string ToString()
        {
            return Constructor.ToString();
        }
    }
}
