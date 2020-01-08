namespace IDesign.Tests.TestClasses.ObserverTest5
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
