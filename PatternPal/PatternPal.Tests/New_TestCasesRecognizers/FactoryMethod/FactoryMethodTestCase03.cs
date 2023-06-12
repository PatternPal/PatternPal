namespace PatternPal.Tests.TestClasses.FactoryMethod
{
    /* Pattern:              Factory method
     * Original code source: None
     *
     * Requirements to fullfill the pattern:
     *         Product
     *            ✓  a) is an interface
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
    public interface IProduct
    {
        string Operation();
    }

    //Concrete product
    internal class ConcreteProduct1 : IProduct
    {
        public string Operation()
        {
            return "{Result of ConcreteProduct1}";
        }
    }

    //Concrete product
    internal class ConcreteProduct2 : IProduct
    {
        public string Operation()
        {
            return "{Result of ConcreteProduct2}";
        }
    }

    //Creator
    internal abstract class Creator
    {
        public abstract IProduct FactoryMethod();

        public string SomeOperation()
        {
            var product = FactoryMethod();
            var result = "Creator: The same creator's code has just worked with "
                         + product.Operation();

            return result;
        }
    }

    //Concrete creator
    internal class ConcreteCreator1 : Creator
    {
        public override IProduct FactoryMethod()
        {
            return new ConcreteProduct1();
        }
    }

    //Concrete creator
    internal class ConcreteCreator2 : Creator
    {
        public override IProduct FactoryMethod()
        {
            return new ConcreteProduct2();
        }
    }
}
