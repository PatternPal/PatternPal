#region

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SyntaxTree.Abstractions;
using SyntaxTree.Models.Members.Method;

#endregion

namespace SyntaxTree
{
    public class Relations
    {
        private static readonly Dictionary< RelationType, RelationType > ReversedTypes =
            new Dictionary< RelationType, RelationType >
            {
                {
                    RelationType.Implements, RelationType.ImplementedBy
                },
                {
                    RelationType.ImplementedBy, RelationType.Implements
                },
                {
                    RelationType.Creates, RelationType.CreatedBy
                },
                {
                    RelationType.CreatedBy, RelationType.Creates
                },
                {
                    RelationType.Uses, RelationType.UsedBy
                },
                {
                    RelationType.UsedBy, RelationType.Uses
                },
                {
                    RelationType.Extends, RelationType.ExtendedBy
                },
                {
                    RelationType.ExtendedBy, RelationType.Extends
                }
            };

        //List with all relations in graph
        internal List< Relation > relations = new();

        //Dictionary to access relations of entities fast
        internal Dictionary< IEntity, List< Relation > > EntityRelations = new();

        //Dictionary to access relations of methods fast
        internal Dictionary< IMethod, List< Relation > > MethodRelations = new();

        private readonly SyntaxGraph _graph;
        private List< IEntity > _entities;
        private List< IMethod > _methods = new();

        public Relations(
            SyntaxGraph graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Creates Relations between entities and nodes.
        /// </summary>
        public void CreateEdges()
        {
            _entities = _graph.GetAll().Values.ToList();

            //Get all methods
            _methods.AddRange(
                _entities.SelectMany(
                    y =>
                        y.GetMembers().OfType< IMethod >()));

            foreach (IEntity entity in _entities)
            {
                CreateParentEdges(entity);
                CreateCreationalEdges(entity);
                CreateUsingEdges(entity);
            }

            foreach (IMethod method in _methods)
            {
                CreateCreationalEdges(method);
                CreateUsingEdges(method);
            }
        }

        /// <summary>
        /// Adds a relation from node1 to node2. It also adds the reversed relation from node2 to node1.
        /// </summary>
        /// <param name="node1">INode, can be IEntity or Method.</param>
        /// <param name="node2">INode, can be IEntity or Method.</param>
        /// <param name="type">The RelationType of the relation</param>
        /// <exception cref="ArgumentException">Throws exception when trying to add a relation to or from a not supported type.</exception>
        private void AddRelation(
            INode ? node1,
            INode ? node2,
            RelationType type)
        {
            if (node1 is null
                || node2 is null)
            {
                return;
            }

            OneOf< IEntity, IMethod > source = node1 switch
            {
                IEntity entity => OneOf< IEntity, IMethod >.FromT0(entity),
                IMethod method => OneOf< IEntity, IMethod >.FromT1(method),
                _ => throw new ArgumentException()
            };

            OneOf< IEntity, IMethod > target = node2 switch
            {
                IEntity entity => OneOf< IEntity, IMethod >.FromT0(entity),
                IMethod method => OneOf< IEntity, IMethod >.FromT1(method),
                _ => throw new ArgumentException()
            };

            Relation relation = new(
                type,
                source,
                target );
            Relation relationReversed = new(
                ReversedTypes[ type ],
                target,
                source );

            // TODO: This requires the custom Equals method.
            if (relations.Any(r => r == relation)
                || relations.Any(r => r == relationReversed))
            {
                return;
            }

            StoreRelation(
                node1,
                relation);

            StoreRelation(
                node2,
                relationReversed);

            relations.Add(relation);
            relations.Add(relationReversed);
        }

        private void StoreRelation(
            INode node,
            Relation relation)
        {
            switch (node)
            {
                case IEntity entity:
                {
                    if (!EntityRelations.TryGetValue(
                        entity,
                        out List< Relation > ? entityRelations))
                    {
                        entityRelations = new List< Relation >();
                        EntityRelations.Add(
                            entity,
                            entityRelations);
                    }
                    entityRelations.Add(relation);
                    break;
                }
                case IMethod method:
                {
                    if (!MethodRelations.TryGetValue(
                        method,
                        out List< Relation > ? methodRelations))
                    {
                        methodRelations = new List< Relation >();
                        MethodRelations.Add(
                            method,
                            methodRelations);
                    }
                    methodRelations.Add(relation);
                    break;
                }
                default:
                    throw new ArgumentException($"Cannot add relations to {node}");
            }
        }

        /// <summary>
        /// Gets the IEntity instance saved in the SyntaxGraph by analyzing the SemanticModel of the SyntaxTree (Roslyn).
        /// </summary>
        /// <param name="syntaxNode">The SyntaxNode from which we want the belonging IEntity instance.</param>
        /// <returns></returns>
        private IEntity ? GetEntityByName(
            SyntaxNode syntaxNode)
        {
            SemanticModel semanticModel = SemanticModels.GetSemanticModel(
                syntaxNode.SyntaxTree,
                false);

            SymbolInfo symbol = semanticModel.GetSymbolInfo(syntaxNode);

            TypeDeclarationSyntax? entityDeclaration;

            if (syntaxNode.Parent is MemberAccessExpressionSyntax && symbol.Symbol is IFieldSymbol fieldSymbol)
            {
                entityDeclaration = fieldSymbol.Type.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as TypeDeclarationSyntax;
            }
            else
            {
                entityDeclaration = symbol.Symbol?.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as TypeDeclarationSyntax;
            }

            return _entities.FirstOrDefault(x => x.GetSyntaxNode().IsEquivalentTo(entityDeclaration));
        }

        /// <summary>
        /// Gets the Method instance saved in the SyntaxGraph by analyzing the SemanticModel of the SyntaxTree (Roslyn).
        /// </summary>
        /// <param name="methodNode">The SyntaxNode from which we want the belonging Method instance.</param>
        /// <returns></returns>
        private IMethod ? GetMethodByName(
            SyntaxNode methodNode)
        {
            SemanticModel semanticModel = SemanticModels.GetSemanticModel(
                methodNode.SyntaxTree,
                false);

            SymbolInfo symbol = semanticModel.GetSymbolInfo(methodNode);

            MethodDeclarationSyntax? methodDeclaration = symbol.Symbol?.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax;

            return _methods.FirstOrDefault(x => x.GetSyntaxNode().IsEquivalentTo(methodDeclaration));
        }

        /// <summary>
        /// Creates the Extend, ExtendedBy, Implements and ImplementedBy relations between two IEntities.
        /// </summary>
        /// <param name="entity">The IEntity which parents will be evaluated.</param>
        private void CreateParentEdges(
            IEntity entity)
        {
            foreach (TypeSyntax type in entity.GetBases())
            {
                //TODO check generic
                //string typeName = type.ToString();

                IEntity ? edgeNode = GetEntityByName(type);
                if (edgeNode is null)
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

                AddRelation(
                    entity,
                    edgeNode,
                    relationType);
            }
        }

        /// <summary>
        /// Creates the Creates and CreatedBy relations between IEntities en Methods.
        /// </summary>
        /// <param name="node">The IEntity or Method which descendant nodes will be evaluated</param>
        private void CreateCreationalEdges(
            INode node)
        {
            List< SyntaxNode > childNodes = GetChildNodes(node);

            foreach (SyntaxNode creation in childNodes)
            {
                switch (creation)
                {
                    case ImplicitObjectCreationExpressionSyntax implicitCreation:
                    {
                        IdentifierNameSyntax ? leftSide = implicitCreation.Parent?.Parent?.Parent?
                            .DescendantNodes().OfType< IdentifierNameSyntax >().FirstOrDefault();

                        if (leftSide is null)
                        {
                            continue;
                        }

                        AddRelation(
                            node,
                            GetEntityByName(leftSide),
                            RelationType.Creates);
                        break;
                    }
                    case ObjectCreationExpressionSyntax normalCreation:
                    {
                        if (normalCreation.Type is IdentifierNameSyntax name)
                        {
                            AddRelation(
                                node,
                                GetEntityByName(name),
                                RelationType.Creates);
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Creates the Uses and UsedBy relations between IEntities and Methods
        /// </summary>
        /// <param name="node">The IEntity or Method which descendant nodes will be evaluated</param>
        private void CreateUsingEdges(
            INode node)
        {
            List< SyntaxNode > childNodes = GetChildNodes(node);

            foreach (IdentifierNameSyntax identifier in childNodes.OfType< IdentifierNameSyntax >())
            {
                INode node2 = (INode?)GetEntityByName(identifier) ?? GetMethodByName(identifier);

                AddRelation(
                    node,
                    node2,
                    RelationType.Uses);
            }
        }

        /// <summary>
        /// Helper function to retrieve all descendant nodes of a node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private List< SyntaxNode > GetChildNodes(
            INode node)
        {
            List< SyntaxNode > childNodes = new();
            switch (node)
            {
                case IEntity entityNode:
                    foreach (var member in entityNode.GetMembers())
                    {
                        childNodes.AddRange(member.GetSyntaxNode().DescendantNodes());
                    }
                    break;
                case Method methodNode:
                    childNodes.AddRange(methodNode.GetSyntaxNode().DescendantNodes());
                    break;
            }
            return childNodes;
        }

        /// <summary>
        /// Resets the relation lists.
        /// </summary>
        public void Reset()
        {
            relations = new List< Relation >();
            EntityRelations = new Dictionary< IEntity, List< Relation > >();
            MethodRelations = new Dictionary< IMethod, List< Relation > >();
        }
    }
}
