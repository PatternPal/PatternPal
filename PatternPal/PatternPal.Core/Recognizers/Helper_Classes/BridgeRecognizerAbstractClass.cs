#region
using PatternPal.SyntaxTree.Models;
using static PatternPal.Core.Checks.CheckBuilder;
#endregion

namespace PatternPal.Core.Recognizers.Helper_Classes
{
    internal class BridgeRecognizerAbstractClass : BridgeRecognizerParent
    {
        /// <inheritdoc />
        public override MethodCheck HasMethod()
        {
            return Method(
                Priority.High,
                Modifiers(
                    Priority.High,
                    Modifier.Abstract
                ));
        }

        /// <inheritdoc />
        public override ClassCheck HasInterfaceOrAbstractClassWithMethod(MethodCheck methodInImplementation)
        {
            return AbstractClass(
                Priority.Knockout,
                methodInImplementation);
        }

        /// <inheritdoc />
        public override ClassCheck ConcreteImplementation(CheckBase implementationCheck, MethodCheck methodInImplementation)
        {
            return Class(
                Priority.High,
                Inherits(
                    Priority.High,
                    implementationCheck),
                Method(
                    Priority.Mid,
                    Overrides(
                        Priority.Mid,
                        methodInImplementation)
                    )
            );
        }
    }
}
