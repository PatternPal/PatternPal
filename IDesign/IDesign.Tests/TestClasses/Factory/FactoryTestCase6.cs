using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.Factory
{
    public interface Product { }

    public abstract class Creator
    {
        public void anOperation()
        {
            Product product = factoryMethod();
        }

        protected abstract Product factoryMethod();
    }

    public class ConcreteProduct : Product { }

    public class FactoryTestCase6 : Creator
    {


        protected override Product factoryMethod()
        {
            return new ConcreteProduct();
        }
    }
}
