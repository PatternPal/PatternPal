﻿namespace PatternPal.Tests.TestClasses.Adapter
{
    //This test is a minimal implementation of the adapter. It only contains the knockouts.
    /* Pattern:              Adapter
     * Original code source: https://www.dotnettricks.com/learn/designpatterns/adapter-design-pattern-dotnet
     *
     *
     * Requirements to fullfill the pattern:
     *         Service
     *            ✓  a) does not inherit from the Client Interface
     *               b) is used by the Adapter class
     *         Client
     *               a) has created an object of the type Adapter
     *               b) has used a method of the Service via the Adapter
     *         Client interface
     *            ✓  a) is an interface/abstract class
     *            ✓  b) is inherited/implemented by an Adapter
     *               c) contains a method
     *                     1) if it is an abstract class the method should be abstract
     *         Adapter
     *            ✓  a) inherits/implements the Client Interface
     *            ✓  b) creates an Service object or gets one via the constructor
     *               c) contains a private field in which the Service is stored
     *               d) does not return an instance of the Service
     *               e) a method uses the Service class
     */

    //Service
    file class HRSystem
    {
        public string[][] GetEmployees()
        {
            var employees = new string[4][];

            employees[0] = new[] { "100", "Deepak", "Team Leader" };
            employees[1] = new[] { "101", "Rohit", "Developer" };
            employees[2] = new[] { "102", "Gautam", "Developer" };
            employees[3] = new[] { "103", "Dev", "Tester" };

            return employees;
        }
    }

    //Client interface
    file interface ITarget
    {

    }

    //Adapter
    file class EmployeeAdapter : ITarget
    {
        public HRSystem service = new();

        public List<string> GetEmployeeList()
        {
            List<string> employeeList = new();
            string[][] employees = Array.Empty<string[]>();
            foreach (string[] employee in employees)
            {
                employeeList.Add(employee[0]);
                employeeList.Add(",");
                employeeList.Add(employee[1]);
                employeeList.Add(",");
                employeeList.Add(employee[2]);
                employeeList.Add("\n");
            }

            return employeeList;
        }

        HRSystem returnServiceInstance()
        {
            return service;
        }
    }
}
