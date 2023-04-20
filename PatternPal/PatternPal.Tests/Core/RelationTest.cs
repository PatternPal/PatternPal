using System.Linq;

using NUnit.Framework;
using SyntaxTree;
using SyntaxTree.Models.Members.Method;

namespace PatternPal.Tests.Core
{
    public class RelationTest
    {
        [Test]
        [TestCase("RelationTestCase1.cs", "RelationTestCase1", "IRelationTestCase1", true)]
        [TestCase("RelationTestCase2.cs", "RelationTestCase2", "IRelationTestCase2", false)]
        [TestCase("RelationTestCase3.cs", "RelationTestCase3", "IRelationTestCase3", true)]
        [TestCase("RelationTestCase4.cs", "RelationTestCase4", "IRelationTestCase4", false)]
        [TestCase("RelationTestCase5.cs", "RelationTestCase5", "IRelationTestCase5", false)]
        [TestCase("RelationTestCase6.cs", "RelationTestCase6", "IRelationTestCase6", false)]
        public void BaseClass_Should_Implement_RelatedInterface(
            string filename,
            string baseClass,
            string relatedClass,
            bool shouldBeValid
        )
        {
            var code = FileUtils.FileToString("Relation\\Entities\\" + filename);
            var NameSpaceNode = "PatternPal.Tests.TestClasses.Relation.Entities";

            var graph = new SyntaxGraph();
            graph.AddFile(code, filename);
            graph.CreateGraph();

            var nodes = graph.GetAll();

            var interfaceCheck = nodes[NameSpaceNode + "." + baseClass].GetRelations(RelationTargetKind.Entity)
                .Any(
                    x => x.GetRelationType() == RelationType.Implements &&
                         x.GetDestinationName() == relatedClass
                );
            Assert.AreEqual(shouldBeValid, interfaceCheck);
        }

        [Test]
        [TestCase("RelationTestCase1.cs", "RelationTestCase1", "ERelationTestCase1", true)]
        [TestCase("RelationTestCase2.cs", "RelationTestCase2", "ERelationTestCase2", false)]
        [TestCase("RelationTestCase3.cs", "RelationTestCase3", "IRelationTestCase3", false)]
        [TestCase("RelationTestCase4.cs", "RelationTestCase4", "ERelationTestCase4", true)]
        [TestCase("RelationTestCase5.cs", "RelationTestCase5", "ERelationTestCase5", false)]
        [TestCase("RelationTestCase6.cs", "RelationTestCase6", "ERelationTestCase6", true)]
        public void BaseClass_Should_Extend_RelatedClass(
            string filename,
            string baseClass,
            string relatedClass,
            bool shouldBeValid
        )
        {
            var code = FileUtils.FileToString("Relation\\Entities\\" + filename);
            var NameSpaceNode = "PatternPal.Tests.TestClasses.Relation.Entities";

            var graph = new SyntaxGraph();
            graph.AddFile(code, filename);
            graph.CreateGraph();

            var nodes = graph.GetAll();

            var extendsCheck = nodes[NameSpaceNode + "." + baseClass].GetRelations(RelationTargetKind.Entity)
                .Any(x => x.GetRelationType() == RelationType.Extends && 
                        x.GetDestinationName() == relatedClass);
            Assert.AreEqual(shouldBeValid, extendsCheck);
        }

        [Test]
        [TestCase("RelationTestCase1.cs", "RelationTestCase1", "CRelationTestCase1", true)]
        [TestCase("RelationTestCase2.cs", "RelationTestCase2", "CRelationTestCase2", true)]
        [TestCase("RelationTestCase3.cs", "RelationTestCase3", "CRelationTestCase2", false)]
        [TestCase("RelationTestCase4.cs", "RelationTestCase4", "CRelationTestCase4", false)]
        [TestCase("RelationTestCase5.cs", "RelationTestCase5", "CRelationTestCase5", false)]
        [TestCase("RelationTestCase6.cs", "RelationTestCase6", "ERelationTestCase6", true)]
        public void BaseClass_Should_Create_RelatedClass(
            string filename,
            string baseClass,
            string relatedClass,
            bool shouldBeValid
        )
        {
            var code = FileUtils.FileToString("Relation\\Entities\\" + filename);
            var NameSpaceNode = "PatternPal.Tests.TestClasses.Relation.Entities";

            var graph = new SyntaxGraph();
            graph.AddFile(code, filename);
            graph.CreateGraph();

            var nodes = graph.GetAll();

            var createCheck = nodes[NameSpaceNode + "." + baseClass].GetRelations(RelationTargetKind.Entity)
                .Any(x => x.GetRelationType() == RelationType.Creates &&
                    x.GetDestinationName() == relatedClass);
            Assert.AreEqual(shouldBeValid, createCheck);
        }

        [Test]
        [TestCase("RelationTestCase1.cs", "RelationTestCase1", "CRelationTestCase1", true)]
        [TestCase("RelationTestCase2.cs", "RelationTestCase2", "CRelationTestCase2", true)]
        [TestCase("RelationTestCase3.cs", "RelationTestCase3", "CRelationTestCase2", false)]
        [TestCase("RelationTestCase4.cs", "RelationTestCase4", "CRelationTestCase4", false)]
        [TestCase("RelationTestCase5.cs", "RelationTestCase5", "CRelationTestCase5", false)]
        [TestCase("RelationTestCase6.cs", "RelationTestCase6", "ERelationTestCase6", true)]
        [TestCase("RelationTestCase7.cs", "RelationTestCase7", "U1RelationTestCase7", true)]
        public void BaseClass_Should_Use_RelatedClass(
            string filename,
            string baseClass,
            string relatedClass,
            bool shouldBeValid
        )
        {
            var code = FileUtils.FileToString("Relation\\Entities\\" + filename);
            var NameSpaceNode = "PatternPal.Tests.TestClasses.Relation.Entities";


            var graph = new SyntaxGraph();
            graph.AddFile(code, code);
            graph.CreateGraph();

            var nodes = graph.GetAll();

            var usingCheck = nodes[NameSpaceNode + "." + baseClass].GetRelations(RelationTargetKind.Entity)
                .Any(x => x.GetRelationType() == RelationType.Uses && 
                          x.GetDestinationName() == relatedClass);
            Assert.AreEqual(shouldBeValid, usingCheck);
        }

        [Test]
        [TestCase("MethodsRelations1.cs", "MethodsRelations1.displayID", false, "ClassWithMethod.GetID", true)]
        [TestCase("MethodsRelations1.cs", "MethodsRelations1", true, "ClassWithMethod.GetID", true)]
        [TestCase("MethodsRelations2.cs", "DoSomeMultiplication", true, "IDoSomething.Multiplication", true)]
        [TestCase("MethodsRelations2.cs", "DoSomeMultiplication.LetsGO", false, "MethodsRelations2.Multiplication", false)]
        [TestCase("MethodsRelations2.cs", "DoSomeMultiplication.LetsGO", false, "MethodsRelations2.Addition", false)]
        public void Entity_Or_Method_Uses_Method(
            string filename,
            string entityOrMethod,
            bool isEntity,
            string method,
            bool shouldBeValid
        )
        {
            string code = FileUtils.FileToString("Relation\\MethodsAndEntities\\" + filename);
            const string NAME_SPACE_NODE = "PatternPal.Tests.TestClasses.Relation.MethodsAndEntities";

            SyntaxGraph graph = new();
            graph.AddFile(code, code);
            graph.CreateGraph();

            //Here the entity or method node is retrieved from the graph
            INode node;

            if (isEntity)
                node = graph.GetAll()[NAME_SPACE_NODE + "." + entityOrMethod];
            else
            {
                string[] splitName = entityOrMethod.Split('.');
                node = graph.GetAll()[NAME_SPACE_NODE + "." + splitName[0]].GetMethods().FirstOrDefault(x => x.GetName() == splitName[1]);
            }

            //Here the method node gets retrieved from the graph
            string[] methodName = method.Split(".");
            Method methodNode = (Method)graph.GetAll()[NAME_SPACE_NODE + "." + methodName[0]].GetMethods()
                .FirstOrDefault(x => x.GetName() == methodName[1]);

            //Checks whether the entity or method node uses the second method node
            bool usingCheck = graph.GetRelations(node, RelationTargetKind.Method)
                .Any(x => x.GetRelationType() == RelationType.Uses && x.Target.AsT1 == methodNode);
            
            //Checks whether the usedBy relation is also in place
            bool usedByCheck = graph.GetRelations(methodNode, isEntity ? RelationTargetKind.Entity : RelationTargetKind.Method)
                .Any(x => x.GetRelationType() == RelationType.UsedBy 
                          && (x.Target.IsT0 ? x.Target.AsT0 == node : x.Target.AsT1 == node));

            Assert.Multiple(() =>
            {
                Assert.AreEqual(shouldBeValid, usingCheck);
                Assert.AreEqual(shouldBeValid, usedByCheck);
            });
        }

        [Test]
        [TestCase("MethodsRelations1.cs", "MethodsRelations1.displayID", "ClassWithMethod", true)]
        [TestCase("MethodsRelations1.cs", "ClassWithMethod.GetID", "MethodsRelations1", false)]
        [TestCase("MethodsRelations2.cs", "DoSomeMultiplication.LetsGO", "IDoSomething", false)]
        [TestCase("MethodsRelations2.cs", "DoSomeMultiplication.LetsGO", "MethodsRelations2", true)]
        [TestCase("MethodsRelations2.cs", "MethodsRelations2.Dividation", "DoSomeMultiplication", true)]
        [TestCase("MethodsRelations2.cs", "MethodsRelations2.Dividation", "MethodsRelations2", false)]
        public void Method_Creates_Entity(
            string filename,
            string method,
            string entity,
            bool shouldBeValid
        )
        {
            string code = FileUtils.FileToString("Relation\\MethodsAndEntities\\" + filename);
            string NameSpaceNode = "PatternPal.Tests.TestClasses.Relation.MethodsAndEntities";

            SyntaxGraph graph = new();
            graph.AddFile(code, code);
            graph.CreateGraph();

            Method methodNode;

            //Here the method node gets retrieved from the graph
            string[] splitName = method.Split('.');
            methodNode = (Method)graph.GetAll()[NameSpaceNode + "." + splitName[0]].GetMethods()
                .FirstOrDefault(x => x.GetName() == splitName[1]);

            //Checks whether the method has a creates relation with the entity
            bool createsCheck = graph.GetRelations(methodNode, RelationTargetKind.Entity).Any(x =>
                x.GetRelationType() == RelationType.Creates && x.GetDestinationName() == entity);

            //Checks whether the entity has a createdby relation with the method
            bool createdByCheck = graph.GetAll()[NameSpaceNode + "." + entity].GetRelations(RelationTargetKind.Method)
                .Any(x => x.GetRelationType() == RelationType.CreatedBy && x.GetDestinationName() == method.Split('.')[1]);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(shouldBeValid, createdByCheck);
                Assert.AreEqual(shouldBeValid, createsCheck);
            });
        }
    }
}
