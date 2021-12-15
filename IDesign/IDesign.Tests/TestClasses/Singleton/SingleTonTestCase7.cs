namespace IDesign.Tests.TestClasses.Singleton
{
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
