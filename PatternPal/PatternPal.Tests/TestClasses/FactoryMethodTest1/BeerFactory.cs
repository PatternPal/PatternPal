namespace PatternPal.Tests.TestClasses.FactoryMethodTest1
{
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
}
