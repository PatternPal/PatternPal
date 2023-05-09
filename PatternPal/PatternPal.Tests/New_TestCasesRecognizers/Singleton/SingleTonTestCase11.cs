namespace PatternPal.Tests.TestClasses.Singleton
{
    /* Pattern:              Singleton
     * Original code source: None
     * 
     * 
     * Requirements to fullfill the pattern:
     *         Singleton
     *            ✓  a) has no public/internal constructor
     *            ✓  b) has at least one private/protected constructor
     *            ✓  c) has a static, private field with the same type as the class
     *            ✓  d) has a static, public/internal method that acts as a constructor in the following way\
     *            ✓        1) if called and there is no instance saved in the private field, then it calls the private constructor
     *            ✓        2) if called and there is an instance saved in the private field it returns this instance
     *         Client
     *            ✓  a) calls the getInstance() method of the singleton class
     */

    //Singleton
    file class SingleTon
    {
        private static SingleTon _instance;

        private SingleTon()
        {
        }

        public static SingleTon Instance()
        {
            _instance ??= new SingleTon();

            return _instance;
        }
    }

    //Client
    file class SingleTonClient
    {
        SingleTonClient()
        {
            SingleTon.Instance();
        }
    }
}
