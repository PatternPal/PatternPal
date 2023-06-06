namespace PatternPal.Core.StepByStep
{
    public class SimpleInstruction : IInstruction
    {
        public string Requirement { get; }
        public string Description { get; }
        public List< ICheck > Checks { get; }
        public string FileId { get; set; }

        public SimpleInstruction(
            string requirement,
            string description,
            List< ICheck > checks)
        {
            Requirement = requirement;
            Description = description;
            Checks = checks;
        }
    }
}
