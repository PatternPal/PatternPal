namespace PatternPal.Tests.TestClasses.FactoryMethod
{
    /* Pattern:              Factory method
     * Original code source: None
     *
     * Requirements to fullfill the pattern:
     *         Product
     *               a) is an interface
     *            ✓  b) gets inherited by at least one class
     *            ✓  c) gets inherited by at least two classes
     *         Concrete Product
     *            ✓  a) inherits Product
     *            ✓  b) gets created in a Concrete Creator
     *         Creator
     *            ✓  a) is an abstract class
     *            ✓  b) gets inherited by at least one class 
     *            ✓  c) gets inherited by at least two classes
     *            ✓  d) contains a factory-method with the following properties
     *            ✓        1) method is abstract
     *            ✓        2) method is public
     *            ✓        3) returns an object of type Product
     *         Concrete Creator
     *            ✓  a) inherits Creator
     *            ✓  b) has exactly one method that creates and returns a Concrete product
     */

    //Product
    internal abstract class CreditCard
    {
        public abstract string CardType { get; }
        public abstract int CreditLimit { get; set; }
        public abstract int AnnualCharge { get; set; }
    }
    
    //Concrete product
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

    //Concrete product
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

    //Concrete product
    internal class TitaniumCreditCard : CreditCard
    {
        private int _annualCharge;
        private int _creditLimit;

        public TitaniumCreditCard(int creditLimit, int annualCharge)
        {
            CardType = "Titanium";
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

    //Creator
    internal abstract class CardFactory
    {
        public abstract CreditCard GetCreditCard();
    }

    //Concrete creator
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

    //Concrete creator
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

    //Concrete creator
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
