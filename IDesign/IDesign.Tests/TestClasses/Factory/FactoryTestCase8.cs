using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.Factory
{
    public class FactoryTestCase8 : IFactoryTestCase8
    {
        public FactoryTestCase8() { }

        public IProduct Create()
        {
            return new ProductA();
        }
        public void Create1()
        {
            new ProductA();
        }
    }

    public interface IFactoryTestCase8
    {
        void Create1();
    }
}
