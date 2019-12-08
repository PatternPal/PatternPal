namespace IDesign.Tests.TestClasses.Singleton
{
    class SingleTonTestCase4
    {
        private static SingleTonTestCase4 _instance;

        protected SingleTonTestCase4() { }

        public static SingleTonTestCase4 Instance()
        {
            if (_instance == null)
            {
                _instance = new SingleTonTestCase4();
            }
            return _instance;
        }
    }
}
