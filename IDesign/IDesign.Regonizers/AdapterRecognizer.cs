using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;

namespace IDesign.Recognizers
{
    public class AdapterRecognizer : IRecognizer
    {
        public IResult Recognize(IEntityNode entityNode)
        {
            var result = new Result();
            IRelation currentRelation = null;
            string currentField = null;
            var adapterCheck = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                new ElementCheck<IEntityNode>(
                    x => x.GetRelations().Any(y => y.GetRelationType() == RelationType.Implements || y.GetRelationType() == RelationType.Extends),
                    "Implements interface or extends class"),
                new GroupCheck<IEntityNode, IRelation>(new List<ICheck<IRelation>>
                {
                    //Is used by adapter    
                    new ElementCheck<IRelation>(x =>
                    {
                        currentRelation = x;
                        return x.GetRelationType() == RelationType.Uses ||
                            x.GetRelationType() == RelationType.Extends
                            ;
                    }, "Is used by adapter"),

                    //Adapter has an adaptee field
                    new GroupCheck<IRelation, IField>(new List<ICheck<IField>>
                    {
                        new ElementCheck<IField>(x =>
                        {
                            currentField = x.GetName();
                            return x.CheckFieldType(currentRelation.GetDestination().GetName());
                        }, "Field has adaptee as type"),
                        new GroupCheck<IField, IMethod>(new List<ICheck<IMethod>>
                        {
                            new ElementCheck<IMethod>(x => x.CheckFieldIsUsed(currentField), "Method uses adpatee"),
                            new ElementCheck<IMethod>(x => !x.CheckReturnType(currentField), "Method does not return adaptee"),
                            new ElementCheck<IMethod>(x => x.IsInterfaceMethod(entityNode) || x.CheckModifier("override"),
                                "Method overrides or is ipmlemented"),

                        }, x => entityNode.GetMethods(), "Is used in every adapter method", GroupCheckType.All)

                    }, x => entityNode.GetFields(), "Adapter has an adaptee field")
                }, node => node.GetRelations(), "Has adaptee")
            }, x => new List<IEntityNode> { entityNode }, "Object adapter", GroupCheckType.All);


            var objectAdapterResult = adapterCheck.Check(entityNode);
            
            var classAdapterResult = InheritanceAdapter(entityNode).Check(entityNode);

            if((float)objectAdapterResult.GetTotalChecks() / objectAdapterResult.GetScore()  < (float)classAdapterResult.GetTotalChecks() / classAdapterResult.GetScore())
                result.Results = objectAdapterResult.GetChildFeedback().ToList();
            else
                result.Results = classAdapterResult.GetChildFeedback().ToList();
            return result;
        }

        public GroupCheck<IEntityNode, IEntityNode> InheritanceAdapter(IEntityNode entityNode)
        {

            IRelation currentRelation = null;
            string currentField = null;
            var adapterCheck = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                new ElementCheck<IEntityNode>(
                    x => x.GetRelations().Any(y => y.GetRelationType() == RelationType.Implements || y.GetRelationType() == RelationType.Extends),
                    "Implements interface or extends class"),
                new GroupCheck<IEntityNode, IRelation>(new List<ICheck<IRelation>>
                {
                    //Is used by adapter    
                    new ElementCheck<IRelation>(x =>
                    {
                        currentRelation = x;
                        return x.GetRelationType() == RelationType.Extends ||
                               x.GetRelationType() == RelationType.Implements
                            ;
                    }, "Is used by adapter"),

                        new GroupCheck<IRelation, IMethod>(new List<ICheck<IMethod>>
                        {
                            new ElementCheck<IMethod>(x => x.CheckIfMethodCallsMethodInNode(currentRelation.GetDestination()), "Method uses adpatee"),
                            new ElementCheck<IMethod>(x => !x.CheckReturnType(currentRelation.GetDestination().GetName()), "Method does not return adaptee"),
                            new ElementCheck<IMethod>(x => x.IsInterfaceMethod(entityNode) || x.CheckModifier("override"),
                                "Method doen'st overide or isn't implemented"),

                        }, x =>
                        {
                            return entityNode.GetMethods();
                        }, "Is used in every adapter method", GroupCheckType.All)

                }, node => node.GetRelations(), "Has adaptee")
            }, x => new List<IEntityNode> { entityNode }, "Class adapter", GroupCheckType.All);

            return adapterCheck;

        }
    }
}