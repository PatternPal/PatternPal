using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Testclasses
/// </summary>
namespace IDesign.Core.TestClasses
{
    interface IFirstTestClass
    {
        string naam { get; set; }
    }

    class FirstTestClass : IFirstTestClass
    {

        public int Getal { get; set; }


        public string s = "shanna";
        public static string Naam { get; set; }
        public string naam { get; set; }

        public FirstTestClass(int g)
        {
            this.Getal = g;
        }



        public int Count(int g)
        {
            Getal += g;
            return Getal;
        }

    }

}

