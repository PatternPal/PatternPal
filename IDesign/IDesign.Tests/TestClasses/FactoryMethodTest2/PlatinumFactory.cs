namespace IDesign.Tests.TestClasses.FactoryMethodTest2
{
    internal class PlatinumFactory : CardFactory
    {
        private readonly int _annualCharge;
        private readonly int _creditLimit;

        public PlatinumFactory(int creditLimit, int annualCharge)
        {
            _creditLimit = creditLimit;
            _annualCharge = annualCharge;
        }

        public override CreditCard GetCreditCard()
        {
            return new PlatinumCreditCard(_creditLimit, _annualCharge);
        }
    }
}
