namespace IDesign.Tests.TestClasses.StateTest2
{
    //this code is from https://www.dofactory.com/net/state-design-pattern

    /// <summary>
    ///     The 'State' abstract class
    /// </summary>
    internal abstract class State
    {
        protected Account account;
        protected double balance;

        protected double interest;
        protected double lowerLimit;
        protected double upperLimit;

        // Properties

        public Account Account
        {
            get => account;
            set => account = value;
        }

        public double Balance
        {
            get => balance;
            set => balance = value;
        }

        public abstract void Deposit(double amount);
        public abstract void Withdraw(double amount);
        public abstract void PayInterest();
    }
}
