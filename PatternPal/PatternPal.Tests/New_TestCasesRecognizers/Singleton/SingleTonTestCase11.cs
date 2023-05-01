namespace PatternPal.Tests.TestClasses.Singleton
{
    /* Pattern:              Singleton
     * Original code source: -
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
     *            ✓  a) the first call of the getInstance() of the Singleton class returns a new instance of this class
     *            ✓  b) the second and next calls of getInstance() of the Singleton class return the same instance of the Singleton class.
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
