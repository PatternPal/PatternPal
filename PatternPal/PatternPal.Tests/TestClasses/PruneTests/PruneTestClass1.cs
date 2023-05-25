namespace PatternPal.Tests.TestClasses.PruneTests;

internal class PruneTestClass1
{
    public static int CalculateClient()
    {
        return 1 + 1;
    }
}

internal class ClassUsesPruneClass 
{
    public int pizza = PruneTestClass1.CalculateClient();
}
