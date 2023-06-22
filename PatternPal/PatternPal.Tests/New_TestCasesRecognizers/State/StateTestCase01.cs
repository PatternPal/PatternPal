using PatternPal.Tests.TestClasses.StateTest1;

namespace PatternPal.Tests.TestClasses.State
{
    /* Pattern:              State
     * Original code source: https://exceptionnotfound.net/state-pattern-in-csharp/
     *
     * Requirements to fullfill the pattern:
     *       Context
     *          ✓  a) contains a field of type state
     *          ✓  b) contains a changeState() method which changes the state field
     *          ✓  c) contains a method which calls a method of the state field
     *          ✓  d) has either:
     *          ✓        1) a constructor with a state as parameter and sets the concretestrategy field
     *          ✓        2) a constructor which creates and sets the concretestrategy field
     *       State
     *          ✓  a) is an interface
     *          ✓  b) gets inherited by one class
     *          ✓  c) gets inherited by two classes
     *          ✓  d) contains a method
     *       Concrete State
     *          ✓  a) inherits state
     *          ✓  b) contain a field of type context
     *       Client
     *          ✓  a) creates a context object
     *          ✓  b) change the state via the changeState() method
     *          ✓  c) calls method of state via context
     */

    // Abstract class
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

    internal class Medium : Doneness
    {
        public Medium(Doneness state) : this(state.CurrentTemp, state.Steak)
        {
        }

        public Medium(double currentTemp, Steak steak)
        {
            this.currentTemp = currentTemp;
            this.steak = steak;
            canEat = true;
            Initialize();
        }

        private void Initialize()
        {
            lowerTemp = 155;
            upperTemp = 169.9999999999;
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
            if (currentTemp < 130)
            {
                steak.State = new Uncooked(this);
            }
            else if (currentTemp < lowerTemp)
            {
                steak.State = new MediumRare(this);
            }
            else if (currentTemp > upperTemp)
            {
                steak.State = new Ruined(this);
            }
        }
    }

    internal class MediumRare : Doneness
    {
        public MediumRare(Doneness state) : this(state.CurrentTemp, state.Steak)
        {
        }

        public MediumRare(double currentTemp, Steak steak)
        {
            this.currentTemp = currentTemp;
            this.steak = steak;
            canEat = true;
            Initialize();
        }

        private void Initialize()
        {
            lowerTemp = 140;
            upperTemp = 154.9999999999;
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
            if (currentTemp < 0.0)
            {
                steak.State = new Uncooked(this);
            }
            else if (currentTemp < lowerTemp)
            {
                steak.State = new Rare(this);
            }
            else if (currentTemp > upperTemp)
            {
                steak.State = new Medium(this);
            }
        }
    }

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

    internal class Ruined : Doneness
    {
        public Ruined(Doneness state) : this(state.CurrentTemp, state.Steak)
        {
        }

        public Ruined(double currentTemp, Steak steak)
        {
            this.currentTemp = currentTemp;
            this.steak = steak;
            canEat = true;
            Initialize();
        }

        private void Initialize()
        {
            lowerTemp = 170;
            upperTemp = 230;
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
            if (currentTemp < 0)
            {
                steak.State = new Uncooked(this);
            }
            else if (currentTemp < lowerTemp)
            {
                steak.State = new Medium(this);
            }
        }
    }

    internal class Steak
    {
        private string _beefCut;

        public Steak(string beefCut)
        {
            _beefCut = beefCut;
            State = new Rare(0.0, this);
        }

        public double CurrentTemp => State.CurrentTemp;

        public Doneness State { get; set; }

        public void AddTemp(double amount)
        {
            State.AddTemp(amount);
            Console.WriteLine("Increased temperature by {0} degrees.", amount);
            Console.WriteLine(" Current temp is {0}", CurrentTemp);
            Console.WriteLine(" Status is {0}", State.GetType().Name);
            Console.WriteLine("");
        }

        public void RemoveTemp(double amount)
        {
            State.RemoveTemp(amount);
            Console.WriteLine("Decreased temperature by {0} degrees.", amount);
            Console.WriteLine(" Current temp is {0}", CurrentTemp);
            Console.WriteLine(" Status is {0}", State.GetType().Name);
            Console.WriteLine("");
        }
    }

    internal class Uncooked : Doneness
    {
        public Uncooked(Doneness state)
        {
            currentTemp = state.CurrentTemp;
            steak = state.Steak;
            Initialize();
        }

        private void Initialize()
        {
            lowerTemp = 0;
            upperTemp = 130;
            canEat = false;
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
            if (currentTemp > upperTemp)
            {
                steak.State = new Rare(this);
            }
        }
    }

}
