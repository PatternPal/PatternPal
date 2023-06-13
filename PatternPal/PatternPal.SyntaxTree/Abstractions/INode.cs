#region

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

using PatternPal.SyntaxTree.Abstractions.Root;

#endregion

namespace PatternPal.SyntaxTree.Abstractions
{
    /// <summary>
    /// Represents a node in the <see cref="SyntaxGraph"/>.
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// Gets the name of the node.
        /// </summary>
        string GetName();

        /// <summary>
        /// Gets the Roslyn syntax of the node.
        /// </summary>
        SyntaxNode GetSyntaxNode();

        /// <summary>
        /// Gets the <see cref="IRoot"/> of the node.
        /// </summary>
        IRoot GetRoot();

        /// <summary>
        /// Gets the location in the source code where this <see cref="INode"/> is defined.
        /// </summary>
        TextSpan GetSourceLocation => GetSyntaxNode().GetLocation().SourceSpan;

        /// <summary>
        /// Indicates that this <see cref="INode"/> serves as a placeholder node. This is only
        /// <see langword="true"/> for the node initially provided by the runner.
        /// </summary>
        bool IsPlaceholder => false;
    }

    /// <summary>
    /// Represents a Node which can have parameters.
    /// </summary>
    /// <example>Methods and constructors.</example>
    public interface IParameterized : INode
    {
        /// <summary>
        /// Returns the parameters of the <see cref="INode"/>.
        /// </summary>
        IEnumerable< TypeSyntax > GetParameters();
    }

    /// <summary>
    /// Represents a Node which has a body.
    /// </summary>
    /// <example>Methods and constructors</example>
    public interface IBodied : INode
    {
        /// <summary>
        /// Returns the body of the <see cref="INode"/>.
        /// </summary>
        CSharpSyntaxNode GetBody();
    }

    /// <summary>
    /// Represents a Node which can have <see cref="IModifier"/>s.
    /// </summary>
    public interface IModified : INode
    {
        /// <summary>
        /// Gets the modifiers of this <see cref="INode"/>.
        /// </summary>
        IEnumerable< IModifier > GetModifiers();
    }

    /// <summary>
    /// Represents a Node which can have children <see cref="INode"/>s.
    /// </summary>
    public interface IParent : INode
    {
        /// <summary>
        /// Get all children nodes from the parent.
        /// </summary>
        IEnumerable< INode > GetChildren();
    }

    /// <summary>
    /// Represents a node which can be a child of another node.
    /// </summary>
    /// <typeparam name="T">The parent of the node</typeparam>
    public interface IChild< out T >
        where T : INode, IParent
    {
        /// <summary>
        /// Gets the parent of this <see cref="INode"/>.
        /// </summary>
        T GetParent();
    }
}
