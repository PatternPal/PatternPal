﻿namespace PatternPal.Tests.TestClasses.StateFactoryMethodTest1
{
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
