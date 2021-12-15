namespace IDesign.Tests.TestClasses.StateTest1
{
    //this code is from https://exceptionnotfound.net/state-pattern-in-csharp/

    /// <summary>
    ///     A 'ConcreteState' class.
    /// </summary>
    internal class Rare : Doneness
    {
        public Rare(Doneness state) : this(state.CurrentTemp, state.Steak)
        {
        }

        public Rare(double currentTemp, Steak steak)
        {
            this.currentTemp = currentTemp;
            this.steak = steak;
            Initialize();
        }

        private void Initialize()
        {
            lowerTemp = 130;
            upperTemp = 139.999999999999;
            canEat = true;
        }

        public override void AddTemp(double amount)
        {
            currentTemp += amount;
            DonenessCheck();
        }

        public override void RemoveTemp(double amount)
        {
            currentTemp -= amount;
            DonenessCheck();
        }

        public override void DonenessCheck()
        {
            if (currentTemp < lowerTemp)
            {
                steak.State = new Uncooked(this);
            }
            else if (currentTemp > upperTemp)
            {
                steak.State = new MediumRare(this);
            }
        }
    }
}
