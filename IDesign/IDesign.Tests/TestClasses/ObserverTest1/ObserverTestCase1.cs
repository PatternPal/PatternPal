using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.ObserverTest1
{
    class ObserverTestCase1
    {
        SubjectA subject;

        public ObserverTestCase1()
        {
            subject = new SubjectA();
            subject.Add(new ObserverA());
            subject.Add(new ObserverA());
            subject.Add(new ObserverA());
        }
    }
}
