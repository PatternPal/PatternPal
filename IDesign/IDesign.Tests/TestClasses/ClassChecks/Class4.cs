using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.ClassChecks
{
    public class Class4 : EClass4 { }

    public class EClass4 : IClass4, I1Class4 { }

    public interface IClass4 { }

    public interface I1Class4 { }
}
