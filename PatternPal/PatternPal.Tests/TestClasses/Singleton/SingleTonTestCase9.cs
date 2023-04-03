namespace PatternPal.Tests.TestClasses.Singleton
{
    /*This test is a WRONG singleton implementation.
     * 1- It has a private static variable with its own type
     * 2- It DOES HAVE a public constructor
     * 3- It does have a protected constructor
     * 4- It has a public static method with as return type its own type,
     * 5- which creates an instance of itself when there is none,
     * 6- and returns always that same instance
    */
    internal class SingleTonTestCase9
    {
        private static SingleTonTestCase9 _instance;
        private bool _badlyInstantiated = false;

        public SingleTonTestCase9(bool badlyInstantiated = true)
        {
            _badlyInstantiated = badlyInstantiated;
        }

        protected SingleTonTestCase9()
        {
        }

        public static SingleTonTestCase9 Instance()
        {
            if (_instance == null)
            {
                _instance = new SingleTonTestCase9();
            }

            return _instance;
        }
    }
}
