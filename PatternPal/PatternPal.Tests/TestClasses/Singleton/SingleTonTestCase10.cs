namespace PatternPal.Tests.TestClasses.Singleton
{
    /*This test is a WRONG singleton implementation
     * 1- It has a PUBLIC static variable with its own type
     * 2- It has no public constructors
     * 3- It does have a protected constructor
     * 4- It has a PRIVATE static method with as return type its own type,
     * 5- which creates an instance of itself when there is none,
     * 6- and returns always that same instance
    */
    internal class SingleTonTestCase10
    {
        private static SingleTonTestCase10 _instance;

        protected SingleTonTestCase10()
        {
        }

        private static SingleTonTestCase10 Instance()
        {
            if (_instance == null)
            {
                _instance = new SingleTonTestCase10();
            }

            return _instance;
        }
    }
}

