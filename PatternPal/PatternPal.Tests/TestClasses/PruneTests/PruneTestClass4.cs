namespace PatternPal.Tests.TestClasses.PruneTests;
 internal class PruneTestClass4
 {
 }

internal class UsesPrune4
{
    PruneTestClass4 pizza() 
    {
        PruneTestClass4 pizza = new PruneTestClass4();
        return pizza;
    }
}
