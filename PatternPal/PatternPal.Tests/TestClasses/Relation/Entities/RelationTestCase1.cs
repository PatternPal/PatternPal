namespace PatternPal.Tests.TestClasses.Relation.Entities
{
    public class RelationTestCase1 : ERelationTestCase1, IRelationTestCase1
    {
        public void Create()
        {
            new CRelationTestCase1();
        }
    }

    public interface IRelationTestCase1
    {
    }

    public class ERelationTestCase1
    {
    }

    public class CRelationTestCase1
    {
        public CRelationTestCase1() { }
    }
}
