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