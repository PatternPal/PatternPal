using System.Collections.Generic;
using System.IO;
using System.Linq;
using IDesign.Core;
using IDesign.Core.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Tests.Utils
{
    public static class EntityNodeUtils
    {
        /// <summary>
        /// Return an entitynode based on a TypeDeclarationSyntax.
        /// </summary>
        /// <param name="testNode">TypeDeclarationSyntax that needs to be converted</param>
        /// <returns>The node from the given TypeDeclarationsSyntax</returns>
        public static EntityNode CreateTestEntityNode(TypeDeclarationSyntax testNode)
        {
            var nameSpaceKey = "";
            var nameSpace = testNode.Parent as NamespaceDeclarationSyntax;
            if (nameSpace != null)
            {
                nameSpaceKey += nameSpace.Name.ToString();
            }
            var entityNode = new EntityNode
            {
                Name = testNode.Identifier.ToString(),
                InterfaceOrClassNode = testNode,
                NameSpace = nameSpaceKey,
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

        /// <summary>
        /// Makes a dictionary of entityNodes based on filecontents
        /// </summary>
        /// <param name="code">contents of the file that needs to be converted</param>
        /// <returns>Dictionary of one file</returns>
        public static Dictionary<string, EntityNode> CreateEntityNodeGraphFromOneFile(string code)
        {
            var graph = new Dictionary<string, EntityNode>();
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var nameSpaces = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
            foreach (var nameSpace in nameSpaces)
            {
                var nameSpaceIdentifier = nameSpace.Name.ToString();
                var nodes = nameSpace.DescendantNodes().OfType<TypeDeclarationSyntax>();
                foreach (var node in nodes)
                {
                    graph.Add(nameSpaceIdentifier + "." + node.Identifier.ToString(), CreateTestEntityNode(node));
                }
            }
            return graph;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectCode">contents of the folder/project that needs to be converted</param>
        /// <returns>Dictionary of the whole folder/project</returns>
        public static Dictionary<string, EntityNode> CreateEntityNodeGraph(List<string> projectCode)
        {
            var graph = new Dictionary<string, EntityNode>();
            foreach (var fileCode in projectCode)
            {
                var partialGraph = CreateEntityNodeGraphFromOneFile(fileCode);
                graph = graph.Union(partialGraph).ToDictionary(k => k.Key, v => v.Value);
            }
            return graph;
        }
    }
}