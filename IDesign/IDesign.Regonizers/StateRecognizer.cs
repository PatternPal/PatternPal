using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models;
using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;

namespace IDesign.Recognizers
{
    public class StateRecognizer : IRecognizer
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
                InterfaceStateChecks(node);
            }
            if ((node.CheckTypeDeclaration(EntityNodeType.Class) && node.CheckModifier("abstract")))
            {
                StateChecks(node);
                AbstractClassStateChecks(node);
            }
            return result;
        }

        /// <summary>
        ///     Function to check if pattern is state
        /// </summary>
        /// <param name="node"></param>
        private void StateChecks(IEntityNode node)
        {
            var relations = node.GetRelations();

            //create list with only Extends and Implements relations
            var inheritanceRelations = relations.Where(x => (x.GetRelationType() == RelationType.ImplementedBy) ||
            (x.GetRelationType() == RelationType.ExtendedBy)).ToList();

            foreach (var edge in relations.Where(x => x.GetRelationType() == RelationType.UsedBy).ToList())
            {
                var edgeRelations = edge.GetDestination().GetRelations();
                var usingEdgeRelations = edgeRelations.Where(x => x.GetRelationType() == RelationType.UsedBy).ToList();
                ContextClassChecks(edge.GetDestination(), usingEdgeRelations);
            }

            foreach (var edge in inheritanceRelations)
            {
                var edgeRelations = edge.GetDestination().GetRelations();

                //create list with only creates relations
                var createsRelations = edgeRelations.Where(x => x.GetRelationType() == RelationType.Creates).ToList();
                ConcreteStateClassChecks(edge.GetDestination(), createsRelations);
            }
        }


        /// <summary>
        ///     Function to do checks for a node that is probaly a state, implemented as an interface
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private void InterfaceStateChecks(IEntityNode node)
        {

            var checkType = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                new ElementCheck<IEntityNode>(x => x.CheckTypeDeclaration(EntityNodeType.Interface), "The modifier should be abstract. Otherwise, use an interface")

            }, x => new List<IEntityNode> { node }, "Class meets the requirements: ");

            result.Results.Add(checkType.Check(node));
            var checkMethods = new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
            {
                new ElementCheck<IMethod>(x => x.CheckReturnType("void"), "return type should be void")

            }, x => x.GetMethods(), "Class has a method: ");
            result.Results.Add(checkMethods.Check(node));
        }

        /// <summary>
        ///     Function to do checks for a node that is probaly a state, implemented as an abstract class
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private void AbstractClassStateChecks(IEntityNode node)
        {
            //check if node is an interface or an abstract class, cannot be both
            var checkType = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                new ElementCheck<IEntityNode>(x => x.CheckModifier("abstract"), "If using a class, the modifier should be abstract. Otherwise, use an interface")

            }, x => new List<IEntityNode> { node }, "Class meets the requirements: ");
            result.Results.Add(checkType.Check(node));

            var abstractStateCheck = new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
            {
                new ElementCheck<IMethod>(x => x.CheckModifier("abstract"), "Modifier of the method should be abstract!"),
                new ElementCheck<IMethod>(x => x.CheckReturnType("void"), "return type should be void"),
                new ElementCheck<IMethod>(x => x.GetBody() == null, "Body should be empty!")
            }, x => x.GetMethods(), "Class has a method: ");
            result.Results.Add(abstractStateCheck.Check(node));
        }


        /// <summary>
        ///     Function to do checks for the concrete state class
        /// </summary>
        /// <param name="node"></param>
        /// <param name="inheritanceRelations"></param>
        /// <returns></returns>
        private void ConcreteStateClassChecks(IEntityNode node, List<IRelation> creationRelations)
        {
            foreach (var edge in creationRelations)
            {
                var edgeNode = edge.GetDestination();
                //check if state makes other state in handle method and check if the return type is void
                var createCheck = new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
                {
                    new ElementCheck<IMethod>(x => x.CheckReturnType("void"), "Return type should be void"),
                    new ElementCheck<IMethod>(x => (x.CheckCreationType(edgeNode.GetName())) &&(!x.CheckCreationType(node.GetName())), $"new state should not be itself")
                    //TO DO: check of functie de zelfte parameters heeft als de interface/abstracte klasse functie
                    //TO DO: check of de functie de zelfde naam heeft als de overervende functie                                

                }, x => x.GetMethods(), "ConcreteState has methods where:");

                result.Results.Add(createCheck.Check(node));
            }
        }

        /// <summary>
        ///     Function to do checks for the context class
        /// </summary>
        /// <param name="node"></param>
        /// <param name="usingRelations"></param>
        private void ContextClassChecks(IEntityNode node, List<IRelation> usingRelations)
        {
            foreach (var edge in usingRelations)
            {
                var edgeNode = edge.GetDestination();

                if ((edgeNode.CheckTypeDeclaration(EntityNodeType.Interface)) |
                    (edgeNode.CheckTypeDeclaration(EntityNodeType.Class) && edgeNode.CheckModifier("abstract")))
                {
                    //check if state makes other state in handle method and check if the return type is void
                    var fieldCheck = new GroupCheck<IEntityNode, IField>(new List<ICheck<IField>>
                    {
                        new ElementCheck<IField>(x => x.CheckFieldType(new List<string>(){ edgeNode.GetName() }),$"{node.GetName()} must be equal to {edgeNode.GetName()}"),
                        new ElementCheck<IField>(x => x.CheckMemberModifier("private"), "Modifier must be private")
                    }, x => x.GetFields(), "Context has method where:");
                    result.Results.Add(fieldCheck.Check(node));
                }
            }
        }
    }
}
