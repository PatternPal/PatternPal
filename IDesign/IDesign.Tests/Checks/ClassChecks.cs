using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using IDesign.Recognizers;
using IDesign.Tests.Utils;
using System.Collections.Generic;
using IDesign.Recognizers.Abstractions;

namespace IDesign.Tests.Checks
{
    public class ClassChecks
    {
        public void CLassImplementsInterface(string filename, bool shouldBeVaild)
        {
            string code = FileUtils.FileToString("ClassChecksTestClasses\\" + filename);
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var NameSpaceNode = root.Members[0] as NamespaceDeclarationSyntax;
            var Nodes = new Dictionary<string, IEntityNode>();

        }
    }
}
