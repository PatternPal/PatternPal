using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SyntaxTree
{
    internal class SemanticModels
    {
        private static CSharpCompilation _compilation = CSharpCompilation.Create(
            Guid.NewGuid().ToString(),
            null,
            new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location)
            }
        );

        public static void AddTreesToCompilation(params Microsoft.CodeAnalysis.SyntaxTree[] trees)
        {
            foreach (var tree in trees)
            {
                if (!_compilation.SyntaxTrees.Any(x => x == tree))
                {
                    _compilation = _compilation.AddSyntaxTrees(tree);
                }
            }
        }
        public static SemanticModel GetSemanticModel(Microsoft.CodeAnalysis.SyntaxTree tree, bool ignoreAccessibility)
        {
            
            if (_compilation.SyntaxTrees.Any(x => x == tree))
            {
                return _compilation.GetSemanticModel(tree, ignoreAccessibility);
            }

            AddTreesToCompilation(tree);
            return _compilation.GetSemanticModel(tree, ignoreAccessibility);
        }
    }
}
