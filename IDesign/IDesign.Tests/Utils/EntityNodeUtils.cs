using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;
using SyntaxTree.Abstractions.Root;
using SyntaxTree.Models.Members.Field;
using SyntaxTree.Models.Members.Method;
using SyntaxTree.Utils;

namespace IDesign.Tests.Utils {
    public static class EntityNodeUtils {
        /// <summary>
        ///     Return an entitynode based on a TypeDeclarationSyntax.
        /// </summary>
        /// <param name="testNode">TypeDeclarationSyntax that needs to be converted</param>
        /// <returns>The node from the given TypeDeclarationsSyntax</returns>
        public static IEntity CreateTestEntityNode(TypeDeclarationSyntax testNode) {
            //TODO maybe namespace
            return testNode.ToEntity(new TestRoot());
        }

        /// <summary>
        ///     Makes a dictionary of entityNodes based on filecontents
        /// </summary>
        /// <param name="code">contents of the file that needs to be converted</param>
        /// <returns>Dictionary of one file</returns>
        public static Dictionary<string, IEntity> CreateEntityNodeGraphFromOneFile(string code) {
            var graph = new Dictionary<string, IEntity>();
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var nameSpaces = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
            foreach (var nameSpace in nameSpaces) {
                var nameSpaceIdentifier = nameSpace.Name.ToString();
                var nodes = nameSpace.DescendantNodes().OfType<TypeDeclarationSyntax>();
                foreach (var node in nodes)
                    graph.Add(nameSpaceIdentifier + "." + node.Identifier, CreateTestEntityNode(node));
            }

            return graph;
        }

        /// <summary>
        /// Makes a dictionary of entityNodes based on foldercontents
        /// </summary>
        /// <param name="projectCode">contents of the folder/project that needs to be converted</param>
        /// <returns>Dictionary of the whole folder/project</returns>
        public static Dictionary<string, IEntity> CreateEntityNodeGraph(List<string> projectCode) {
            var graph = new Dictionary<string, IEntity>();
            foreach (var fileCode in projectCode) {
                var partialGraph = CreateEntityNodeGraphFromOneFile(fileCode);
                graph = graph.Union(partialGraph).ToDictionary(k => k.Key, v => v.Value);
            }

            return graph;
        }

        public static IMethod CreateMethod(string method) {
            var root = CSharpSyntaxTree.ParseText($"public class Test {{{method}}}").GetCompilationUnitRoot();
            var methodSyntax = root.DescendantNodes(n => true).OfType<MethodDeclarationSyntax>().First();
            if (methodSyntax == null) {
                Assert.Fail();
            }

            return new Method(methodSyntax, null);
        }

        public static IField CreateField(string field) {
            var root = CSharpSyntaxTree.ParseText($"public class Test {{{field}}}").GetCompilationUnitRoot();
            var fieldSyntax = root.DescendantNodes(n => true).OfType<FieldDeclarationSyntax>().First();
            if (fieldSyntax == null) {
                Assert.Fail();
            }

            return new Field(fieldSyntax, null);
        }
    }

    internal class TestRoot : IRoot {
        public string GetName() => "TestRoot";

        public SyntaxNode GetSyntaxNode() => null;

        public IRoot GetRoot() => this;

        public IEnumerable<INamespace> GetNamespaces() => Array.Empty<INamespace>();

        public string GetSource() => "test";

        public IEnumerable<UsingDirectiveSyntax> GetUsing() => Array.Empty<UsingDirectiveSyntax>();

        public IEnumerable<IEntity> GetEntities() => Array.Empty<IEntity>();

        public Dictionary<string, IEntity> GetAllEntities() => new Dictionary<string, IEntity>();

        public IEnumerable<IRelation> GetRelations(IEntity entity) => Array.Empty<IRelation>();
    }
}
