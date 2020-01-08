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
                    new ResourceMessage("Parent")),
                new GroupCheck<IEntityNode, IRelation>(new List<ICheck<IRelation>>
                {
                    //Is used by adapter    
                    new ElementCheck<IRelation>(x =>
                    {
                        currentRelation = x;
                        return x.GetRelationType() == RelationType.Uses;
                    }, new ResourceMessage("AdapteeIsUsed")),

                    //Adapter has an adaptee field
                    new GroupCheck<IRelation, IField>(new List<ICheck<IField>>
                    {
                        new ElementCheck<IField>(x =>
                        {
                            currentField = x.GetName();
                            return x.CheckFieldType(new List<string>{ currentRelation.GetDestination().GetName() });
                        }, new ResourceMessage("AdapteeField")),
                        //Every method uses the adaptee 
                        new GroupCheck<IField, IMethod>(new List<ICheck<IMethod>>
                        {
                            new ElementCheck<IMethod>(x => x.CheckFieldIsUsed(currentField), new ResourceMessage("AdapterMethodUses")),
                            new ElementCheck<IMethod>(x => !x.CheckReturnType(currentField), new ResourceMessage("AdapterMethodReturnType")),
                            new ElementCheck<IMethod>(x => x.IsInterfaceMethod(entityNode) || x.CheckModifier("override"),
                                new ResourceMessage("MethodOverride")),

                        }, x => entityNode.GetMethods(), new ResourceMessage("AdapterMethod"), GroupCheckType.All)

                    }, x => entityNode.GetFields(), new ResourceMessage("AdapteeField"))
                }, node => node.GetRelations(),  new ResourceMessage("AdapterAdaptee"))
            }, x => new List<IEntityNode> { entityNode }, new ResourceMessage("ObjectAdapter"), GroupCheckType.All);


            var objectAdapterResult = adapterCheck.Check(entityNode);

            var classAdapterResult = InheritanceAdapter(entityNode).Check(entityNode);

            if ((float)objectAdapterResult.GetTotalChecks() / objectAdapterResult.GetScore() < (float)classAdapterResult.GetTotalChecks() / classAdapterResult.GetScore())
                result.Results = objectAdapterResult.GetChildFeedback().ToList();
            else
                result.Results = classAdapterResult.GetChildFeedback().ToList();
            return result;
        }

        public GroupCheck<IEntityNode, IEntityNode> InheritanceAdapter(IEntityNode entityNode)
        {

            IRelation currentRelation = null;
            var adapterCheck = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                new ElementCheck<IEntityNode>(
                    x => x.GetRelations().Any(y => y.GetRelationType() == RelationType.Implements || y.GetRelationType() == RelationType.Extends),
                   new ResourceMessage("Parent")),
                new GroupCheck<IEntityNode, IRelation>(new List<ICheck<IRelation>>
                {
                    //Is used by adapter (parent)    
                    new ElementCheck<IRelation>(x =>
                    {
                        currentRelation = x;
                        return x.GetRelationType() == RelationType.Extends ||
                               x.GetRelationType() == RelationType.Implements
                            ;
                    }, new ResourceMessage("AdapteeExtendsApter")),

                        new GroupCheck<IRelation, IMethod>(new List<ICheck<IMethod>>
                        {
                            new ElementCheck<IMethod>(x => x.CheckIfMethodCallsMethodInNode(currentRelation.GetDestination()), new ResourceMessage("AdapterMethodUses")),
                            new ElementCheck<IMethod>(x => !x.CheckReturnType(currentRelation.GetDestination().GetName()), new ResourceMessage("AdapterMethodReturnType")),
                            new ElementCheck<IMethod>(x => x.IsInterfaceMethod(entityNode) || x.CheckModifier("override"),
                                new ResourceMessage("MethodOverride")),

                        }, x => entityNode.GetMethods(), new ResourceMessage("AdapterMethod"), GroupCheckType.All)

                }, node => node.GetRelations(), new ResourceMessage("AdapterAdaptee"))
            }, x => new List<IEntityNode> { entityNode }, new ResourceMessage("AdapterClass"), GroupCheckType.All);

            return adapterCheck;

        }
    }
}