using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.FactoryMethodTest2
{
    /// <summary>  
    /// A 'ConcreteCreator' class  
    /// </summary>  
    class MoneyBackFactory : CardFactory
    {
        private int _creditLimit;
        private int _annualCharge;

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
