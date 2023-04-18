using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions.Root;

namespace SyntaxTree.Abstractions
{
    public interface INode
    {
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

    public interface IParameterized : INode
    {
        IEnumerable<TypeSyntax> GetParameters();
    }

    public interface IBodied : INode
    {
        CSharpSyntaxNode GetBody();
    }

    public interface IModified : INode
    {
        /// <summary>
        ///     Gets the modifiers of this node
        /// </summary>
        /// <returns>A list of Modifiers</returns>
        IEnumerable<IModifier> GetModifiers();
    }

    public interface IParent : INode
    {
        /// <summary>
        ///     Get all children
        /// </summary>
        /// <returns>All children of this node</returns>
        IEnumerable<INode> GetChildren();
    }

    /// <summary>
    /// Is needed because net standard 2.0 doesn't support default interface implementations, stupid
    /// </summary>
    public static class ParentExtension
    {
        /// <summary>
        ///     Get all children recursive
        /// </summary>
        /// <returns></returns>
        static IEnumerable<INode> GetAllChildren(this IParent parent)
        {
            return parent
                .GetChildren()
                .SelectMany(child => (child as IParent)?.GetAllChildren().Append(child) ?? new[] { child });
        }
    }

    public interface IChild<out T> where T : INode, IParent
    {
        /// <summary>
        ///     Gets the parent of this node
        /// </summary>
        /// <returns>The parent of this node</returns>
        T GetParent();
    }
}
