using System;
using PatternPal.Recognizers.Models.Output;

// ReSharper disable once CheckNamespace
namespace PatternPal.Recognizers.Models.Checks.Entities
{
    public abstract partial class AbstractMemberCheckBuilder<T, R, C>
    {
        /// <inheritdoc cref="AbstractListCheck{T,R}.Custom"/>
        public R Custom(Predicate<T> predicate, ResourceMessage message)
        {
            Check.Custom(predicate, message);
            return This();
        }
    }
}
