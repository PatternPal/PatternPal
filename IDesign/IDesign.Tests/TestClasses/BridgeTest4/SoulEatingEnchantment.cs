﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.BridgeTest4
{
    public class SoulEatingEnchantment : IEnchantment
    {
        public void OnActivate()
        {
            Console.WriteLine("The item spreads bloodlust.");
        }

        public void Apply()
        {
            Console.WriteLine("The item eats the soul of enemies.");
        }

        public void OnDeactivate()
        {
            Console.WriteLine("Bloodlust slowly disappears.");
        }
    }
}
