namespace PatternPal.Tests.TestClasses.Singleton
{
    /*This test is a possible "perfect" singleton implementation.
     * 1- It has a private static variable with its own type
     * 2- It has no public constructors
     * 3- It does have a private constructor
     * 4- It has a public static property with as return type its own type,
     * 5- which creates an instance of itself when there is none,
     * 6- and returns always that same instance
    */
    public class SingleTonTestCase1
    {
        private static SingleTonTestCase1 instance;

        private SingleTonTestCase1()
        {
        }

        public static SingleTonTestCase1 Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SingleTonTestCase1();
                }

                return instance;
            }
        }
    }
}
