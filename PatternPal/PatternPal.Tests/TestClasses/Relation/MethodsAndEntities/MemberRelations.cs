using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternPal.Tests.TestClasses.Relation.MethodsAndEntities
{
    internal class MemberRelations
    {
        public ClassWithField fieldClass = new ClassWithField();

        public MemberRelations()
        {
            fieldClass.TestFuncionality();
        }

        public void CheckCount()
        {
            fieldClass.GetCount();
        }

        public void DoNothing()
        {

        }
    }

    internal class ClassWithField
    {
        private int _count = 0;
        public void TestFuncionality()
        {
            _count++;
        }

        public int GetCount()
        {
            return _count;
        }
    }
}
