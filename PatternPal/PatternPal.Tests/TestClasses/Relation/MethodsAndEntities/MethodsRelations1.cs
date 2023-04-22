using System;
using System.Text;
using System.Threading.Tasks;

namespace PatternPal.Tests.TestClasses.Relation.MethodsAndEntities
{
    public static class MethodsRelations1
    {
        public static void displayID()
        {
            ClassWithMethod cl = new ClassWithMethod(8);

            int increasedID = cl.GetID() + 1;

            Console.WriteLine(increasedID);
        }
    }


    public class ClassWithMethod
    {
        private int ID;
        public ClassWithMethod(int id)
        {
            ID = id;
        }

        public int GetID()
        {
            return ID;
        }
    }
}
