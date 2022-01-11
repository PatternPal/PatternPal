using System;
using System.Collections;
using System.Collections.Generic;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions;

namespace IDesign.Recognizers.Models.Checks
{
    public abstract class AbstractListCheck<T, R> : IEnumerable<ICheck<T>>
        where T : INode where R : AbstractListCheck<T, R>
    {
        protected readonly List<ICheck<T>> _checks = new List<ICheck<T>>();

        public IEnumerator<ICheck<T>> GetEnumerator() { return _checks.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        protected abstract R This();

        public R Custom(Predicate<T> predicate, ResourceMessage message)
        {
            _checks.Add(
                new ElementCheck<T>(
                    predicate,
                    message
                )
            );
            return This();
        }
    }
}
