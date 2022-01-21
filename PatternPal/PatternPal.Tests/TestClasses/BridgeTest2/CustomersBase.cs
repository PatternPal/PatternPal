using System;
using System.Collections.Generic;
using System.Text;

namespace PatternPal.Tests.TestClasses.BridgeTest2
{
    /// <summary>
    /// The 'Abstraction' class
    /// </summary>
    public class CustomersBase
    {
        private DataObject dataObject;

        public DataObject Data
        {
            set => dataObject = value;
            get => dataObject;
        }

        public virtual void Next()
        {
            dataObject.NextRecord();
        }

        public virtual void Prior()
        {
            dataObject.PriorRecord();
        }

        public virtual void Add(string customer)
        {
            dataObject.AddRecord(customer);
        }

        public virtual void Delete(string customer)
        {
            dataObject.DeleteRecord(customer);
        }

        public virtual void Show()
        {
            dataObject.ShowRecord();
        }

        public virtual void ShowAll()
        {
            dataObject.ShowAllRecords();
        }
    }
}
