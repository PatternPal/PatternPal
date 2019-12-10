using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.StateTest1
{
    //this code is from https://exceptionnotfound.net/state-pattern-in-csharp/

    /// <summary>
    /// The State abstract class
    /// </summary>
    abstract class Doneness
    {
        protected Steak steak;
        protected double currentTemp;
        protected double lowerTemp;
        protected double upperTemp;
        protected bool canEat;

        public Steak Steak
        {
            get { return steak; }
            set { steak = value; }
        }

        public double CurrentTemp
        {
            get { return currentTemp; }
            set { currentTemp = value; }
        }

        public abstract void AddTemp(double temp);
        public abstract void RemoveTemp(double temp);
        public abstract void DonenessCheck();
    }
}
