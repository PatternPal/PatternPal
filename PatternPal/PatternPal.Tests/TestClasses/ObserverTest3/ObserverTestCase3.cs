namespace PatternPal.Tests.TestClasses.ObserverTest3
{
    internal class ObserverTestCase3
    {
        private readonly SubjectA subject;

        public ObserverTestCase3()
        {
            subject = new SubjectA();
            subject.Add(new ObserverA());
            subject.Add(new ObserverA());
            subject.Add(new ObserverA());
        }
    }
}
