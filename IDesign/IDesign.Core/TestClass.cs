using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Core
{
    interface IClass
    {

    }
    class TestClass
    {
        public int Getal { get; set; }

        public string s = "shanna";
        public static string Naam { get; set; }

        public TestClass(int g)
        {
            this.Getal = g;
        }

        public int Count(int g)
        {
            Getal += g;
            return Getal;
        }
        static class InnerClass{

        }
    }

    class TestClass2
    {
        public int Som { get; set; }
        public static string Uitslag { get; set; }

        public TestClass2(int s)
        {
            this.Som = s;
        }

        public int Bereken(int x)
        {
            Som += x;
            return Som;
        }
    }

}

class OuterClass
{
    interface IOuterInnerInterface
    {

    }

}

interface IOuterInterface
{

}

