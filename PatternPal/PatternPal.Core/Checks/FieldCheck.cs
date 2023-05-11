using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternPal.SyntaxTree.Abstractions.Members;
namespace PatternPal.Core.Checks;

/// <summary>
/// <see cref="ICheck"/> implementation for <see cref="IField"/> entities.
/// </summary>
internal class FieldCheck : NodeCheck< IField >
{
    /// <inheritdoc cref="NodeCheck{TNode}"/>
    internal FieldCheck(
        Priority priority,
        IEnumerable< ICheck > checks)
        : base(
            priority,
            checks)
    {
    }

    /// <inheritdoc path="//summary|//param" />
    /// <returns>The <see cref="IEntity"/> which represents the type constructed by the <see cref="IField"/>.</returns>
    protected override IEntity? GetType4TypeCheck(
        RecognizerContext ctx,
        IField node)
    {
        if (node.GetFieldType() is IdentifierNameSyntax identifier)
        {
            return ctx.Graph.Relations.GetEntityByName(identifier);
        }

        return null;
    }


    /// <inheritdoc />
    protected override string GetFeedbackMessage(
        IField node) => $"Found field '{node}'";
}
