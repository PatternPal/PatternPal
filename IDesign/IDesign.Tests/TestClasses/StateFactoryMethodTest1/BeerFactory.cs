using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.StateFactoryMethodTest1
{
    abstract class BeerFactory
    {
        public abstract IBeer BrewBier();
        string batchSize;
        public int amount;

        public int SetBatch()
        {
            Console.WriteLine("Choose batch Size: 6, 8 or 24");
            batchSize = Console.ReadLine();
            amount = batchSize switch
            {
                "6" => 6,
                "8" => 8,
                "24" => 24,
                _ => 0,
            };
            return amount;
        }

        public string FillBottle()
        {
            if (amount > 0)
            {
                return $"{amount} bottles are getting filled!";
            }
            else
            {
                throw new Exception("Amount not set!");
            }
        }

        public string RinseBottle()
        {
            if (amount > 0)
            {
                return $"{amount} bottle is getting cleaned!";
            }
            else
            {
                throw new Exception("Amount not set!");
            }
        }
    }
}
