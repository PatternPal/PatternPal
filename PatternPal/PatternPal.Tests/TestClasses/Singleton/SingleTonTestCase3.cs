namespace PatternPal.Tests.TestClasses.Singleton
{
    /* This test is a WRONG implementation using an edge case.
     * 1- It has a static variable with its own type
     * 2- Which is NOT private 
     * 3- It does NOT have a private/protected constructor
     * 5- It does NOT have a public/internal property or method with its own type.
     * 6- Instead, its field is a public readonly property
     * 8- Which guarantees it always is that same instance
     */
    public class SingleTonTestCase3
    {
        public static SingleTonTestCase3 Instance { get; } = new SingleTonTestCase3();
    }
}
