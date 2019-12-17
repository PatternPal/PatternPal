namespace IDesign.Tests.TestClasses.FactoryMethodTest1
{
    public interface IBeer
    {
        string GetBeer();

        int BatchSize { get; set; }

        double AlcoholPercentage
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }
    }
}