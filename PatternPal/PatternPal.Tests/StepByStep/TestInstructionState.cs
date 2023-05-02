using PatternPal.StepByStep.Abstractions;

namespace PatternPal.Tests.StepByStep
{
    public class TestInstructionState : Dictionary<string, IEntity>, IInstructionState
    {
        /// <summary>
        /// Creates a Instruction state from a given set of files
        /// </summary>
        /// <param name="files">Key indicates the id of the class, the value indicates which file to read from</param>
        /// <param name="dictionary">The location of all files</param>
        /// <remarks>
        /// When multiple entities are in a single file it will take the first one for the State
        /// </remarks>
        public TestInstructionState(Dictionary<string, string> files, string dictionary)
        {
            var graph = new SyntaxGraph();
            foreach (var (id, file) in files)
            {
                var loc = $"{dictionary}/{file}";
                var root = graph.AddFile(FileUtils.FileToString(loc), loc);
                var (_, entity) = root.GetAllEntities().FirstOrDefault();
                if (entity != default) this[id] = entity;

            }
            
            graph.CreateGraph();
        }
    }
}
