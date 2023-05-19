using Microsoft.CodeAnalysis.CSharp.Syntax;

using PatternPal.SyntaxTree.Abstractions.Entities;
using PatternPal.SyntaxTree.Abstractions.Root;

namespace PatternPal.SyntaxTree.Models.Entities
{
    /// <inheritdoc cref="IInterface"/>
    public class Interface : AbstractEntity, IInterface
    {
        private readonly InterfaceDeclarationSyntax _typeDeclarationSyntax;

        public Interface(InterfaceDeclarationSyntax typeDeclarationSyntax, IEntitiesContainer parent) : base(
            typeDeclarationSyntax, parent
        )
        {
            _typeDeclarationSyntax = typeDeclarationSyntax;
        }

        /// <inheritdoc />
        public override EntityType GetEntityType()
        {
            return EntityType.Interface;
        }
    }
}
