#region

using PatternPal.SyntaxTree.Abstractions;

#endregion

namespace PatternPal.SyntaxTree.Models
{
    // TODO It feels like this class needs review and has many unused methods.

    /// <summary>
    /// Represents a modifier of an <see cref="INode"/>.
    /// </summary>
    /// <example>Examples are private, static, and abstract</example>
    public class Modifier : IModifier
    {
        // TODO possibly completely change to an Enum instead.

        // The type of modifier
        private readonly string _name;

        /// <summary>
        /// Returns an instance of <see cref="Modifier"/>
        /// </summary>
        public Modifier(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Returns the name, thus type, of the modifier
        /// </summary>
        public string GetName() { return _name.ToLower(); }

        protected bool Equals(IModifier other) {
            return _name == other.GetName();
        }

        public override bool Equals(object obj) {
            return ReferenceEquals(this, obj) || obj is IModifier other && Equals(other);
        }

        public override int GetHashCode() {
            return (_name != null ? _name.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return _name;
        }

        // An object pool of all types of modifiers.
        public static readonly IModifier Public = new Modifier("public");
        public static readonly IModifier Internal = new Modifier("internal");
        public static readonly IModifier Protected = new Modifier("protected");
        public static readonly IModifier Private = new Modifier("private");
        public static readonly IModifier Abstract = new Modifier("abstract");
        public static readonly IModifier Const = new Modifier("const");
        public static readonly IModifier Extern = new Modifier("extern");
        public static readonly IModifier Override = new Modifier("override");
        public static readonly IModifier Partial = new Modifier("partial");
        public static readonly IModifier Readonly = new Modifier("readonly");
        public static readonly IModifier Sealed = new Modifier("sealed");
        public static readonly IModifier Static = new Modifier("static");
        public static readonly IModifier Unsafe = new Modifier("unsafe");
        public static readonly IModifier Virtual = new Modifier("virtual");
        public static readonly IModifier Volatile = new Modifier("volatile");
        public static readonly IModifier New = new Modifier("new");
        public static readonly IModifier Async = new Modifier("async");

        /// <summary>
        /// Adds the possibility to create a modifier not already present in the object pool of modifiers.
        /// </summary>
        public static IModifier Custom(string name)
        {
            return new Modifier(name);
        }
    }

    /// <summary>
    /// A <see cref="Modifier"/> with a <see cref="SyntaxToken"/>.
    /// </summary>
    internal class SyntaxModifier : Modifier
    {
        internal readonly SyntaxToken syntaxToken;

        public SyntaxModifier(SyntaxToken token) : base(token.Text)
        {
            syntaxToken = token;
        }
    }
}
