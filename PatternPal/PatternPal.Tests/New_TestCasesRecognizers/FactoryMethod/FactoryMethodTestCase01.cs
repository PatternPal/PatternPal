namespace PatternPal.Tests.TestClasses.FactoryMethod
{
    // This test is a "close to perfect" implementation using an edge case.
    /* Pattern:              Singleton
    * Original code source: None
    * 
    * 
    */
    internal abstract class BeerFactory
    {
        public int amount;
        private string batchSize;
        public abstract IBeer BrewBier();

        public int SetBatch()
        {
            Console.WriteLine("Choose batch Size: 6, 8 or 24");
            batchSize = Console.ReadLine();
            switch (batchSize)
            {
                case "6":
                    amount = 6;
                    break;
                case "8":
                    amount = 8;
                    break;
                case "24":
                    amount = 24;
                    break;
                default:
                    amount = 0;
                    break;
            }

            return amount;
        }

        public string FillBottle()
        {
            if (amount > 0)
            {
                return $"{amount} bottles are getting filled!";
            }

            throw new Exception("Amount not set!");
        }

        public string RinseBottle()
        {
            if (amount > 0)
            {
                return $"{amount} bottle is getting cleaned!";
            }

            throw new Exception("Amount not set!");
        }
    }

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

    internal class BlondFactory : BeerFactory
    {
        public override IBeer BrewBier()
        {
            return new Blond();
        }
    }

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

    //Context class
    internal class DubbleFactory : BeerFactory
    {
        public override IBeer BrewBier()
        {
            return new Dubbel();
        }
    }

    public interface IBeer
    {
        int BatchSize { get; set; }

        double AlcoholPercentage
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }

        string GetBeer();
    }
}
