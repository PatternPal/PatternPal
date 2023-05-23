using System;
using System.Linq;
using PatternPal.Recognizers.Abstractions;
using PatternPal.Recognizers.Models.Output;
using PatternPal.SyntaxTree.Abstractions;

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
}
