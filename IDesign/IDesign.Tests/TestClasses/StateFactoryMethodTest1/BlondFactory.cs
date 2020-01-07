﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.StateFactoryMethodTest1
{
    class BlondFactory : BeerFactory
    {
        public override IBeer BrewBier()
        {
            return new Blond();
        }
    }
}
