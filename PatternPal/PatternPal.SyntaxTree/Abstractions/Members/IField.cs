namespace PatternPal.SyntaxTree.Abstractions.Members
{
    public interface IField : IMember
    {
        TypeSyntax GetFieldType();
    }
}
