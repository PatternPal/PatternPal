namespace IDesign.Tests.TestClasses.StateFactoryMethodTest1
{
    public class Dubbel : IBeer
    {
        public double AlcoholPercentage { get { return 6.8; } set { } }
        public string Name { get { return "Dubbel biertje"; } set { } }
        public int BatchSize { get; set; }

        public string GetBeer()
        {
            return $"Naam: {Name} \nAlcoholpercentage: {AlcoholPercentage} \nBatch: {BatchSize}";
        }
    }
}