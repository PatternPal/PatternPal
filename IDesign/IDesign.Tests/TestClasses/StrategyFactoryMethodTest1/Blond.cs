using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.StrategyFactoryMethodTest1
{
    public class Blond : IBeer
    {
        public double AlcoholPercentage { get { return 5.6; } set { } }
        public string Name { get { return "Blond herfstbier"; } set { } }
        public int BatchSize { get; set; }

        public string GetBeer()
        {
            return $"Naam: {Name} \nAlcoholpercentage: {AlcoholPercentage} \nBatchGrootte: {BatchSize}";
        }
    }
}
