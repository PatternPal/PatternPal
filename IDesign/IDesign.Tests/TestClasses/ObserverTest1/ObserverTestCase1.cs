namespace IDesign.Tests.TestClasses.ObserverTest1
{
    internal class ObserverTestCase1
    {
        private readonly SubjectA subject;

        public ObserverTestCase1()
        {
            subject = new SubjectA();
            subject.Add(new ObserverA());
            subject.Add(new ObserverA());
            subject.Add(new ObserverA());
        }
    }
}
