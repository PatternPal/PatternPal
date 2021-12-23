using System;
using System.Collections;
using System.Collections.Generic;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers.Models.ElementChecks
{
    public class MethodCheck : IEnumerable<ICheck<IMethod>>
    {
        private readonly List<ICheck<IMethod>> _checks = new List<ICheck<IMethod>>();

        public MethodCheck Modifiers(params IModifier[] modifiers)
        {
            _checks.Add(new ModifierCheck(modifiers));
            return this;
        }

        public MethodCheck ReturnType(IEntity entity)
        {
            return Custom(
                x => x.CheckReturnType(entity.GetName()), new ResourceMessage("MethodReturnType", entity.GetName())
            );
        }

        public MethodCheck Custom(Predicate<IMethod> predicate, ResourceMessage message)
        {
            _checks.Add(
                new ElementCheck<IMethod>(
                    predicate,
                    message
                )
            );
            return this;
        }

        public IEnumerator<ICheck<IMethod>> GetEnumerator() { return _checks.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}
