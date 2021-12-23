using System.Collections;
using System.Collections.Generic;
using IDesign.Recognizers.Abstractions;
using SyntaxTree.Abstractions;

namespace IDesign.Recognizers.Models.Checks
{
    public class AbstractListCheck<T> : IEnumerable<ICheck<T>> where T : INode
    {
        protected readonly List<ICheck<T>> _checks = new List<ICheck<T>>();

        public IEnumerator<ICheck<T>> GetEnumerator() {return _checks.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}
