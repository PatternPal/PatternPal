namespace PatternPal.Tests.TestClasses.Singleton
{
    /*A somewhat intuitive, yet WRONG implementation of the Singleton.
     * 1- It has no public constructors.
     * 2- It does have a private constructor.
     * 3- It does have a private and static field,
     * 4- yet this field is not of the same type as the class.
     * 5- It does have a public static method,
     * 6- yet this method is not of the same type as the class,
     * 7- thus the implementation is also wrong.
    */
    public class SingleTonTestCase7
    {
        private static int? testInt;

        private SingleTonTestCase7()
        {
        }

        public static int? GetInt()
        {
            if (testInt == null)
            {
                testInt = 5;
            }

            return testInt;
        }
    }
}
