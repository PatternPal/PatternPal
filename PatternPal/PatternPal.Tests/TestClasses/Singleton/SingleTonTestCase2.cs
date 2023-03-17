namespace PatternPal.Tests.TestClasses.Singleton
{
    /* This test is a "close to perfect" implementation using an edge case.
     * 1- It has a static variable with its own type
     * 2- Which is NOT private 
     * 3- It has no public constructors
     * 4- It does have a private constructor
     * 5- It does NOT have a public/internal property or method with its own type.
     * 6- Instead, its field is public and readonly
     * 7- And already instantiated
     * 8- Which guarantees it always is that same instance
     */
    public class SingleTonTestCase2
    {
        public static readonly SingleTonTestCase2 _obj = new SingleTonTestCase2();

        private SingleTonTestCase2()
        {
        }

        
    }
}
