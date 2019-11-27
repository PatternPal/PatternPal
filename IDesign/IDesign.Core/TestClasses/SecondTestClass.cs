using System;

/// <summary>
/// Testclasses
/// </summary>
namespace IDesign.Core.TestClasses
{
    internal class ThirdClass
    {
    }

    internal class SecondTestClass : ThirdClass, IFirstTestClass
    {
        public SecondTestClass(int s)
        {
            Som = s;
        }

        public int Som { get; set; }
        public static string Uitslag { get; set; }

        public string naam
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public int Bereken(int x)
        {
            Som += x;
            return Som;
        }
    }
}