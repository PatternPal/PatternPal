﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.ObserverTest1
{
    interface ISubject
    {
        void Add(IObserver observer);
        void Remove(IObserver observer);
        void Notify();
    }
}
