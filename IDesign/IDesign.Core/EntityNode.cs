using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace IDesign.Core
{
    public class EntityNode
    {
        public string Name { get; set; }
        public TypeDeclarationSyntax InterfaceOrClassNode { get; set; }
        public List<MethodDeclarationSyntax> MethodDeclarationSyntaxList = new List<MethodDeclarationSyntax>();
        public List<ConstructorDeclarationSyntax> ConstructorDeclarationSyntaxList = new List<ConstructorDeclarationSyntax>();
        public List<PropertyDeclarationSyntax> PropertyDeclarationSyntaxList = new List<PropertyDeclarationSyntax>();
        public List<FieldDeclarationSyntax> FieldDeclarationSyntaxList = new List<FieldDeclarationSyntax>();
        public List<EntityNodeEdges> EntityNodeEdgesList = new List<EntityNodeEdges>();
    }
}
