namespace IDesign.Tests.TestClasses.FactoryMethodTest1
{
    public class Dubbel : IBeer
    {
        public double AlcoholPercentage
        {
            get => 6.8;
            set { }
        }

        public string Name
        {
            get => "Dubbel biertje";
            set { }
        }

        public int BatchSize { get; set; }

        public string GetBeer()
        {
            return $"Naam: {Name} \nAlcoholpercentage: {AlcoholPercentage} \nBatch: {BatchSize}";
        }
    }
}
