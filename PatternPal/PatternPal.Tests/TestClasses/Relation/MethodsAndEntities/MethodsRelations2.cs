using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternPal.Tests.TestClasses.Relation.MethodsAndEntities
{
    public interface IDoSomething
    {
        public int Multiplication(int a, int b);
    }

    public class MethodsRelations2 : IDoSomething
    {
        public int Multiplication(int a, int b)
        {
            return a + b;
        }

        public int Addition(int a, int b)
        {
            return a * b;
        }

        public void Dividation()
        {
            DoSomeMultiplication dos = new();

            Console.WriteLine(dos.Division(69));
        }
    }

    public class DoSomeMultiplication
    {
        public static void LetsGO()
        {
            IDoSomething mr2 = new MethodsRelations2();

            int multiplied = mr2.Multiplication(1, 2);

            Console.WriteLine(multiplied);
        }

        public double Division(int a)
        {
            return Math.Sqrt(a);
        }
    }
}
