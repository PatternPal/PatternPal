namespace PatternPal.Tests.TestClasses.FactoryMethodTest1
{
    public class Blond : IBeer
    {
        public double AlcoholPercentage
        {
            get => 5.6;
            set { }
        }

        public string Name
        {
            get => "Blond herfstbier";
            set { }
        }

        public int BatchSize { get; set; }

        public string GetBeer()
        {
            return $"Naam: {Name} \nAlcoholpercentage: {AlcoholPercentage} \nBatchGrootte: {BatchSize}";
        }
    }
}
