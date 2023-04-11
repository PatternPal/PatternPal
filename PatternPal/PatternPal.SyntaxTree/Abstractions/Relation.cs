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
        private RelationType _type;

        //There is one variable per possible type for node 1 and 2, this is done so we can have one generic Relation class. Only one Node1.. variable and only one Node2.. variable is meant to be initialized. This is done when the relation is created and should not be altered somewhere else!
        public IEntity Node1Entity;
        public IEntity Node2Entity;

        public Method Node1Method;
        public Method Node2Method;

        /// <summary>
        /// Creates a class to represent a relation between a single entity or method and another entity or method. 
        /// </summary>
        /// <param name="relationType">The type of relation.</param>
        public Relation(RelationType relationType)
        {
            _type = relationType;
        }

        /// <summary>
        /// Gets the RelationType of this Relation.
        /// </summary>
        /// <returns>The RelationType of this Relation.</returns>
        public RelationType GetRelationType()
        {
            return _type;
        }

        /// <summary>
        /// Gets the name of the destination node of this relation.
        /// </summary>
        /// <returns>The name of the destination node of this relation.</returns>
        public string GetDestinationName()
        {
            return Node2Method.GetName() ?? Node2Entity.GetName();
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

    /// <summary>
    /// This enum denotes which type the destination node has to be when querying on sets of relations.
    /// </summary>
    public enum Relationable
    {
        All,
        Entity,
        Method
    }
}
