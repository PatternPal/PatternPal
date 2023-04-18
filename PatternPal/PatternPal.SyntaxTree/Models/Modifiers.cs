using SyntaxTree.Abstractions;

namespace SyntaxTree.Models
{
    public class Modifier : IModifier
    {
        private readonly string _name;

        public Modifier(string name)
        {
            _name = name;
        }

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

        public static IModifier Custom(string name)
        {
            return new Modifier(name);
        }
    }

    internal class SyntaxModifier : Modifier
    {
        internal readonly SyntaxToken syntaxToken;

        public SyntaxModifier(SyntaxToken token) : base(token.Text)
        {
            syntaxToken = token;
        }
    }
}
