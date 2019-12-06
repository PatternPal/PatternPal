using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models;
using IDesign.Recognizers.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDesign.Recognizers
{
    public class StateRecognizer : Recognizer, IRecognizer
    {
        Result result;

        public IResult Recognize(IEntityNode node)
        {
            Console.WriteLine("----- NAME OF NODE -----");
            Console.WriteLine(node.GetName());

            result = new Result();

            //if node is interface or abstract class, do checks for this type else do classnode checks
            if (node.GetEntityNodeType() == EntityNodeType.Interface)
            {
                StateInterfaceChecks(node);
            }
            else if (node.CheckModifier("abstract"))
            {
                StateClassChecks(node);
            }
            else if (node.GetEntityNodeType() == EntityNodeType.Class)
            {
                ClassChecks(node);
            }

            return result;
        }


        /// <summary>
        ///     Function to do the checks for a normall class (Context or ConcreteState)
        /// </summary>
        /// <param name="node"></param>
        private void ClassChecks(IEntityNode node)
        {
            ContextClassChecks(node);
            ConcreteStateClassChecks(node);
        }

        /// <summary>
        ///     Search if class contains a field that is private and with the type of the class
        /// </summary>
        /// <param name="node"></param>
        /// <param name="edgeNode"></param>
        private IResult SearchForField(IEntityNode node, IEntityNode edgeNode)
        {
            var checkEdges = new List<ElementCheck<IField>>
            {
                new ElementCheck<IField>(x => x.CheckFieldType(edgeNode.GetName()), $"{node.GetName()} must be equal to {edgeNode.GetName()}"),
                new ElementCheck<IField>(x => x.CheckMemberModifier("private"), "modifier must be private")
            };
            CheckElements(result, node.GetFields(), checkEdges);
            result.Score = (int)(result.Score / 2f * 100f);
            return result;
        }

        /// <summary>
        ///     Checks for a ConcreteState Class
        /// </summary>
        /// <param name="node"></param>
        private void ConcreteStateClassChecks(IEntityNode node)
        {
            var relations = node.GetRelations();

            //create list with only Extends and Implements relations
            relations = relations.Where(item => (item.GetRelationType() == RelationType.Extends) || (item.GetRelationType() == RelationType.Implements)).ToList();

            foreach (var edge in relations)
            {
                var edgeNode = edge.GetDestination();
                Console.WriteLine("----- INHERITANCE -----");
                Console.WriteLine(edgeNode.GetName());

                //TO DO: check overerving state

                InheritanceType(edgeNode);

            }
        }

        /// <summary>
        ///     Check if a concreteState implements an interface or abstract class
        /// </summary>
        /// <param name="edgeNode"></param>
        /// <returns></returns>
        private IResult InheritanceType(IEntityNode edgeNode)
        {
            if (edgeNode.GetEntityNodeType() == EntityNodeType.Interface)
            {
                //give all point if implements interface
                result.Score = 1;
            }
            else
            {
                var check = new List<ElementCheck<IEntityNode>>
                {
                    new ElementCheck<IEntityNode>(x => x.CheckModifier("abstract"), "If using a class, the modifier should be abstract. Otherwise, use an interface")
                };
                CheckElements(result, new List<IEntityNode> { edgeNode }, check);
            }
            result.Score = (int)(result.Score / 1f * 100f);
            return result;
        }

        /// <summary>
        ///     Checks for a Context class
        /// </summary>
        /// <param name="node"></param>
        private void ContextClassChecks(IEntityNode node)
        {
            //Make list with edges of the node
            var relations = node.GetRelations();

            //create list with only Uses relations
            relations = relations.Where(item => item.GetRelationType() == RelationType.Uses).ToList();

            //now select te first, cause there are identical usings in the usings list
            //TO DO: delete this and loop over relations 
            var firstEdge = relations.Select(x => x.GetDestination()).Distinct().ToList();

            //loop over all edges
            foreach (var edge in firstEdge)
            {
                //if edge is interface or abstract class, check if edge has a prop with this node as type
                if (edge.GetEntityNodeType() == EntityNodeType.Interface | edge.CheckModifier("abstract"))
                {
                    Console.WriteLine("\t" + edge.GetName());
                    SearchForField(node, edge);
                }
            }
            Console.WriteLine();
        }

        private IResult StateInterfaceChecks(IEntityNode node)
        {
            var methodCheck = new List<ElementCheck<IMethod>>
            {
                new ElementCheck<IMethod>(x => x.CheckReturnType("void"), "return type should be void")
            };
            CheckElements(result, node.GetMethods(), methodCheck);
            result.Score = (int)(result.Score / 1f * 100f);
            return result;
        }

        private IResult StateClassChecks(IEntityNode node)
        {
            var classModifierCheck = new List<ElementCheck<IEntityNode>>
            {
                new ElementCheck<IEntityNode>(x => x.CheckModifier("abstract"), "If using a class, the modifier should be abstract. Otherwise, use an interface")
            };
            CheckElements(result, new List<IEntityNode> { node }, classModifierCheck);

            var checks = new List<ElementCheck<IMethod>>
            {
                new ElementCheck<IMethod>(x => x.CheckModifier("abstract"), "If using a class, the modifier should be abstract. Otherwise, use an interface"),
                new ElementCheck<IMethod>(x => x.CheckReturnType("void"), "return type should be void")
            };
            CheckElements(result, node.GetMethods(), checks);

            result.Score = (int)(result.Score / 3f * 100f);
            return result;
        }

    }
}
