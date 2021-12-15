namespace IDesign.Tests.TestClasses.FactoryMethodTest2
{
    internal class TitaniumFactory : CardFactory
    {
        private readonly int _annualCharge;
        private readonly int _creditLimit;

        public TitaniumFactory(int creditLimit, int annualCharge)
        {
            _creditLimit = creditLimit;
            _annualCharge = annualCharge;
        }

        public override CreditCard GetCreditCard()
        {
            return new TitaniumCreditCard(_creditLimit, _annualCharge);
        }
    }
}
