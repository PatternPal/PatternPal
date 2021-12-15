namespace IDesign.Tests.TestClasses.FactoryMethodTest2
{
    /// <summary>
    ///     The 'Creator' Abstract Class
    /// </summary>
    internal abstract class CardFactory
    {
        public abstract CreditCard GetCreditCard();
    }
}
