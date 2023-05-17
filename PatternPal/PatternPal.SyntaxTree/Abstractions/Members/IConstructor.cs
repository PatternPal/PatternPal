namespace PatternPal.SyntaxTree.Abstractions.Members
{
    public interface IConstructor : IMember, IParameterized, IBodied, IChild<IClass>
    {
        /// <summary>
        /// Gets the type of the constructor.
        /// </summary>
        /// <returns>The type of the constructor in string format.</returns>
        string GetConstructorType();

        /// <summary>
        /// Gets the <see cref="ConstructorDeclarationSyntax"/> of the constructor.
        /// </summary>
        /// <returns>The <see cref="ConstructorDeclarationSyntax"/>.</returns>
        public ConstructorDeclarationSyntax GetConstructorDeclarationSyntax();

        /// <summary>
        /// Gets the arguments the constructor was used.
        /// </summary>
        /// <returns>Arguments the constructor was used with in string format.</returns>
        IEnumerable<string> GetArguments();

        /// <summary>
        /// Casts the Constructor to a Method.
        /// </summary>
        /// <returns>An <see cref="IMethod"/>.</returns>
        IMethod AsMethod();

        /// <summary>
        /// Gets the parent of the constructor.
        /// </summary>
        /// <returns>The <see cref="IClass"/> which is the parent of the constructor.</returns>
        new IClass GetParent();
    }
}
