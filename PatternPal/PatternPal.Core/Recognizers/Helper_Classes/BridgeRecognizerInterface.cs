#region
using static PatternPal.Core.Checks.CheckBuilder;
#endregion

namespace PatternPal.Core.Recognizers.Helper_Classes
{
    internal class BridgeRecognizerInterface : BridgeRecognizerParent
    {
        /// <inheritdoc />
        public override MethodCheck HasMethod()
        {
            return Method(Priority.High);
        }

        /// <inheritdoc />
        public override InterfaceCheck HasInterfaceOrAbstractClassWithMethod(MethodCheck methodInImplementation)
        {
            return Interface(
                Priority.Knockout,
                methodInImplementation);
        }

        /// <inheritdoc />
        public override ClassCheck ConcreteImplementation(CheckBase implementationCheck, MethodCheck methodInImplementation)
        {
            return Class(
                Priority.High,
                Implements(
                    Priority.High,
                    implementationCheck));
        }
    }
}
