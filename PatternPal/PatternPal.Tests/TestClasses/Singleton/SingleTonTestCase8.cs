namespace PatternPal.Tests.TestClasses.Singleton
{
    /*This test is a WRONG singleton implementation
     * 1- It has a PUBLIC static variable with its own type
     * 2- It has no public constructors
     * 3- It does have a protected constructor
     * 4- It has a public static method with as return type its own type,
     * 5- which creates an instance of itself when there is none,
     * 6- and returns always that same instance
    */
    internal class SingleTonTestCase8
    {
        public static SingleTonTestCase8 instance;

        protected SingleTonTestCase8()
        {
        }

        public static SingleTonTestCase8 Instance()
        {
            if (instance == null)
            {
                instance = new SingleTonTestCase8();
            }

            return instance;
        }
    }
}
