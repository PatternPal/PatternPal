using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace SyntaxTree
{
    public static class SemanticModels
    {
        //This is the CSharpCompilation class which represents the entire SyntaxGraph. It is initialized to an empty class, and SyntaxTrees are added while files are added.
        private static CSharpCompilation _compilation = CSharpCompilation.Create(
            Guid.NewGuid().ToString(),
            null,
            new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location)
            }
        );

        /// <summary>
        ///     Adds roslyn SyntaxTrees to the CSharpCompilation class. This is done when a new file is added to the SyntaxGraph. Each file has it's own SyntaxTree.
        /// </summary>
        /// <param name="trees">The SyntaxTrees to be added.</param>
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

        /// <summary>
        ///     Gets the SemanticModel for a specific SyntaxTree in the CSharpCompilation class.
        /// </summary>
        /// <param name="tree">The SyntaxTree to get the SemanticModel for.</param>
        /// <param name="ignoreAccessibility">Whether to include items in the SemanticModel which are inaccessible for the given SyntaxTree.</param>
        /// <returns>The parsed file</returns>
        public static SemanticModel GetSemanticModel(Microsoft.CodeAnalysis.SyntaxTree tree, bool ignoreAccessibility)
        {
            
            if (_compilation.SyntaxTrees.Any(x => x == tree))
            {
                return _compilation.GetSemanticModel(tree, ignoreAccessibility);
            }

            AddTreesToCompilation(tree);
            return _compilation.GetSemanticModel(tree, ignoreAccessibility);
        }

        /// <summary>
        /// Resets the compilation, used in the unit tests.
        /// </summary>
        public static void Reset()
        {
            _compilation = CSharpCompilation.Create(
                Guid.NewGuid().ToString(),
                null,
                new MetadataReference[]
                {
                    MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location)
                }
            );
        }
    }
}
