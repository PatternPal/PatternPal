namespace IDesign.Tests.TestClasses.Singleton
{
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
                if (instance == null) instance = new SingleTonTestCase1();
                return instance;
            }
        }
    }
}