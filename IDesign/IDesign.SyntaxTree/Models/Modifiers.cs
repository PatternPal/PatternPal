using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using SyntaxTree.Abstractions;

namespace SyntaxTree.Models {
    internal class Modifier : IModifier {
        private readonly string _name;

        public Modifier(string name) {
            this._name = name;
        }

        public string GetName() { return _name.ToLower(); }

        private bool Equals(Modifier other) {
            return _name == other._name;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            if (obj.GetType() != GetType()) {
                return false;
            }

            return Equals((Modifier)obj);
        }

        public override int GetHashCode() {
            return _name != null ? _name.GetHashCode() : 0;
        }

        public override string ToString() => _name;
    }

    public static class Modifiers {
        //AccessModifiers
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

        public static IModifier Custom(string name) {
            return new Modifier(name);
        }
    }

    internal class SyntaxModifier : Modifier {
        internal readonly SyntaxToken syntaxToken;
        
        public SyntaxModifier(SyntaxToken token) : base(token.Text) {
            syntaxToken = token;
        }
    }
}
