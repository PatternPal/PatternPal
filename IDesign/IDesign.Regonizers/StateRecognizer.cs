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
            result = new Result();

            //if node is interface, node is probaly a state. else node can be state, context or concrete state
            if (node.GetEntityNodeType() == EntityNodeType.Interface)
            {
                //state checks
                StateChecks(node);
            }
            else if (node.GetEntityNodeType() == EntityNodeType.Class)
            {
                //if node is an abstract class, node is probaly a state
                if (node.CheckModifier("abstract"))
                {
                    //state checks
                    StateChecks(node);
                }
                else
                {
                    //normal class checks
                    ClassChecks(node);
                }
            }
            return result;
        }

        /// <summary>
        ///     Function to check if a node is an interface or abstract class, when true node is probaly a state
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IResult StateChecks(IEntityNode node)
        {
            //standard amount of checks for state when using an interface
            float amountOfChecks = 2;

            //check if node is an interface or an abstract class, canot be both
            var checkType = new List<ElementCheck<IEntityNode>>
            {
                new ElementCheck<IEntityNode>(x => (x.CheckModifier("abstract")) |
                (x.CheckTypeDeclaration(EntityNodeType.Interface)), "If using a class, the modifier should be abstract. Otherwise, use an interface")
            };
            CheckElements(result, new List<IEntityNode> { node }, checkType);

            //check if the method of the node has return type void
            var checkMethods = new List<ElementCheck<IMethod>>
            {
                 new ElementCheck<IMethod>(x => x.CheckReturnType("void"), "return type should be void")
            };
            CheckElements(result, node.GetMethods(), checkMethods);

            //if node is an abstract class check of the method is also abstract and has void as return type
            if (node.CheckModifier("abstract"))
            {
                var abstractStateChecks = new List<ElementCheck<IMethod>>
                {
                    new ElementCheck<IMethod>(x => x.CheckModifier("abstract"), "If using a class, the modifier should be abstract. Otherwise, use an interface"),
                    new ElementCheck<IMethod>(x => x.CheckReturnType("void"), "return type should be void")

                };
                amountOfChecks = 4;
                CheckElements(result, node.GetMethods(), abstractStateChecks);
            }
            result.Score = (int)(result.Score / amountOfChecks * 100f);
            return result;
        }

        /// <summary>
        ///     Function to do the checks for a normall class (Context or ConcreteState)
        /// </summary>
        /// <param name="node"></param>
        private IResult ClassChecks(IEntityNode node)
        {
            var relations = node.GetRelations();

            //create list with only Extends and Implements relations
            var inheritanceRelations = relations.Where(x => (x.GetRelationType() == RelationType.Implements) ||
            (x.GetRelationType() == RelationType.Extends)).ToList();

            //create list with only uses realations
            var usingRelations = relations.Where(x => x.GetRelationType() == RelationType.Uses).ToList();

            if (inheritanceRelations.Count() > 0)
            {
                ConcreteStateClassChecks(node, inheritanceRelations);
            }
            if (usingRelations.Count() > 0 && inheritanceRelations.Count() <= 0)
            {
                //node is probaly context class
                ContextClassChecks(node, usingRelations);
            }
            return result;
        }

        /// <summary>
        ///     Function to do checks for the concrete state class
        /// </summary>
        /// <param name="node"></param>
        /// <param name="inheritanceRelations"></param>
        /// <returns></returns>
        private IResult ConcreteStateClassChecks(IEntityNode node, List<IRelation> inheritanceRelations)
        {
            float amountOfChecks = 1;
            foreach (var edge in inheritanceRelations)
            {
                var edgeNode = edge.GetDestination();

                var inheritanceChecks = new List<ElementCheck<IEntityNode>>
                    {
                        //check if node is an interface or an abstract class
                        new ElementCheck<IEntityNode>(x => (x.CheckTypeDeclaration(EntityNodeType.Interface)) |
                        ((x.CheckTypeDeclaration(EntityNodeType.Class)) && (x.CheckModifier("abstract"))),"message")
                    };
                CheckElements(result, new List<IEntityNode> { edgeNode }, inheritanceChecks);
            }
            result.Score = (int)(result.Score / amountOfChecks * 100f);
            return result;
        }

        /// <summary>
        ///     Function to do checks for the context class
        /// </summary>
        /// <param name="node"></param>
        /// <param name="usingRelations"></param>
        /// <returns></returns>
        private IResult ContextClassChecks(IEntityNode node, List<IRelation> usingRelations)
        {
            float amountOfChecks = 2;
            foreach (var edge in usingRelations)
            {
                var edgeNode = edge.GetDestination();

                if ((edgeNode.CheckTypeDeclaration(EntityNodeType.Interface)) |
                    (edgeNode.CheckTypeDeclaration(EntityNodeType.Class) && edgeNode.CheckModifier("abstract")))
                {
                    var usingChecks = new List<ElementCheck<IField>>
                    {
                        new ElementCheck<IField>(x => x.CheckFieldType(edgeNode.GetName()),$"{node.GetName()} must be equal to {edgeNode.GetName()}"),
                        new ElementCheck<IField>(x => x.CheckMemberModifier("private"), "modifier must be private")
                    };
                    CheckElements(result, node.GetFields(), usingChecks);
                }
            }
            result.Score = (int)(result.Score / amountOfChecks * 100f);
            return result;
        }

    }
}
