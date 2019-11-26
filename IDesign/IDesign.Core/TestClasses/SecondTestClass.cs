using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Testclasses
/// </summary>
namespace IDesign.Core.TestClasses
{
    class Third
    {

    }
    class SecondTestClass :  Third, IFirstTestClass
    {

        public int Som { get; set; }
        public static string Uitslag { get; set; }
        public string naam { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }



        public SecondTestClass(int s)
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
