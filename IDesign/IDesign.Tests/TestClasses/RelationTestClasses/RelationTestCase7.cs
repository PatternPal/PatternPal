using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.RelationTestClasses
{
    public class RelationTestCase7
    {
        private URelationTestCase7 Using1 {get; set;}
        private U1RelationTestCase7 Using2 { get; set; }
    }
    public class URelationTestCase7 { }
    public class U1RelationTestCase7 { }
}
