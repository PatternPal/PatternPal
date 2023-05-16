#region

using Microsoft.CodeAnalysis;

using PatternPal.SyntaxTree;
using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Entities;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Abstractions.Root;
using PatternPal.SyntaxTree.Models.Entities;
using PatternPal.SyntaxTree.Models.Members.Field;
using PatternPal.SyntaxTree.Models.Members.Method;
using PatternPal.SyntaxTree.Models.Members.Property;
using PatternPal.SyntaxTree.Models.Root;
using PatternPal.SyntaxTree.Utils;

#endregion

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
        internal static IClass CreateClass(
            string ? input = null) =>
            new Class(
                GetClassDeclaration(input),
                new TestRoot());

        /// <summary>
        /// Creates an <see cref="IInterface"/> instance which can be used in tests.
        /// </summary>
        internal static IInterface CreateInterface() =>
            new Interface(
                GetInterfaceDeclaration(),
                new TestRoot());

        /// <summary>
        /// Creates a <see cref="INamespace"/> instance which can be used in tests.
        /// </summary>
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
            Assert.IsNotNull(methodSyntax);

            return new Method(
                methodSyntax,
                new Class(
                    classDeclaration,
                    new TestRoot()));
        }

        internal static IConstructor CreateConstructor(
            out SyntaxGraph graph)
        {
            graph = CreateGraphFromInput(
                """
                public class Test
                {
                    public Test()
                    {
                    }
                }
                """);

            IClass classEntity = (IClass)graph.GetAll().Values.First();
            return classEntity.GetConstructors().First();
        }

        /// <summary>
        /// Creates an <see cref="IProperty"/> instance which can be used in tests.
        /// </summary>
        internal static IProperty CreateProperty()
        {
            ClassDeclarationSyntax classDeclaration = GetClassDeclaration();
            PropertyDeclarationSyntax propertySyntax = classDeclaration.DescendantNodes(_ => true).OfType< PropertyDeclarationSyntax >().First();
            Assert.IsNotNull(propertySyntax);

            return new Property(
                propertySyntax,
                new Class(
                    classDeclaration,
                    new TestRoot()));
        }

        /// <summary>
        /// Creates a SyntaxGraph containing a uses relation
        /// </summary>
        /// <returns>A <see cref="SyntaxGraph"/> to be used inside tests.</returns>
        internal static SyntaxGraph CreateUsesRelation()
        {
            const string INPUT = """
                                 public class Uses
                                 {
                                     private Used used = new();

                                     internal void UsesFunction()
                                     {
                                         used.UsedFunction(new Uses(), new Used(), new Used());
                                     }
                                 }
                                 public class Used
                                 {
                                     internal void UsedFunction(Uses testParam1, Used testParam2, Used testParam3)
                                     {
                                     }
                                 }
                                 """;

            return CreateGraphFromInput(INPUT);
        }

        /// <summary>
        /// Creates a SyntaxGraph containing an inheritance relation
        /// </summary>
        /// <returns>A <see cref="SyntaxGraph"/> to be used inside tests.</returns>
        internal static SyntaxGraph CreateInheritanceRelation()
        {
            const string INPUT = """
                                 public class Parent
                                 {                                 
                                 }
                                 public class Child : Parent
                                 {
                                 }
                                 """;

            return CreateGraphFromInput(INPUT);
        }

        /// <summary>
        /// Creates a SyntaxGraph containing an implementation relation
        /// </summary>
        /// <returns>A <see cref="SyntaxGraph"/> to be used inside tests.</returns>
        internal static SyntaxGraph CreateImplementationRelation()
        {
            const string INPUT = """
                                 public interface Parent
                                 {                                 
                                 }
                                 public interface Child : Parent
                                 {
                                 }
                                 """;

            return CreateGraphFromInput(INPUT);
        }

        /// <summary>
        /// Creates a SyntaxGraph from a string representing a file
        /// </summary>
        /// <returns>A <see cref="SyntaxGraph"/> to be used inside tests.</returns>
        private static SyntaxGraph CreateGraphFromInput(
            string input)
        {
            SyntaxGraph graph = new();

            graph.AddFile(
                input,
                "0");
            graph.CreateGraph();

            return graph;
        }

        internal static SyntaxGraph CreateMethodWithParamaters()
        {
            const string INPUT = """
                                 public class StringTest
                                 {
                                    internal StringTest StringTestFunction(IntTest testParam1, IntTest testParam2, StringTest testParam3)
                                    {
                                    }
                                 }
                                 public class IntTest
                                 {
                                    internal IntTest IntTestFunction(StringTest testParam1)
                                    {
                                    }
                                 }
                                 """;

            SyntaxGraph graph = new();
            graph.AddFile(
                INPUT,
                "0");
            graph.CreateGraph();
            return graph;
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
        private static ClassDeclarationSyntax GetClassDeclaration(
            string ? input = null)
        {
            input ??= """
                                 public class Test
                                 {
                                     public Test()
                                     {
                                     }

                                     internal void DoSomething()
                                     {
                                     }

                                     protected int TestProperty { get; set; }
                                 }
                                 """;

            CompilationUnitSyntax root = GetCompilationRoot(input);
            MemberDeclarationSyntax rootMember = root.Members.First();

            Assert.IsInstanceOf< ClassDeclarationSyntax >(rootMember);

            return (ClassDeclarationSyntax)rootMember;
        }

        /// <summary>
        /// Gets a <see cref="ClassDeclarationSyntax"/>.
        /// </summary>
        /// <returns>A <see cref="ClassDeclarationSyntax"/> to be used inside tests.</returns>
        private static InterfaceDeclarationSyntax GetInterfaceDeclaration()
        {
            const string INPUT = """
                                 public interface Test
                                 {
                                     void DoSomething()
                                     {
                                     }

                                     int CalculateSomething();

                                     protected int TestProperty { get; set; }
                                 }
                                 """;

            CompilationUnitSyntax root = GetCompilationRoot(INPUT);
            MemberDeclarationSyntax rootMember = root.Members.First();

            Assert.IsInstanceOf< InterfaceDeclarationSyntax >(rootMember);

            return (InterfaceDeclarationSyntax)rootMember;
        }

        /// <summary>
        /// Gets a <see cref="NamespaceDeclarationSyntax"/>
        /// </summary>
        /// <returns>A <see cref="NamespaceDeclarationSyntax"/> to be used inside tests</returns>
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

            Assert.IsInstanceOf< NamespaceDeclarationSyntax >(rootMember);

            return (NamespaceDeclarationSyntax)rootMember;
        }

        public static IField CreateField()
        {
            const string INPUT = """
                                 public class Test
                                 {
                                     private int _test;
                                 }
                                 """;
            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(INPUT).GetCompilationUnitRoot();
            FieldDeclarationSyntax fieldSyntax = root.DescendantNodes(_ => true).OfType< FieldDeclarationSyntax >().First();
            Assert.IsNotNull(fieldSyntax);

            return new Field(
                fieldSyntax,
                CreateClass(INPUT));
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
            throw new NotImplementedException();
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

        public IEnumerable< Relation > GetRelations(
            INode node,
            RelationTargetKind type)
        {
            return Array.Empty< Relation >();
        }

        public IEnumerable< INode > GetChildren() { return Array.Empty< INode >(); }
    }
}
