namespace PatternPal.Tests.TestClasses.Singleton
{
    /* This test is a "close to perfect" implementation using an edge case.
     * 1- It has a static variable with its own type
     * 2- Which is NOT private 
     * 3- It has no public constructors (a static constructor cannot be called directly)
     * 5- It does NOT have a public/internal property or method with its own type.
     * 6- Instead, its field is public and readonly
     * 7- And instantiated in the static constructor
     * 8- Which guarantees it always is that same instance
     */
    public class SingleTonTestCase6
    {
        public static readonly SingleTonTestCase6 instance;

        static SingleTonTestCase6()
        {
            instance = new SingleTonTestCase6 ();
        }

        
    }
}
