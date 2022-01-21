namespace PatternPal.Tests.TestClasses.ObserverTest5
{
    internal class ObserverTestCase5
    {
        private readonly SubjectA subject;

        public ObserverTestCase5()
        {
            subject = new SubjectA();
            subject.Add(new ObserverA());
            subject.Add(new ObserverA());
            subject.Add(new ObserverA());
        }
    }
}
