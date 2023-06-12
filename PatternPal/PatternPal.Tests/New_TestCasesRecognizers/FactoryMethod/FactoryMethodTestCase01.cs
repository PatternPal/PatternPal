namespace PatternPal.Tests.TestClasses.FactoryMethod
{
    /* Pattern:              Factory method
     * Original code source: None
     *
     * Requirements to fullfill the pattern:
     *         Product
     *            ✓  a) is an interface
     *            ✓  b) gets inherited by at least one class
     *            ✓  c) gets inherited by at least two classes
     *         Concrete Product
     *            ✓  a) inherits Product
     *            ✓  b) gets created in a Concrete Creator
     *         Creator
     *            ✓  a) is an abstract class
     *            ✓  b) gets inherited by at least one class 
     *            ✓  c) gets inherited by at least two classes
     *            ✓  d) contains a factory-method with the following properties
     *            ✓        1) method is abstract
     *            ✓        2) method is public
     *            ✓        3) returns an object of type Product
     *         Concrete Creator
     *            ✓  a) inherits Creator
     *            ✓  b) has exactly one method that creates and returns a Concrete product
     */

    //Product
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

    //Concrete product
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

    //Concrete product
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

    //Creator
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

    //Concrete creator
    internal class BlondFactory : BeerFactory
    {
        public override IBeer BrewBier()
        {
            return new Blond();
        }
    }

    //Concrete creator
    internal class DubbleFactory : BeerFactory
    {
        public override IBeer BrewBier()
        {
            return new Dubbel();
        }
    }
}
