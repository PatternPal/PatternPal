namespace IDesign.Tests.TestClasses.FactoryMethodTest2
{
    /// <summary>
    ///     A 'ConcreteCreator' class
    /// </summary>
    internal class MoneyBackFactory : CardFactory
    {
        private readonly int _annualCharge;
        private readonly int _creditLimit;

        public MoneyBackFactory(int creditLimit, int annualCharge)
        {
            _creditLimit = creditLimit;
            _annualCharge = annualCharge;
        }

        public override CreditCard GetCreditCard()
        {
            return new MoneyBackCreditCard(_creditLimit, _annualCharge);
        }
    }
}
