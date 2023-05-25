using Microsoft.CodeAnalysis;

using PatternPal.SyntaxTree.Abstractions.Root;

namespace PatternPal.Tests.Utils;

internal class RootNode4Tests : INode
{
    string INode.GetName() => "root node";

    SyntaxNode INode.GetSyntaxNode() => throw new UnreachableException();

    IRoot INode.GetRoot() => throw new UnreachableException();
}
