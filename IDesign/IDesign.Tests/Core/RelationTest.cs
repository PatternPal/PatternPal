using System.Linq;
using IDesign.Tests.Utils;
using NUnit.Framework;
using SyntaxTree;
using SyntaxTree.Abstractions;

namespace IDesign.Tests.Core {
    public class RelationTest {
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
        ) {
            var code = FileUtils.FileToString("Relation\\" + filename);
            var NameSpaceNode = "IDesign.Tests.TestClasses.Relation";

            var graph = new SyntaxGraph();
            graph.AddFile(code, filename);
            graph.CreateGraph();

            var nodes = graph.GetAll();

            var interfaceCheck = nodes[NameSpaceNode + "." + baseClass].GetRelations()
                .Any(
                    x => x.GetRelationType() == RelationType.Implements &&
                         x.GetDestination().GetName() == relatedClass
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
        ) {
            var code = FileUtils.FileToString("Relation\\" + filename);
            var NameSpaceNode = "IDesign.Tests.TestClasses.Relation";

            var graph = new SyntaxGraph();
            graph.AddFile(code, filename);
            graph.CreateGraph();

            var nodes = graph.GetAll();

            var extendsCheck = nodes[NameSpaceNode + "." + baseClass].GetRelations()
                .Any(x => x.GetRelationType() == RelationType.Extends && x.GetDestination().GetName() == relatedClass);
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
        ) {
            var code = FileUtils.FileToString("Relation\\" + filename);
            var NameSpaceNode = "IDesign.Tests.TestClasses.Relation";

            var graph = new SyntaxGraph();
            graph.AddFile(code, filename);
            graph.CreateGraph();

            var nodes = graph.GetAll();

            var createCheck = nodes[NameSpaceNode + "." + baseClass].GetRelations()
                .Any(x => x.GetRelationType() == RelationType.Creates && x.GetDestination().GetName() == relatedClass);
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
        ) {
            var code = FileUtils.FileToString("Relation\\" + filename);
            var NameSpaceNode = "IDesign.Tests.TestClasses.Relation";

            var graph = new SyntaxGraph();
            graph.AddFile(code, code);
            graph.CreateGraph();

            var nodes = graph.GetAll();

            var usingCheck = nodes[NameSpaceNode + "." + baseClass].GetRelations()
                .Any(x => x.GetRelationType() == RelationType.Uses && x.GetDestination().GetName() == relatedClass);
            Assert.AreEqual(shouldBeValid, usingCheck);
        }
    }
}
