using System;

/// <summary>
/// Testclasses
/// </summary>
namespace IDesign.Core.TestClasses
{
    internal class Third { }

    class SecondTestClass : IFirstTestClass
    {
        public SecondTestClass(int s)
        {
            Som = s;
        }

        public int Som { get; set; }
        public static string Uitslag { get; set; }
        public string naam { get; set; }

        public int Bereken(int x)
        {
            Som += x;
            return Som;
        }
    }
}
