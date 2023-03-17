namespace PatternPal.Tests.TestClasses.Singleton
{
    /*This class uses the SingleTonTestCase5
    */
    public class SingleTonTestCase5User
    {
        SingleTonTestCase5 instance;

        public SingleTonTestCase5User()
        {
            instance = SingleTonTestCase5.GetInstance();
        }

        public void DoSomethingWithSingleton()
        {
            instance.DoSomething();
        }
    }
}
