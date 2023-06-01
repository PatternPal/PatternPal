#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PatternPal.StepByStep.Abstractions;
using PatternPal.StepByStep.InstructionSets;

#endregion

namespace PatternPal.StepByStep
{
    public static class InstructionSetsCreator
    {
        public static IInstructionSet InstructionSet;
        public static List<string> selectablePatterns = new List<string>
        {
            "singleton"
        };

        static InstructionSetsCreator() { }

        public static void SetInstructionSet(string patternName)
        {
            switch (patternName)
            {
                case "singleton":
                    InstructionSet = new SingletonInstructionSet();
                    break;
                default:
                    // TODO make this the default selection from the combobox
                    InstructionSet = new SingletonInstructionSet();
                    break;
            }
        }
    }
}
