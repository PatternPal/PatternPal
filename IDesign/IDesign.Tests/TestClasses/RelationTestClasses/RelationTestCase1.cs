using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses
{
  public class RelationTestCase1 : EReatlionTestCase1, IRelationTestCase1
    {
        public void Create()
        {
            new CRelationTestCase1();
        }
    }
    public interface IRelationTestCase1
    {

    }
    public class EReatlionTestCase1 { }
    public class CRelationTestCase1 { }
}
