namespace PatternPal.Tests.TestClasses.Singleton
{
    /*This test is a possible "perfect" singleton implementation.
     * 1- It has a private static variable with its own type
     * 2- It has no public constructors
     * 3- It does have a protected constructor
     * 4- It has a public static method with as return type its own type,
     * 5- which creates an instance of itself when there is none,
     * 6- and returns always that same instance
    */
    internal class SingleTonTestCase4
    {
        private static SingleTonTestCase4 _instance;

        protected SingleTonTestCase4()
        {
        }

        public static SingleTonTestCase4 Instance()
        {
            if (_instance == null)
            {
                _instance = new SingleTonTestCase4();
            }

            return _instance;
        }
    }
}
