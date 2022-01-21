using System;

namespace PatternPal.Tests.TestClasses.StrategyFactoryMethodTest1
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
            amount = batchSize switch
            {
                "6" => 6,
                "8" => 8,
                "24" => 24,
                _ => 0
            };
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
