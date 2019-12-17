using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.FactoryMethodTest1
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
