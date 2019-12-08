namespace IDesign.Tests.TestClasses.Singleton
{
    public class SingleTonTestCase3
    {
        private static SingleTonTestCase3 instance = new SingleTonTestCase3();
        public static SingleTonTestCase3 Instance => instance;
    }
}
