using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.BridgeTest1
{
    public class ConcreteImplementationB : IImplementation
    {
        public string OperationImplementation()
        {
            return "ConcreteImplementationA: The result in platform B.\n";
        }
    }
}
