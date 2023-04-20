﻿namespace PatternPal.Tests.TestClasses.Singleton
{
    //This test is a WRONG singleton implementation
    /* Pattern:              Singleton
     * Original code source: -
     * 
     * 
     * Requirements to fullfill the pattern:
     *         Singleton
     *            ✓  a) has no public/internal constructor
     *            ✓  b) has at least one private/protected constructor
     *               c) has a static, private field with the same type as the class
     *            ✓  d) has a static, public/internal method that acts as a constructor in the following way\
     *            ✓        1) if called and there is no instance saved in the private field, then it calls the private constructor
     *            ✓        2) if called and there is an instance saved in the private field it returns this instance
     *         Client
     *               a) the first call of the getInstance() of the Singleton class returns a new instance of this class
     *               b) the second and next calls of getInstance() of the Singleton class return the same instance of the Singleton class.
     */
    internal class SingleTonTestCase08
    {
        public static SingleTonTestCase08 instance;

        protected SingleTonTestCase08()
        {
        }

        public static SingleTonTestCase08 Instance()
        {
            if (instance == null)
            {
                instance = new SingleTonTestCase08();
            }

            return instance;
        }
    }
}
