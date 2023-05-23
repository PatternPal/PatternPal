namespace PatternPal.Tests.TestClasses.PruneTests;
 internal class PruneTestClass4
 {
 }

internal class UsesPrune4
{
    PruneTestClass4 pizza() 
    {
        return new PruneTestClass4();
    }
}
