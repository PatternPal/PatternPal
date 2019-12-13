using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.FactoryMethodTest2
{
    /// <summary>  
    /// The 'Creator' Abstract Class  
    /// </summary>  
    abstract class CardFactory
    {
        public abstract CreditCard GetCreditCard();
    }
}
