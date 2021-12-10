namespace SyntaxTree.Abstractions.Root {
    public interface INamespace : IRoot, IChild<IRoot> {
        string GetNamespace();
    }
}
