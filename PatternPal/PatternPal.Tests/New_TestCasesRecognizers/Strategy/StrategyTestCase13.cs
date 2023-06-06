using System;
using System.Collections.Generic;
using System.Text;

namespace PatternPal.Tests.TestClasses.Strategy
{
    /* Pattern:              Strategy
     * Original code source: None
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
     *               c) if the class is not used it should be used via the context class
     *               d) is stored in the context class
     *         Context
     *            ✓  a) has a private field or property that has a Strategy class as type 
     *            ✓  b) has a function setStrategy() to set the non-public field / property with parameter of type Strategy
     *            ✓  c) has a function useStrategy() to execute the strategy. 
     *         Client
     *            ✓  a) has created an object of the type ConcreteStrategy
     *            ✓  b) has used the setStrategy() in the Context class to store the ConcreteStrategy object
     *            ✓  c) has executed the ConcreteStrategy via the Context class
     */

    //Strategy interface
    file interface TeamInformation
    {
        public void ShowMembers();
        public void TellMeARisk();

        void SayVo()
        {
            Console.WriteLine("Vo!");
        }
    }

    //Concrete strategy
    file class TeamRefactor : TeamInformation
    {
        public void ShowMembers()
        {
            Console.WriteLine("Matteo, Linde and Jeroen");
        }

        public void TellMeARisk()
        {
            Console.WriteLine("Rewriting the code takes too much time");
        }
    }

    //Concrete strategy
    file class TeamLogging : TeamInformation
    {
        public void ShowMembers()
        {
            Console.WriteLine("Olaf, Wing and Siem");
        }

        public void TellMeARisk()
        {
            Console.WriteLine("The logging data could be too big for the server");
        }
    }

    //Concrete strategy
    file class TeamService : TeamInformation
    {
        public void ShowMembers()
        {
            Console.WriteLine("Rutger, Casper and Daan");
        }

        public void TellMeARisk()
        {
            Console.WriteLine("Someone could forget his laptop on the train...");
        }
    }

    //Context
    file class TeamContext
    {
        public void SetStrategy(TeamInformation strategy, TeamInformation _teamInformation)
        {
            _teamInformation = strategy;
        }

        public void TellMeSomething(TeamInformation _teamInformation)
        {
            _teamInformation.ShowMembers();
            _teamInformation.TellMeARisk();
        }

    }

    //Client
    file class Main
    {
        private TeamInformation _teamInformation;

        static void Program(string[] args)
        {
            TeamInformation _teamInformation = new TeamLogging();

            TeamContext strat = new TeamContext();
            strat.TellMeSomething(_teamInformation);
            strat.SetStrategy(new TeamRefactor(), _teamInformation);
            strat.TellMeSomething(_teamInformation);

        }
    }
}
