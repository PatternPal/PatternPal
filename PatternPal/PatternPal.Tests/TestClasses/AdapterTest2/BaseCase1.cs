using System;
using System.Collections.Generic;

namespace PatternPal.Tests.TestClasses.AdapterTest2
{
    /// <summary>
    ///     The 'Client' class
    /// </summary>
    public class ThirdPartyBillingSystem
    {
        private readonly ITarget employeeSource;

        public ThirdPartyBillingSystem(ITarget employeeSource)
        {
            this.employeeSource = employeeSource;
        }

        public void ShowEmployeeList()
        {
            var employee = employeeSource.GetEmployeeList();
            //To DO: Implement you business logic

            Console.WriteLine("######### Employee List ##########");
            foreach (var item in employee)
            {
                Console.Write(item);
            }
        }
    }

    /// <summary>
    ///     The 'ITarget' interface
    /// </summary>
    public interface ITarget
    {
        List<string> GetEmployeeList();
    }

    /// <summary>
    ///     The 'Adaptee' class
    /// </summary>
    public class HRSystem
    {
        public string[][] GetEmployees()
        {
            var employees = new string[4][];

            employees[0] = new[] {"100", "Deepak", "Team Leader"};
            employees[1] = new[] {"101", "Rohit", "Developer"};
            employees[2] = new[] {"102", "Gautam", "Developer"};
            employees[3] = new[] {"103", "Dev", "Tester"};

            return employees;
        }
    }

    /// <summary>
    ///     The 'Adapter' class
    /// </summary>
    public class EmployeeAdapter : HRSystem, ITarget
    {
        public List<string> GetEmployeeList()
        {
            var employeeList = new List<string>();
            var employees = GetEmployees();
            foreach (var employee in employees)
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
    }
}
