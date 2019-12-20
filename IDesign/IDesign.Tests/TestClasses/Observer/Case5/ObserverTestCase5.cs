namespace IDesign.Tests.TestClasses.Observer.Case5
{
    class ObserverTestCase5
    {
        SubjectA subject;

        public ObserverTestCase5()
        {
            subject = new SubjectA();
            subject.Add(new ObserverA());
            subject.Add(new ObserverA());
            subject.Add(new ObserverA());
        }
    }
}
