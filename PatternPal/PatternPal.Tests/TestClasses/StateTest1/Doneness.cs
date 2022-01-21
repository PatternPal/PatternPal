namespace PatternPal.Tests.TestClasses.StateTest1
{
    //this code is from https://exceptionnotfound.net/state-pattern-in-csharp/

    /// <summary>
    ///     The State abstract class
    /// </summary>
    internal abstract class Doneness
    {
        protected bool canEat;
        protected double currentTemp;
        protected double lowerTemp;
        protected Steak steak;
        protected double upperTemp;

        public Steak Steak
        {
            get => steak;
            set => steak = value;
        }

        public double CurrentTemp
        {
            get => currentTemp;
            set => currentTemp = value;
        }

        public abstract void AddTemp(double temp);
        public abstract void RemoveTemp(double temp);
        public abstract void DonenessCheck();
    }
}
