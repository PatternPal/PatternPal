using PatternPal.SyntaxTree.Abstractions.Root;

namespace PatternPal.SyntaxTree.Models.Entities
{
    public class Interface : AbstractEntity, IInterface
    {
        private readonly InterfaceDeclarationSyntax _typeDeclarationSyntax;

        public Interface(InterfaceDeclarationSyntax typeDeclarationSyntax, IEntitiesContainer parent) : base(
            typeDeclarationSyntax, parent
        )
        {
            _typeDeclarationSyntax = typeDeclarationSyntax;
        }

        public override EntityType GetEntityType()
        {
            return EntityType.Interface;
        }
    }
}
