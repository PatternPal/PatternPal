using System.IO;
using System.Linq;
using IDesign.Core;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Tests.Utils
{
    public static class EntityNodeUtils
    {
        public static EntityNode CreateTestEntityNode(string code)
        {

            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var nameSpaceNode = root.Members[0] as NamespaceDeclarationSyntax;
            foreach (var testNode in nameSpaceNode.Members.OfType<TypeDeclarationSyntax>())
            {
                var entityNode = new EntityNode
                {
                    Name = testNode.Identifier.ToString(),
                    InterfaceOrClassNode = testNode,

                    MethodDeclarationSyntaxList =
                        testNode.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList(),
                    FieldDeclarationSyntaxList =
                        testNode.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList(),
                    PropertyDeclarationSyntaxList =
                        testNode.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList(),
                    ConstructorDeclarationSyntaxList =
                        testNode.DescendantNodes().OfType<ConstructorDeclarationSyntax>().ToList()
                };
                return entityNode;
            }

            return null;
        }
    }
}