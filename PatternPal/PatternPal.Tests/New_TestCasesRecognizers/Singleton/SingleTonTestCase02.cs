namespace PatternPal.Tests.TestClasses.Singleton
{
    // This test is a "close to perfect" implementation using an edge case.
    /* Pattern:              Singleton
    * Original code source: None
    * 
    * 
    * Requirements to fullfill the pattern:
    *         Singleton
    *            ✓  a) has no public/internal constructor
    *            ✓  b) has at least one private/protected constructor
    *            ✓  c) has a static, private field with the same type as the class
    *               d) has a static, public/internal method that acts as a constructor in the following way
    *                     1) if called and there is no instance saved in the private field, then it calls the private constructor
    *                     2) if called and there is an instance saved in the private field it returns this instance
    *         Client
    *               a) calls the method that acts as a constructor of the singleton class
    */
    public class SingleTonTestCase02
    {
        public static readonly SingleTonTestCase02 _obj = new SingleTonTestCase02();

        private SingleTonTestCase02()
        {
        }

        
    }
}
