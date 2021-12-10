using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions.Root;

namespace SyntaxTree.Abstractions {
    public interface INode {
        /// <summary>
        ///     Get the name of the node.
        /// </summary>
        /// <returns>The name of the node</returns>
        string GetName();

        /// <summary>
        ///     Get the syntax of the node.
        /// </summary>
        /// <returns>The declaration syntax of the node</returns>
        SyntaxNode GetSyntaxNode();

        /// <summary>
        ///     Get the root of the node.
        /// </summary>
        /// <returns>The root of the node</returns>
        IRoot GetRoot();
    }

    public interface IParameterized {
        IEnumerable<TypeSyntax> GetParameters();
    }

    public interface IBodied {
        CSharpSyntaxNode GetBody();
    }

    public interface IModified {
        /// <summary>
        ///     Gets the modifiers of this node
        /// </summary>
        /// <returns>A list of Modifiers</returns>
        IEnumerable<IModifier> GetModifiers();
    }

    public interface IChild<out T> where T : INode {
        /// <summary>
        ///     Gets the parent of this node
        /// </summary>
        /// <returns>The parent of this node</returns>
        T GetParent();
    }
}
