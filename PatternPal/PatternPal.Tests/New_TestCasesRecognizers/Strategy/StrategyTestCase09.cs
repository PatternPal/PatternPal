using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternPal.Tests.TestClasses.Strategy
{
    /* Pattern:              Strategy
     * Original code source: https://scottlilly.com/c-design-patterns-the-strategy-pattern/
     * 
     * 
     * Requirements to fullfill the pattern:
     *         Strategy interface
     *            ✓  a) is an interface	/ abstract class
     *            ✓  b) has declared a method
     *            ✓        1) if the class is an abstract instead of an interface the method has to be an abstract method
     *            ✓  c) is used by another class
     *            ✓  d) is implemented / inherited by at least one other class
     *            ✓  e) is implemented / inherited by at least two other classes
     *         Concrete strategy
     *            ✓  a) is an implementation of the Strategy interface
     *            ✓  b) if the class is used, it must be used via the context class
     *            ✓  c) if the class is not used it should be used via the context class
     *               d) is stored in the context class
     *         Context
     *            ✓  a) has a private field or property that has a Strategy class as type 
     *               b) has a function setStrategy() to set the non-public field / property with parameter of type Strategy
     *            ✓  c) has a function useStrategy() to execute the strategy. 
     *         Client
     *            ✓  a) has created an object of the type ConcreteStrategy
     *               b) has used the setStrategy() in the Context class to store the ConcreteStrategy object
     *            ✓  c) has executed the ConcreteStrategy via the Context class
     */

    //Strategy interface
    public interface IAveragingMethod
    {
        double AverageFor(List<double> values);
    }

    //Concrete strategy
    public class AverageByMean : IAveragingMethod
    {
        public double AverageFor(List<double> values)
        {
            // Simple method to calculate average: 
            // sum of all values, divided by number of values.
            return values.Sum() / values.Count;
        }
    }

    //Concrete strategy
    public class AverageByMedian : IAveragingMethod
    {
        public double AverageFor(List<double> values)
        {
            // Median average is the middle value of the values in the list.
            var sortedValues = values.OrderBy(x => x).ToList();
            // Use the "%" (modulus) to determine if there is an even, or odd, number of values.
            if (sortedValues.Count % 2 == 1)
            {
                // Number of values is odd.
                // Return the middle value of the sorted list.
                // REMEMBER: The list's index is zero-based, so we subtract 1, instead of adding 1,
                //           to determine which element of the list to return
                return sortedValues[(sortedValues.Count - 1) / 2];
            }
            // Number of values is even.
            // Return the mean average of the two middle values.
            // REMEMBER: The list's index is zero-based, so we subtract 1, instead of adding 1,
            //           to determine which elements of the list to use
            return (sortedValues[(sortedValues.Count / 2) - 1] +
                    sortedValues[sortedValues.Count / 2]) / 2;
        }
    }

    //Context
    public class Calculator
    {
        private IAveragingMethod _strategyUnused;
        public double CalculateAverageFor(List<double> values, IAveragingMethod averagingMethod)
        {
            return averagingMethod.AverageFor(values);
        }
    }

    //Client
    public class TestCalculator
    {
        private readonly List<double> _values = new List<double> { 10, 5, 7, 15, 13, 12, 8, 7, 4, 2, 9 };

        public void Test_AverageByMean()
        {
            Calculator calculator = new Calculator();
            var averageByMean = calculator.CalculateAverageFor(_values, new AverageByMean());
        }

        public void Test_AverageByMedian()
        {
            Calculator calculator = new Calculator();
            var averageByMedian = calculator.CalculateAverageFor(_values, new AverageByMedian());
        }
    }
}

