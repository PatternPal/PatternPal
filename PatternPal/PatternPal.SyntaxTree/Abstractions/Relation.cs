using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Models.Entities;
using SyntaxTree.Models.Members.Method;

namespace SyntaxTree.Abstractions
{

    public class Relation
    {
        //private Relationable _relationable;
        private RelationType _type;

        public IEntity Node1Entity;
        public IEntity Node2Entity;

        public Method Node1Method;
        public Method Node2Method;

        public Relation(RelationType relationType)
        {
            //_relationable = relationable;
            _type = relationType;
        }

        public RelationType GetRelationType()
        {
            return _type;
        }
    }

    public enum RelationType
    {
        Implements,
        ImplementedBy,
        Extends,
        ExtendedBy,
        Uses,
        UsedBy,
        Creates,
        CreatedBy
    }

    public enum Relationable
    {
        Entity,
        Method
    }
}
