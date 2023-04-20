using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

using SyntaxTree.Abstractions.Root;
using SyntaxTree.Models.Members.Field;
using SyntaxTree.Models.Members.Method;
using SyntaxTree.Models.Root;
using SyntaxTree.Utils;

namespace PatternPal.Tests.Utils
{
    public static class EntityNodeUtils
    {
        /// <summary>
        ///     Return an entitynode based on a TypeDeclarationSyntax.
        /// </summary>
        /// <param name="testNode">TypeDeclarationSyntax that needs to be converted</param>
        /// <returns>The node from the given TypeDeclarationsSyntax</returns>
        public static IEntity CreateTestEntityNode(
            TypeDeclarationSyntax testNode)
        {
            //TODO maybe namespace
            return testNode.ToEntity(new TestRoot());
        }

        /// <summary>
        ///     Makes a dictionary of entityNodes based on filecontents
        /// </summary>
        /// <param name="code">contents of the file that needs to be converted</param>
        /// <returns>Dictionary of one file</returns>
        public static Dictionary< string, IEntity > CreateEntityNodeGraphFromOneFile(
            string code)
        {
            Dictionary< string, IEntity > graph = new();
            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            IEnumerable< NamespaceDeclarationSyntax > nameSpaces = root.DescendantNodes().OfType< NamespaceDeclarationSyntax >();
            foreach (NamespaceDeclarationSyntax nameSpace in nameSpaces)
            {
                string nameSpaceIdentifier = nameSpace.Name.ToString();
                IEnumerable< TypeDeclarationSyntax > nodes = nameSpace.DescendantNodes().OfType< TypeDeclarationSyntax >();
                foreach (TypeDeclarationSyntax node in nodes)
                {
                    graph.Add(
                        nameSpaceIdentifier + "." + node.Identifier,
                        CreateTestEntityNode(node));
                }
            }

            return graph;
        }

        /// <summary>
        ///     Makes a dictionary of entityNodes based on foldercontents
        /// </summary>
        /// <param name="projectCode">contents of the folder/project that needs to be converted</param>
        /// <returns>Dictionary of the whole folder/project</returns>
        public static Dictionary< string, IEntity > CreateEntityNodeGraph(
            List< string > projectCode)
        {
            Dictionary< string, IEntity > graph = new();
            foreach (string fileCode in projectCode)
            {
                Dictionary< string, IEntity > partialGraph = CreateEntityNodeGraphFromOneFile(fileCode);
                graph = graph.Union(partialGraph).ToDictionary(
                    k => k.Key,
                    v => v.Value);
            }

            return graph;
        }

        /// <summary>
        /// Creates an <see cref="IClass"/> instance which can be used in tests.
        /// </summary>
        internal static IClass CreateClass() =>
            new Class(
                GetClassDeclaration(),
                new TestRoot());

        internal static INamespace CreateNamespace() =>
            new Namespace(
                GetNamespaceDeclaration(),
                new TestRoot());

        /// <summary>
        /// Creates an <see cref="IMethod"/> instance which can be used in tests.
        /// </summary>
        internal static IMethod CreateMethod()
        {
            ClassDeclarationSyntax classDeclaration = GetClassDeclaration();
            MethodDeclarationSyntax methodSyntax = classDeclaration.DescendantNodes(_ => true).OfType< MethodDeclarationSyntax >().First();
            if (methodSyntax is null)
            {
                Assert.Fail();
            }

            return new Method(
                methodSyntax,
                new Class(
                    classDeclaration,
                    new TestRoot()));
        }

        /// <summary>
        /// Gets the root <see cref="CompilationUnitSyntax"/> from a string of valid C# code.
        /// </summary>
        /// <param name="input">A piece of valid C# code.</param>
        /// <returns>The root <see cref="CompilationUnitSyntax"/> of the C# code provided as input.</returns>
        private static CompilationUnitSyntax GetCompilationRoot(
            string input) => CSharpSyntaxTree.ParseText(input).GetCompilationUnitRoot();

        /// <summary>
        /// Gets a <see cref="ClassDeclarationSyntax"/>.
        /// </summary>
        /// <returns>A <see cref="ClassDeclarationSyntax"/> to be used inside tests.</returns>
        private static ClassDeclarationSyntax GetClassDeclaration()
        {
            const string INPUT = """
                                 public class Test
                                 {
                                     internal void DoSomething()
                                     {
                                     }
                                 }
                                 """;

            CompilationUnitSyntax root = GetCompilationRoot(INPUT);
            MemberDeclarationSyntax rootMember = root.Members.First();

            Assert.IsInstanceOf< ClassDeclarationSyntax >(rootMember);

            return (ClassDeclarationSyntax)rootMember;
        }

        private static NamespaceDeclarationSyntax GetNamespaceDeclaration()
        {
            const string INPUT = """
                                 namespace TestNamespace
                                 {
                                    class Test
                                    {
                                        internal void DoSomething()
                                        {
                                        }
                                    }
                                 }
                                 """;

            CompilationUnitSyntax root = GetCompilationRoot(INPUT);
            MemberDeclarationSyntax rootMember = root.Members.First();

            Assert.IsInstanceOf<NamespaceDeclarationSyntax>(rootMember);

            return (NamespaceDeclarationSyntax)rootMember;
        }

        public static IMethod CreateMethod(
            string method)
        {
            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText($"public class Test {{{method}}}").GetCompilationUnitRoot();
            MethodDeclarationSyntax methodSyntax = root.DescendantNodes(n => true).OfType< MethodDeclarationSyntax >().First();
            if (methodSyntax == null)
            {
                Assert.Fail();
            }

            return new Method(
                methodSyntax,
                null);
        }

        public static IField CreateField(
            string field)
        {
            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText($"public class Test {{{field}}}").GetCompilationUnitRoot();
            FieldDeclarationSyntax fieldSyntax = root.DescendantNodes(n => true).OfType< FieldDeclarationSyntax >().First();
            if (fieldSyntax == null)
            {
                Assert.Fail();
            }

            return new Field(
                fieldSyntax,
                null);
        }
    }

    internal class TestRoot : IRoot
    {
        public string GetName()
        {
            return "TestRoot";
        }

        public SyntaxNode GetSyntaxNode()
        {
            return null;
        }

        public IRoot GetRoot()
        {
            return this;
        }

        public IEnumerable< INamespace > GetNamespaces()
        {
            return Array.Empty< INamespace >();
        }

        public string GetSource()
        {
            return "test";
        }

        public IEnumerable< UsingDirectiveSyntax > GetUsing()
        {
            return Array.Empty< UsingDirectiveSyntax >();
        }

        public IEnumerable< IEntity > GetEntities()
        {
            return Array.Empty< IEntity >();
        }

        public Dictionary< string, IEntity > GetAllEntities()
        {
            return new Dictionary< string, IEntity >();
        }

        public IEnumerable<Relation> GetRelations(INode node, RelationTargetKind type)
        {
            return Array.Empty<Relation>();
        }

        public IEnumerable< INode > GetChildren() { return Array.Empty< INode >(); }
    }
}
