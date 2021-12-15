namespace IDesign.Tests.TestClasses.FactoryMethodTest2
{
    /// <summary>
    ///     A 'ConcreteProduct' class
    /// </summary>
    internal class PlatinumCreditCard : CreditCard
    {
        private int _annualCharge;
        private int _creditLimit;

        public PlatinumCreditCard(int creditLimit, int annualCharge)
        {
            CardType = "Platinum";
            _creditLimit = creditLimit;
            _annualCharge = annualCharge;
        }

        public override string CardType { get; }

        public override int CreditLimit
        {
            get => _creditLimit;
            set => _creditLimit = value;
        }

        public override int AnnualCharge
        {
            get => _annualCharge;
            set => _annualCharge = value;
        }
    }
}
