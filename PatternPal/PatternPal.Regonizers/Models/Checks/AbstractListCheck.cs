using System;
using System.Collections;
using System.Collections.Generic;
using PatternPal.Recognizers.Abstractions;
using PatternPal.Recognizers.Models.ElementChecks;
using PatternPal.Recognizers.Models.Output;
using SyntaxTree.Abstractions;

namespace PatternPal.Recognizers.Models.Checks
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
