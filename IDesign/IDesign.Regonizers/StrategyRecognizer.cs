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
    public class StrategyRecognizer : Recognizer, IRecognizer
    {
        Result result;

        public IResult Recognize(IEntityNode node)
        {
            result = new Result();

            //if node is interface, node is probaly a strategy. else node can be strategy, context or concrete strategy
            if (node.GetEntityNodeType() == EntityNodeType.Interface)
            {
                //strategy checks
                StrategyChecks(node);
            }
            else if (node.GetEntityNodeType() == EntityNodeType.Class)
            {
                //if node is an abstract class, node is probaly a strategy
                if (node.CheckModifier("abstract"))
                {
                    //strategy checks
                    StrategyChecks(node);
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
        ///     Function to check if a node is an interface or abstract class, when true node is probaly a strategy
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IResult StrategyChecks(IEntityNode node)
        {
            //standard amount of checks for strategy when using an interface
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
                var abstractStrategyChecks = new List<ElementCheck<IMethod>>
                {
                    new ElementCheck<IMethod>(x => x.CheckModifier("abstract"), "If using a class, the modifier should be abstract. Otherwise, use an interface"),
                    new ElementCheck<IMethod>(x => x.CheckReturnType("void"), "return type should be void")

                };
                amountOfChecks = 4;
                CheckElements(result, node.GetMethods(), abstractStrategyChecks);
            }
            result.Score = (int)(result.Score / amountOfChecks * 100f);
            return result;
        }

        /// <summary>
        ///     Function to do the checks for a normall class (Context or ConcreteStrategy)
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


            if (inheritanceRelations.Count() > 0 && usingRelations.Count() <= 0)
            {
                ConcreteStrategyClassChecks(node, inheritanceRelations);
            }
            if (usingRelations.Count() > 0 && inheritanceRelations.Count() <= 0)
            {
                ContextClassChecks(node, usingRelations);
            }
            return result;
        }

        /// <summary>
        ///     Function to do checks for the concrete strategy class
        /// </summary>
        /// <param name="node"></param>
        /// <param name="inheritanceRelations"></param>
        /// <returns></returns>
        private IResult ConcreteStrategyClassChecks(IEntityNode node, List<IRelation> inheritanceRelations)
        {
            float amountOfChecks = 0;

            foreach (var edge in inheritanceRelations)
            {
                var edgeNode = edge.GetDestination();

                var inheritanceChecks = new List<ElementCheck<IEntityNode>>
                {
                    //check if node is an interface or an abstract class
                    new ElementCheck<IEntityNode>(x => (x.CheckTypeDeclaration(EntityNodeType.Interface)) |
                    ((x.CheckTypeDeclaration(EntityNodeType.Class)) && (x.CheckModifier("abstract"))),"message")
                };
                amountOfChecks += 1;
                CheckElements(result, new List<IEntityNode> { edgeNode }, inheritanceChecks);
            }

            //check if strategy has void method
            var createChecks = new List<ElementCheck<IMethod>>
            {
                new ElementCheck<IMethod>(x => x.CheckReturnType("void"), "return type should be void!")
                //TO DO: check of functie overerft van de strategy functie
                //TO DO: check of de functie de zelfde naam heeft als de overervende functie
            };
            amountOfChecks += 1;
            CheckElements(result, node.GetMethods(), createChecks);

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
