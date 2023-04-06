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
        private readonly Relationship _relationship;
        private RelationEntity _relationEntity;
        private RelationMethod _relationMethod;
        
        public Relation(Relationship relationship)
        {   
            _relationship = relationship;
        }

        public void SetRelation(RelationEntity relationEntity)
        {
            if (_relationship == Relationship.EntityEntity || _relationship == Relationship.MethodEntity)
                _relationEntity = relationEntity;
            else
                throw new ArgumentException("Cannot have a RelationEntity with the current RelationShip type.");
        }

        public void SetRelation(RelationMethod relationMethod)
        {
            if (_relationship == Relationship.EntityMethod || _relationship == Relationship.MethodMethod)
                _relationMethod = relationMethod;
            else
                throw new ArgumentException("Cannot have a RelationMethod with the current RelationShip type.");
        }

        public RelationEntity GetRelationEntity()
        {
            if (_relationEntity == null)
                throw new ArgumentException("RelationEntity is null");
            else
                return _relationEntity;
        }

        public RelationMethod GetRelationMethod()
        {
            if (_relationMethod == null)
                throw new ArgumentException("RelationEntity is null");
            else
                return _relationMethod;
        }

    }
    
    public class Relation2
    {
        //private Relationable _relationable;
        private RelationType _type;

        public IEntity Node1Entity;
        public IEntity Node2Entity;

        public Method Node1Method;
        public Method Node2Method;

        public Relation2(RelationType relationType)
        {
            //_relationable = relationable;
            _type = relationType;
        }

        public RelationType GetRelationType()
        {
            return _type;
        }
    }


    public enum Relationable
    {
        Entity,
        Method
    }
}
