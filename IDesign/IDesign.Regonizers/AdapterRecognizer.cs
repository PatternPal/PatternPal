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
                        return x.GetRelationType() == RelationType.Uses;
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

                        }, x => entityNode.GetMethods(), "Is used in every adapter method", GroupCheckType.All)

                    }, x => entityNode.GetFields(), "Adapter has an adaptee field")
                }, node => node.GetRelations(), "Has adaptee")
            }, x => new List<IEntityNode> { entityNode }, "Adapter", GroupCheckType.All);


            var r = adapterCheck.Check(entityNode);

            result.Results = r.GetChildFeedback().ToList();
            return result;
        }
    }
}