using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternPal.StepByStep.Abstractions;
using PatternPal.Core.Recognizers;
using PatternPal.Recognizers.Abstractions;
using PatternPal.Core.Checks;
using PatternPal.Core.StepByStep;

namespace PatternPal.StepByStep.InstructionSets
{
    public class SingletonInstructionSet : IInstructionSet
    {
        public SingletonInstructionSet()
        {
            Recognizer = new SingletonRecognizer();
            // TODO implement in recognizer
            Steps = Recognizer.GenerateStepsList();
            Index = 0;
            Pattern = "Singleton";
        }
        public string Pattern { get; }
        public IRecognizer Recognizer { get; }
        public List<IInstruction> Steps { get; }
        public IInstruction ObtainCurrentInstruction()
        {
            return Steps[Index];
        }
        public int Index { get; set; }

        public void NextStep()
        {
            Index++;
        }

        public void PreviousStep()
        {
            throw new NotImplementedException();
        }
    }
}
