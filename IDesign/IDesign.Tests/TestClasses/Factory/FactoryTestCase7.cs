using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.Factory
{
    class FactoryTestCase7 : IFactoryTestCase7
    {
        public FactoryTestCase7() { }

        public IProduct Create(string str)
        {
            return new ProductB();
        }
    }

    public interface IFactoryTestCase7
    {
        IProduct Create(string str);
    }
}
