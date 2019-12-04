using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses
{
    internal interface IFirstTestClass
    {
        string naam { get; set; }
    }

    internal class FirstTestClass : IFirstTestClass
    {
        public FirstTestClass(int g)
        {
            Getal = g;
        }

        public int Getal { get; set; }
        public static string Naam { get; set; }
        public string naam { get; set; }

        public int Count(int g)
        {
            Getal += g;
            return Getal;
        }
    }
}
