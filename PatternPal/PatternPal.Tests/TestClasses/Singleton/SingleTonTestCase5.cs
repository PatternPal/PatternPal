namespace PatternPal.Tests.TestClasses.Singleton
{
    /*This test is a possible "perfect" singleton implementation.
     * 1- It has a private static variable with its own type
     * 2- It has no public constructors
     * 3- It does have a private constructor
     * 4- It has a public static method with as return type its own type,
     * 5- which creates an instance of itself when there is none,
     * 6- and returns always that same instance
    */
    public class SingleTonTestCase5
    {
        private static SingleTonTestCase5 _instance;

        private SingleTonTestCase5()
        {
        }

        public static SingleTonTestCase5 GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SingleTonTestCase5();
            }

            return _instance;
        }

        public void DoSomething()
        {

        }
    }
}
