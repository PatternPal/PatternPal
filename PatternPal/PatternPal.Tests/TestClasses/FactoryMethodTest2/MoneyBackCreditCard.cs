namespace PatternPal.Tests.TestClasses.FactoryMethodTest2
{
    /// <summary>
    ///     A 'ConcreteProduct' class
    /// </summary>
    internal class MoneyBackCreditCard : CreditCard
    {
        private int _annualCharge;
        private int _creditLimit;

        public MoneyBackCreditCard(int creditLimit, int annualCharge)
        {
            CardType = "MoneyBack";
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
