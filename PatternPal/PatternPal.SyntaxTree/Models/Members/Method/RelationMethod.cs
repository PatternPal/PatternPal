using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;

namespace SyntaxTree.Models.Members.Method
{
    public class RelationMethod : IRelation
    {
        private readonly Method _methodNode;
        private readonly RelationType _type;

        public RelationMethod(Method methodNode, RelationType type)
        {
            _methodNode = methodNode;
            _type = type;
        }

        public Method GetDestination()
        {
            return _methodNode;
        }

        public RelationType GetRelationType()
        {
            return _type;
        }
    }
}
