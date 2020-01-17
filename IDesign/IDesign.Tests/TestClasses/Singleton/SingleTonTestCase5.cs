namespace IDesign.Tests.TestClasses.Singleton
{
    public class SingleTonTestCase5
    {
        private static SingleTonTestCase5 _instance;

        private SingleTonTestCase5()
        {
        }

        public static SingleTonTestCase5 GetInstance()
        {
            if (_instance == null) _instance = new SingleTonTestCase5();
            return _instance;
        }
    }
}