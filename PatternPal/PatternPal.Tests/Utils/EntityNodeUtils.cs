#region

using Microsoft.CodeAnalysis;
using PatternPal.SyntaxTree.Abstractions.Entities;
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
        /// Tries to get a method from a <see cref="SyntaxGraph"/> by matching the method's name to all methods in a specific class
        /// </summary>
        /// <param name="graph">The <see cref="SyntaxGraph"/> in which the method can be found.</param>
        /// <param name="className">The name of the class in which the method resides</param>
        /// <param name="methodName">The name of the method</param>
        /// <returns></returns>
        public static T GetMemberFromGraph<T>(SyntaxGraph graph, string className, string methodName)
        {
            return (T)graph.GetAll()[className].GetMembers().FirstOrDefault(x => x is T && x.GetName() == methodName);
        }

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

        internal static SyntaxGraph CreatePerfectSingleton()
        {
            // SingleTonTestCase05 is a perfect implementation
            const string INPUT = """
                                 public class SingleTonTestCase05
                                 {
                                     private static SingleTonTestCase05 _instance;

                                     private SingleTonTestCase05()
                                     {
                                     }

                                     public static SingleTonTestCase05 GetInstance()
                                     {
                                         if (_instance == null)
                                         {
                                             _instance = new SingleTonTestCase05();
                                         }

                                         return _instance;
                                     }

                                     public void DoSomething()
                                     {

                                     }
                                 }

                                 public class SingleTonTestCase5User
                                 {
                                     SingleTonTestCase05 instance;

                                     public SingleTonTestCase5User()
                                     {
                                         instance = SingleTonTestCase05.GetInstance();
                                     }

                                     public void DoSomethingWithSingleton()
                                     {
                                         instance.DoSomething();
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

        #region singletonTesting
        /// <summary>
        /// contains multiple constructors with different modifiers
        /// </summary>
        internal static SyntaxGraph CreateMultipleConstructors()
        {
            const string INPUT = """
                                 public class MockClass1
                                 {
                                    private MockClass1()
                                    {
                                    }
                                 }

                                 public class MockClass2
                                 {
                                    protected MockClass2()
                                    {
                                    }
                                 }

                                 public class MockClass3
                                 {
                                    public MockClass3()
                                    {
                                    }
                                 }

                                 public class MockClass4
                                 {
                                    internal MockClass4()
                                    {
                                    }
                                 }

                                 public class MockClass5
                                 {
                                    internal MockClass5()
                                    {
                                    }

                                    private MockClass5(int i)
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
        /// contains multiple fields with different modifiers and return types
        /// </summary>
        internal static SyntaxGraph CreateMultipleFields()
        {
            const string INPUT = """
                                 public class MockClass1
                                 {
                                    static private MockClass1 name;
                                 }

                                 public class MockClass2
                                 {
                                    private MockClass2 name;
                                 }

                                 public class MockClass3
                                 {
                                    static protected MockClass3 name;
                                 }

                                 public class MockClass4
                                 {
                                    static private MockClass1 name;
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
        /// contains multiple simplistic singleton examples with different methods
        /// </summary>
        internal static SyntaxGraph CreateMultipleSingletons()
        {
            const string INPUT = """
                                 //Correct
                                 public class MockClass1
                                 {
                                     static private MockClass1 _instance;

                                     private MockClass1()
                                     {
                                     }

                                     public static MockClass1 GetInstance()
                                     {
                                         if (_instance == null)
                                         {
                                             _instance = new MockClass1();
                                         }

                                         return _instance;
                                     }
                                 }
                                 //Wrong method modifiers 1
                                 public class MockClass2
                                 {
                                     static private MockClass2 _instance;

                                     private MockClass2()
                                     {
                                     }

                                     public MockClass2 GetInstance()
                                     {
                                         if (_instance == null)
                                         {
                                             _instance = new MockClass2();
                                         }

                                         return _instance;
                                     }
                                 }

                                 //Wrong method modifiers 2
                                 public class MockClass3
                                 {
                                     static private MockClass3 _instance;

                                     private MockClass3()
                                     {
                                     }

                                     private static MockClass3 GetInstance()
                                     {
                                         if (_instance == null)
                                         {
                                             _instance = new MockClass3();
                                         }

                                         return _instance;
                                     }
                                 }

                                 //Constructor not being called
                                 public class MockClass4
                                 {
                                     static private MockClass4 _instance;

                                     private MockClass4()
                                     {
                                     }

                                     public static MockClass4 GetInstance()
                                     {
                                         if (_instance == null)
                                         {
                                             return _instance;
                                         }

                                         return _instance;
                                     }
                                 }

                                 //does not use private field
                                 public class MockClass5
                                 {
                                     static private MockClass5 _instance;

                                     private MockClass5()
                                     {
                                     }

                                     public static MockClass5 GetInstance()
                                     {
                                         return new MockClass5();
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

        internal static SyntaxGraph CreateCorrectClientSingleton()
        {
            const string INPUT = """
                                 public class MockClass1
                                 {
                                    static private MockClass1 _instance;

                                    private MockClass1()
                                    {
                                    }

                                    public static MockClass1 GetInstance()
                                    {
                                        if (_instance == null)
                                        {
                                            _instance = new MockClass1();
                                        }

                                        return _instance;
                                    }
                                 }

                                 public class Client1
                                 {
                                    public Client1()
                                    {
                                        MockClass1 singleton = MockClass1.GetInstance();
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
        internal static SyntaxGraph CreateWrongClientSingleton()
        {
            const string INPUT = """
                                 public class MockClass2
                                 {
                                     static private MockClass2 _instance;

                                     private MockClass2()
                                     {
                                     }

                                     public static MockClass2 GetInstance()
                                     {
                                         if (_instance == null)
                                         {
                                             _instance = new MockClass2();
                                         }

                                         return _instance;
                                     }
                                 }

                                 public class Client2
                                 {
                                     public Client2()
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
        
        #endregion

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
