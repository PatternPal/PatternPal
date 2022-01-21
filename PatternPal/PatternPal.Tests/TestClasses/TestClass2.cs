namespace PatternPal.Tests.TestClasses
{
    internal class TestClass2
    {
        //Should be a field

        //Should be a constructor
        public TestClass2(int g)
        {
            Getal = g;
        }

        //Should be a field 
        public int Getal { get; set; }

        //Should be a field
        public static string Naam { get; set; }

        //Shoud be a field
        public string naam { get; set; }

        //Should be a method and field
        public int PublicProperty { get; set; }

        //Should be method
        public int Count(int g)
        {
            Getal += g;
            return Getal;
        }
    }
}
