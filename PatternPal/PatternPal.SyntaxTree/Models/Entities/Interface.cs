#region

using PatternPal.SyntaxTree.Abstractions.Root;

#endregion

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
