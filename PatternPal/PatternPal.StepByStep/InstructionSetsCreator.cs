#region

using System.Collections.Generic;
using System.Collections.ObjectModel;

using PatternPal.StepByStep.Abstractions;
using PatternPal.StepByStep.InstructionSets;

#endregion

namespace PatternPal.StepByStep
{
    public static class InstructionSetsCreator
    {
        public static readonly IReadOnlyDictionary< string, IInstructionSet > InstructionSets;

        static InstructionSetsCreator()
        {
            IDictionary< string, IInstructionSet > sets = new Dictionary< string, IInstructionSet >();

            IInstructionSet set = new StrategyInstructionSet();
            sets.Add(
                set.Name,
                set);

            InstructionSets = new ReadOnlyDictionary< string, IInstructionSet >(sets);
        }
    }
}
