namespace PatternPal.Tests.TestClasses.Singleton
{
    //This test is a possible "perfect" singleton implementation.
    /* Pattern:              Singleton
     * Original code source: None
     * 
     * 
     * Requirements to fullfill the pattern:
     *         Singleton
     *            ✓  a) has no public/internal constructor
     *            ✓  b) has at least one private/protected constructor
     *            ✓  c) has a static, private field with the same type as the class
     *            ✓  d) has a static, public/internal method that acts as a constructor in the following way
     *            ✓        1) if called and there is no instance saved in the private field, then it calls the private constructor
     *            ✓        2) if called and there is an instance saved in the private field it returns this instance
     *         Client
     *            ✓  a) calls the method that acts as a constructor of the singleton class
     */

    //Singleton
    public class SingleTonTestCase05
    {
        private static SingleTonTestCase05 _instance;

        private SingleTonTestCase05()
        {
        }

        public static SingleTonTestCase05 GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SingleTonTestCase05();
            }

            return _instance;
        }

        public void DoSomething()
        {

        }
    }
    
    //Client
    public class SingleTonTestCase5User
    {
        SingleTonTestCase05 instance;

        public SingleTonTestCase5User()
        {
            instance = SingleTonTestCase05.GetInstance();
        }

        public void DoSomethingWithSingleton()
        {
            instance.DoSomething();
        }
    }
}
