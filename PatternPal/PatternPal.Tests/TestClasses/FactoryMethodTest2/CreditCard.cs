namespace PatternPal.Tests.TestClasses.FactoryMethodTest2
{
    /// <summary>
    ///     The 'Product' Abstract Class
    /// </summary>
    internal abstract class CreditCard
    {
        public abstract string CardType { get; }
        public abstract int CreditLimit { get; set; }
        public abstract int AnnualCharge { get; set; }
    }
}
