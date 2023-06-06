#region

using System;
using System.Linq;

using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Models.Entities;

#endregion

namespace PatternPal.SyntaxTree
{
    /// <summary>
    /// Makes and keeps a collection of all <see cref="Relation"/>s of the <see cref="SyntaxGraph"/>,
    /// and makes it possible to search for specific relations
    /// </summary>
    public class Relations
    {
        // Reverses the type of relation.
        private static readonly Dictionary< RelationType, RelationType > ReversedTypes =
            new()
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
                    RelationType.Overrides, RelationType.OverriddenBy
                },
                {
                    RelationType.OverriddenBy, RelationType.Overrides
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

        // List with all relations in the graph.
        private List< Relation > _relations = new();

        // Dictionary to access relations of entities fast.
        internal Dictionary< IEntity, List< Relation > > EntityRelations = new();

        // Dictionary to access relations of members fast.
        internal Dictionary< IMember, List< Relation > > MemberRelations = new();

        private readonly SyntaxGraph _graph;
        private List< IEntity > _entities = new();
        private readonly List< IMember > _members = new();

        /// <summary>
        /// Returns an instance of <see cref="Relations"/>.
        /// </summary>
        public Relations(
            SyntaxGraph graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Creates <see cref="Relation"/>s between <see cref="IEntity"/>s and <see cref="IMember"/>s.
        /// </summary>
        public void CreateEdges()
        {
            // Get all entities
            _entities = _graph.GetAll().Values.ToList();

            // Get all members.
            _members.AddRange(
                _entities.SelectMany(
                    y =>
                        y.GetMembers()));

            foreach (IEntity entity in _entities)
            {
                CreateParentEdges(entity);
                CreateCreationalEdges(entity);
                CreateUsingEdges(entity);
            }

            foreach (IMember member in _members)
            {
                CreateCreationalEdges(member);
                CreateUsingEdges(member);
                CreateOverridingEdges(member);
            }
        }

        /// <summary>
        /// Adds a <see cref="Relation"/> from node1 to node2. It also adds the reversed <see cref="Relation"/> from node2 to node1.
        /// </summary>
        /// <param name="node1"><see cref="INode"/>, can be <see cref="IEntity"/> or <see cref="IMember"/>.</param>
        /// <param name="node2"><see cref="INode"/>, can be <see cref="IEntity"/> or <see cref="IMember"/>.</param>
        /// <param name="type">The <see cref="RelationType"/> of the <see cref="Relation"/></param>
        /// <exception cref="ArgumentException">Throws exception when trying to add a <see cref="Relation"/> to or from a not supported type.</exception>
        private void AddRelation(
            INode ? node1,
            INode ? node2,
            RelationType type)
        {
            // node1 and node2 can be null when the node searched does not exist.
            if (node1 is null
                || node2 is null)
            {
                return;
            }

            OneOf< IEntity, IMember > source = node1 switch
            {
                IEntity entity => OneOf< IEntity, IMember >.FromT0(entity),
                IMember member => OneOf< IEntity, IMember >.FromT1(member),
                _ => throw new ArgumentException()
            };

            OneOf< IEntity, IMember > target = node2 switch
            {
                IEntity entity => OneOf< IEntity, IMember >.FromT0(entity),
                IMember member => OneOf< IEntity, IMember >.FromT1(member),
                _ => throw new ArgumentException()
            };

            // Create the relation of type 'type' between the node1 and node2.
            Relation relation = new(
                type,
                source,
                target );
            Relation relationReversed = new(
                ReversedTypes[ type ],
                target,
                source );

            // TODO: This requires the custom Equals method. Does not yet work
            // If the relation is already stored, do not add it again.
            if (_relations.Any(r => r == relation)
                || _relations.Any(r => r == relationReversed))
            {
                return;
            }

            StoreRelation(
                node1,
                relation);

            StoreRelation(
                node2,
                relationReversed);

            _relations.Add(relation);
            _relations.Add(relationReversed);
        }

        /// <summary>
        /// Helper function for <see cref="AddRelation"/>. This stores the <see cref="Relation"/> in the right place.
        /// </summary>
        /// <param name="node">The source of the <see cref="Relation"/></param>
        /// <param name="relation">The <see cref="Relation"/> to store.</param>
        /// <exception cref="ArgumentException">Throws an exception when trying to add a <see cref="Relation"/> with a not supported <see cref="INode"/>.</exception>
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
                case IMember member:
                {
                    if (!MemberRelations.TryGetValue(
                        member,
                        out List< Relation > ? memberRelations))
                    {
                        memberRelations = new List< Relation >();
                        MemberRelations.Add(
                            member,
                            memberRelations);
                    }
                    memberRelations.Add(relation);
                    break;
                }
                default:
                    throw new ArgumentException($"Cannot add relations to {node}");
            }
        }

        /// <summary>
        /// Gets the <see cref="IEntity"/> instance saved in the <see cref="SyntaxGraph"/> by analyzing the <see cref="SemanticModel"/> of the <see cref="Microsoft.CodeAnalysis.SyntaxTree"/> (Roslyn).
        /// </summary>
        /// <param name="syntaxNode">The <see cref="SyntaxNode"/> from which we want the belonging <see cref="IEntity"/> instance.</param>
        /// <returns>The matched <see cref="IEntity"/></returns>
        public IEntity ? GetEntityByName(
            SyntaxNode syntaxNode)
        {
            SemanticModel semanticModel = SemanticModels.GetSemanticModel(
                syntaxNode.SyntaxTree,
                false);

            SymbolInfo symbol = semanticModel.GetSymbolInfo(syntaxNode);

            TypeDeclarationSyntax ? entityDeclaration = symbol.Symbol?.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as TypeDeclarationSyntax;

            return _entities.FirstOrDefault(x => x.GetSyntaxNode().IsEquivalentTo(entityDeclaration));
        }

        /// <summary>
        /// Gets the <see cref="IMember"/> instance saved in the <see cref="SyntaxGraph"/> by analyzing the <see cref="SemanticModel"/> of the <see cref="Microsoft.CodeAnalysis.SyntaxTree"/> (Roslyn).
        /// </summary>
        /// <param name="memberNode">The <see cref="SyntaxNode"/> from which we want the belonging <see cref="IMember"/> instance.</param>
        /// <returns>The matched <see cref="IMember"/></returns>
        private IMember ? GetMemberByName(
            SyntaxNode memberNode)
        {
            SemanticModel semanticModel = SemanticModels.GetSemanticModel(
                memberNode.SyntaxTree,
                false);

            SymbolInfo symbol = semanticModel.GetSymbolInfo(memberNode);

            MemberDeclarationSyntax ? memberDeclaration;

            SyntaxNode ? declarationNode = symbol.Symbol?.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();

            //When a field is already declared, the DeclaringSyntaxReferences is a VariableDeclaratorSyntax
            if (declarationNode is VariableDeclaratorSyntax)
            {
                memberDeclaration = symbol.Symbol?.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax().Parent?.Parent as MemberDeclarationSyntax;
            }
            else
            {
                memberDeclaration = symbol.Symbol?.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as MemberDeclarationSyntax;
            }

            return _members.FirstOrDefault(x => x.GetSyntaxNode().IsEquivalentTo(memberDeclaration));
        }

        /// <summary>
        /// Gets the <see cref="IMember"/> instance saved in the <see cref="SyntaxGraph"/>.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> for which to get the corresponding <see cref="IMember"/> instance.</param>
        /// <returns>The corresponding <see cref="IMember"/> instance, or <see langword="null"/> if it does not exist.</returns>
        private IMember ? GetMemberBySymbol(
            ISymbol symbol)
        {
            SyntaxNode ? declaringSyntax = symbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
            return _members.FirstOrDefault(x => x.GetSyntaxNode().IsEquivalentTo(declaringSyntax));
        }

        /// <summary>
        /// Creates the Extend, ExtendedBy, Implements and ImplementedBy relations between two <see cref="IEntity"/>'s.
        /// </summary>
        /// <param name="entity">The <see cref="IEntity"/> which parents will be evaluated.</param>
        private void CreateParentEdges(
            IEntity entity)
        {
            foreach (TypeSyntax type in entity.GetBases())
            {
                //TODO check generic

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
        /// Creates the Creates and CreatedBy relations between <see cref="IEntity"/>'s and <see cref="IMember"/>'s.
        /// </summary>
        /// <param name="node">The <see cref="IEntity"/> or <see cref="IMember"/> which descendant nodes will be evaluated</param>
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
        /// Creates the Uses and UsedBy relations between <see cref="IEntity"/>'s and <see cref="IMember"/>'s.
        /// </summary>
        /// <param name="node">The <see cref="IEntity"/> or <see cref="IMember"/> which descendant nodes will be evaluated</param>
        private void CreateUsingEdges(
            INode node)
        {
            List< SyntaxNode > childNodes = GetChildNodes(node);

            foreach (IdentifierNameSyntax identifier in childNodes.OfType< IdentifierNameSyntax >())
            {
                INode ? node2 = (INode ?)GetEntityByName(identifier) ?? GetMemberByName(identifier);

                AddRelation(
                    node,
                    node2,
                    RelationType.Uses);

                if (identifier.Parent is ObjectCreationExpressionSyntax parent
                    && node2 is Class classNode)
                {
                    IMember ? matchedConstructor = GetMemberByName(parent);

                    AddRelation(
                        node,
                        matchedConstructor,
                        RelationType.Uses);
                }
            }
        }

        /// <summary>
        /// Creates the <see cref="RelationType.Overrides"/> relations between <see cref="IMember"/>s.
        /// </summary>
        /// <param name="member">The <see cref="IMember"/> which will be checked if it overrides another <see cref="IMember"/>.</param>
        private void CreateOverridingEdges(
            IMember member)
        {
            if (member is not IMethod)
            {
                return;
            }

            SemanticModel semanticModel = SemanticModels.GetSemanticModel(
                member.GetSyntaxNode().SyntaxTree,
                false);
            ISymbol declaredSymbol = semanticModel.GetDeclaredSymbol(member.GetSyntaxNode())!;

            if (declaredSymbol.IsOverride)
            {
                switch (declaredSymbol)
                {
                    case IMethodSymbol methodSymbol:
                    {
                        AddRelation(
                            member,
                            GetMemberBySymbol(methodSymbol.OverriddenMethod!),
                            RelationType.Overrides);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Helper function to retrieve all descendant nodes of a node.
        /// </summary>
        /// <param name="node">The <see cref="IEntity"/> or <see cref="IMember"/> which descendant <see cref="SyntaxNode"/>'s will be found</param>
        /// <returns>List of descendant <see cref="SyntaxNode"/>'s of an <see cref="INode"/></returns>
        private List< SyntaxNode > GetChildNodes(
            INode node)
        {
            List< SyntaxNode > childNodes = new();
            switch (node)
            {
                case IEntity entityNode:
                    foreach (IMember member in entityNode.GetMembers())
                    {
                        childNodes.AddRange(member.GetSyntaxNode().DescendantNodes());
                    }
                    break;
                case IMember memberNode:
                    childNodes.AddRange(memberNode.GetSyntaxNode().DescendantNodes());
                    break;
            }
            return childNodes;
        }

        /// <summary>
        /// Resets the lists of <see cref="Relation"/>'s.
        /// </summary>
        public void Reset()
        {
            _relations = new List< Relation >();
            EntityRelations = new Dictionary< IEntity, List< Relation > >();
            MemberRelations = new Dictionary< IMember, List< Relation > >();
        }
    }
}
