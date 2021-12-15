namespace IDesign.Tests.TestClasses.BridgeTest2
{
    /// <summary>
    ///     The 'Abstraction' class
    /// </summary>
    public class CustomersBase
    {
        public DataObject Data { set; get; }

        public virtual void Next()
        {
            Data.NextRecord();
        }

        public virtual void Prior()
        {
            Data.PriorRecord();
        }

        public virtual void Add(string customer)
        {
            Data.AddRecord(customer);
        }

        public virtual void Delete(string customer)
        {
            Data.DeleteRecord(customer);
        }

        public virtual void Show()
        {
            Data.ShowRecord();
        }

        public virtual void ShowAll()
        {
            Data.ShowAllRecords();
        }
    }
}
