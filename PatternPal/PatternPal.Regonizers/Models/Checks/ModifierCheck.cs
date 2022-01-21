using System;
using System.Linq;
using PatternPal.Recognizers.Abstractions;
using PatternPal.Recognizers.Models.Output;
using SyntaxTree.Abstractions;
using static PatternPal.Recognizers.Abstractions.FeedbackType;

namespace PatternPal.Recognizers.Models.Checks
{
    public class ModifierCheck : ICheck<IModified>
    {
        private readonly IResourceMessage _modifiersFeedback;
        private readonly IModifier[] _modifiers;

        public ModifierCheck(params IModifier[] modifiers)
        {
            _modifiers = modifiers;
            _modifiersFeedback = new ResourceMessage(
                "Modifier",
                "Node", 
                String.Join(", ", _modifiers.Select(m => m.GetName()))
            );
        }

        private bool CheckModifiers(IModified modified)
        {
            return _modifiers.All(modifier => modified.GetModifiers().Any(modifier.Equals));
        }

        public ICheckResult Check(IModified modified)
        {
            if (modified == null) return new CheckResult(_modifiersFeedback, Incorrect, null, 1f);

            return new CheckResult(_modifiersFeedback, CheckModifiers(modified) ? Correct : Incorrect, modified, 1f);
        }
    }

    class NotModifier : IModifier
    {
        private IModifier _modifier;
        public NotModifier(IModifier modifier) { _modifier = modifier; }

        public string GetName() => $"not {_modifier.GetName()}";

        protected bool Equals(IModifier other) { return !_modifier.Equals(other); }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is IModifier other && Equals(other);
        }

        public override int GetHashCode() { return GetName().GetHashCode(); }
    }

    public static class ModifierExtension
    {
        public static IModifier Not(this IModifier modifier)
        {
            return new NotModifier(modifier);
        }
    }
}
