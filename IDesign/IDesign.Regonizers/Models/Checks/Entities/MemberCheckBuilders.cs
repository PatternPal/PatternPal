using System;
using System.Collections.Generic;
using IDesign.Recognizers.Models.Checks.Members;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers.Models.Checks.Entities
{
    public abstract partial class AbstractMemberCheckBuilder<T, R, C>
    {
        /// <inheritdoc cref="AbstractMemberListCheck{T,R}.Modifiers"/>
        public R Modifiers(params IModifier[] modifiers)
        {
            Check.Modifiers(modifiers);
            return This();
        }

        /// <inheritdoc cref="AbstractMemberListCheck{T,R}.Type(IEntity)"/>
        public R Type(IEntity entity)
        {
            Check.Type(entity);
            return This();
        }

        /// <inheritdoc cref="AbstractMemberListCheck{T,R}.Type(string)"/>
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

    public class CustomMemberCheckBuilder<N, T> : AbstractMemberCheckBuilder<N, CustomMemberCheckBuilder<N, T>, T>
        where N : IMember
        where T : AbstractMemberListCheck<N, T>
    {
        public CustomMemberCheckBuilder(ICheckBuilder<IEntity> root, T check) : base(root, check)
        {
        }

        protected override CustomMemberCheckBuilder<N, T> This() => this;
    }

    public class MethodCheckBuilder : AbstractMemberCheckBuilder<IMethod, MethodCheckBuilder, MethodCheck>
    {
        private readonly bool _all;

        public MethodCheckBuilder(ICheckBuilder<IEntity> root, bool all = false) : base(root, new MethodCheck())
        {
            _all = all;
        }

        public override IEnumerable<IMember> GetCheckable(IEntity entity)
        {
            return _all ? entity.GetAllMethods() : entity.GetMethods();
        }

        protected override MethodCheckBuilder This() => this;

        /// <inheritdoc cref="MethodCheck.ReturnType(SyntaxTree.Abstractions.Entities.IEntity)"/>
        public MethodCheckBuilder ReturnType(IEntity entity)
        {
            Check.ReturnType(entity);
            return This();
        }

        /// <inheritdoc cref="MethodCheck.ReturnType(string)"/>
        public MethodCheckBuilder ReturnType(string entity)
        {
            Check.ReturnType(entity);
            return This();
        }
    }

    public class FieldCheckBuilder : AbstractMemberCheckBuilder<IField, FieldCheckBuilder, FieldCheck>
    {
        private readonly bool _all;

        public FieldCheckBuilder(ICheckBuilder<IEntity> root, bool all = false) : base(root, new FieldCheck())
        {
            _all = all;
        }

        public override IEnumerable<IMember> GetCheckable(IEntity entity)
        {
            return _all ? entity.GetAllFields() : entity.GetFields();
        }

        protected override FieldCheckBuilder This() => this;

        /// <inheritdoc cref="FieldCheckBuilder.Type(IEntity, bool)"/>
        public FieldCheckBuilder Type(IEntity entity, bool includeGeneric = true)
        {
            Check.Type(entity, includeGeneric);
            return This();
        }

        /// <inheritdoc cref="FieldCheckBuilder.Type(string, bool)"/>
        public FieldCheckBuilder Type(string type, bool includeGeneric = true)
        {
            Check.Type(type, includeGeneric);
            return This();
        }
    }

    public class ConstructorCheckBuilder
        : AbstractMemberCheckBuilder<IConstructor, ConstructorCheckBuilder, ConstructorCheck>
    {
        public ConstructorCheckBuilder(ICheckBuilder<IEntity> root) : base(root, new ConstructorCheck())
        {
        }

        protected override ConstructorCheckBuilder This() => this;
    }

    public class PropertyCheckBuilder : AbstractMemberCheckBuilder<IProperty, PropertyCheckBuilder, PropertyCheck>
    {
        public PropertyCheckBuilder(ICheckBuilder<IEntity> root) : base(root, new PropertyCheck())
        {
        }

        protected override PropertyCheckBuilder This() => this;
    }
}
