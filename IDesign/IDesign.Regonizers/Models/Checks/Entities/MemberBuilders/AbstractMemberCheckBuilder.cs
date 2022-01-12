using System;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;

// ReSharper disable once CheckNamespace
namespace IDesign.Recognizers.Models.Checks.Entities
{
    public abstract partial class AbstractMemberCheckBuilder<T, R, C>
    {
        /// <inheritdoc cref="SyntaxTree.Models.Modifiers"/>
        public R Modifiers(params IModifier[] modifiers)
        {
            Check.Modifiers(modifiers);
            return This();
        }

        /// <inheritdoc cref="System.Type"/>
        public R Type(IEntity entity)
        {
            Check.Type(entity);
            return This();
        }

        /// <inheritdoc cref="System.Type"/>
        public R Type(string type)
        {
            Check.Type(type);
            return This();
        }

        /// <inheritdoc cref="AbstractListCheck{T,R}.Custom"/>
        public R Custom(Predicate<T> predicate, ResourceMessage message)
        {
            Check.Custom(predicate, message);
            return This();
        }
    }
}
