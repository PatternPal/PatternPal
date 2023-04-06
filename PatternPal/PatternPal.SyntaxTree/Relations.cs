using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Root;
using SyntaxTree.Models.Entities;
using SyntaxTree.Models.Members.Method;

namespace SyntaxTree
{
    public class Relations
    {
        private static readonly Dictionary<RelationType, RelationType> ReversedTypes =
            new Dictionary<RelationType, RelationType>
            {
                {RelationType.Implements, RelationType.ImplementedBy},
                {RelationType.ImplementedBy, RelationType.Implements},
                {RelationType.Creates, RelationType.CreatedBy},
                {RelationType.CreatedBy, RelationType.Creates},
                {RelationType.Uses, RelationType.UsedBy},
                {RelationType.UsedBy, RelationType.Uses},
                {RelationType.Extends, RelationType.ExtendedBy},
                {RelationType.ExtendedBy, RelationType.Extends}
            };

        private readonly SyntaxGraph _graph;
        private Dictionary<string, IEntity> _entities;

        //Relations between entities
        internal Dictionary<IEntity, List<Relation>> relations = new Dictionary<IEntity, List<Relation>>();

        public Relations(SyntaxGraph graph)
        {
            _graph = graph;
        }

        public void CreateEdgesOfEntities()
        {
            _entities = _graph.GetAll();
            foreach (var entity in _entities.Values)
            {
                CreateParentClasses(entity);
                CreateCreationalEdges(entity);
                CreateUsingEdges(entity);
            }
        }

        private void CreateMethodEdges(IEntity entity)
        {
            foreach (Method method in entity.GetAllMethods())
            {
                IEnumerable childNodes = method.GetBody().DescendantNodes();

                foreach (var creation in childNodes.OfType<ObjectCreationExpressionSyntax>())
                {
                    if (creation.Type is IdentifierNameSyntax name)
                    {
                        //Get method/entity associated with creation
                        //Add relation
                    }
                }

                foreach (var identifier in childNodes.OfType<IdentifierNameSyntax>())
                {
                    //Get method/entity associated with identifier
                    //Add relation
                }
            }
        }

        private void AddRelation(IEntity node, RelationType type, string destination)
        {
            var edgeNode = GetNodeByName(node, destination);
            if (edgeNode == null)
            {
                return;
            }

            AddRelation(node, type, edgeNode);
        }

        private void AddRelation(IEntity node, RelationType type, IEntity edgeNode)
        {
            if (edgeNode == null)
            {
                return;
            }

            if (!relations.ContainsKey(node))
            {
                relations.Add(node, new List<Relation>());
            }

            var list = relations[node];

            //Make sure relation does not already exists
            if (list.Any(x => x.GetDestination() == edgeNode && x.GetRelationType() == type))
            {
                return;
            }

            list.Add(new Relation(edgeNode, type));
            AddRelation(edgeNode, ReversedTypes[type], node);
        }

        private void CreateCreationalEdges(IEntity entity)
        {
            var childNodes = entity.GetSyntaxNode().DescendantNodes();
            foreach (var creation in childNodes.OfType<ObjectCreationExpressionSyntax>())
            {
              
                if (creation.Type is IdentifierNameSyntax name)
                {
                    AddRelation(entity, RelationType.Creates, name.ToString());
                }
            }
        }

        private void CreateUsingEdges(IEntity entity)
        {
            var childNodes = new List<SyntaxNode>();
            foreach (var member in entity.GetMembers())
            {
                childNodes.AddRange(member.GetSyntaxNode().DescendantNodes(n => true));
            }

            foreach (var identifier in childNodes.OfType<IdentifierNameSyntax>())
            {
                if (identifier is IdentifierNameSyntax name)
                {
                    AddRelation(entity, RelationType.Uses, name.ToString());
                }
            }
        }

        private IEntity GetNodeByName(IEntity node, string name)
        {
            var namespaces = node.GetParent().GetAllAccessNamespaces();

            return namespaces.Select(x => x.Length == 0 ? name : $"{x}.{name}")
                .Where(x => _entities.ContainsKey(x))
                .Select(x => _entities[x])
                .FirstOrDefault();
        }

        private void CreateParentClasses(IEntity entity)
        {
            foreach (var type in entity.GetBases())
            {
                //TODO check generic
                var typeName = type.ToString();

                var edgeNode = GetNodeByName(entity, typeName);
                if (edgeNode == null)
                {
                    continue;
                }

                RelationType relationType;

                switch (edgeNode.GetEntityType())
                {
                    case EntityType.Class:
                        relationType = RelationType.Extends;
                        break;
                    case EntityType.Interface:
                        relationType = RelationType.Implements;
                        break;
                    default:
                        Console.Error.WriteLine(
                            $"EntityType {edgeNode.GetType()} is not yet supported in DetermineRelations!"
                        );
                        continue;
                }

                AddRelation(entity, relationType, edgeNode);
            }
        }

        public void Reset()
        {
            relations = new Dictionary<IEntity, List<Relation>>();
        }
    }

    public static class UsingUtil
    {
        public static IEnumerable<string> GetAllAccessNamespaces(this IEntitiesContainer root)
        {
            var namespaces = new List<string>();
            if (root is INamespace names)
            {
                namespaces.AddRange(names.GetRoot().GetAllAccessNamespaces());
                namespaces.Add(names.GetName());
            }

            if (root is IUsingContainer usingContainer)
            {
                namespaces.AddRange(usingContainer.GetUsing().Select(x => x.Name.ToString()));
            }

            namespaces.Add("");

            return namespaces;
        }
    }
}
