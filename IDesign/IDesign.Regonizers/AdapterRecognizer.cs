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

            var objectAdapterResult = GetObjectAdapterCheck(entityNode).Check(entityNode);
            var classAdapterResult = GetInheritanceAdapterCheck(entityNode).Check(entityNode);

            if ((float)objectAdapterResult.GetTotalChecks() / objectAdapterResult.GetScore() < (float)classAdapterResult.GetTotalChecks() / classAdapterResult.GetScore())
                result.Results = objectAdapterResult.GetChildFeedback().ToList();
            else
                result.Results = classAdapterResult.GetChildFeedback().ToList();
            return result;
        }

        private GroupCheck<IEntityNode, IEntityNode> GetObjectAdapterCheck(IEntityNode entityNode)
        {
            IRelation currentRelation = null;
            string currentField = null;
            var adapterCheck = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                GetImplementsInterfaceOrExtendsClassCheck(),
                new GroupCheck<IEntityNode, IRelation>(new List<ICheck<IRelation>>
                {
                    //Is used by adapter
                    new ElementCheck<IRelation>(x =>
                    {
                        currentRelation = x;
                        return x.GetRelationType() == RelationType.Uses;
                    }, "AdapteeIsUsed"),

                    //Adapter has an adaptee field
                    new GroupCheck<IRelation, IField>(new List<ICheck<IField>>
                    {
                        new ElementCheck<IField>(x =>
                        {
                            currentField = x.GetName();
                            return x.CheckFieldType(new List<string>{ currentRelation.GetDestination().GetName() });
                        }, "AdapteeField"),
                        //Every method uses the adaptee
                        new GroupCheck<IField, IMethod>(new List<ICheck<IMethod>>
                        {
                            new ElementCheck<IMethod>(x => x.CheckFieldIsUsed(currentField), "AdapterMethodUses", 2),
                            new ElementCheck<IMethod>(x => !x.CheckReturnType(currentField), "AdapterMethodReturnType", 1),
                            new ElementCheck<IMethod>(x => x.IsInterfaceMethod(entityNode) || x.CheckModifier("override"),
                                "MethodOverride", 1)

                        }, x => entityNode.GetMethods(), "AdapterMethod", GroupCheckType.Median)

                    }, x => entityNode.GetFields(), "AdapteeField", GroupCheckType.Median)
                }, node => node.GetRelations(),  "AdapterAdaptee")
            }, x => new List<IEntityNode> { entityNode }, "ObjectAdapter", GroupCheckType.All);


            return adapterCheck;
        }

        private GroupCheck<IEntityNode, IEntityNode> GetInheritanceAdapterCheck(IEntityNode entityNode)
        {
            IRelation currentRelation = null;
            var adapterCheck = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                GetImplementsInterfaceOrExtendsClassCheck()                ,
                new GroupCheck<IEntityNode, IRelation>(new List<ICheck<IRelation>>
                {
                    //Is used by adapter (parent)
                    new ElementCheck<IRelation>(x =>
                    {
                        currentRelation = x;
                        return x.GetRelationType() == RelationType.Extends ||
                               x.GetRelationType() == RelationType.Implements
                            ;
                    }, "AdapteeExtendsApter", 2),

                        new GroupCheck<IRelation, IMethod>(new List<ICheck<IMethod>>
                        {
                            new ElementCheck<IMethod>(x => x.CheckIfMethodCallsMethodInNode(currentRelation.GetDestination()), new ResourceMessage("AdapterMethodUses"), 2),
                            new ElementCheck<IMethod>(x => !x.CheckReturnType(currentRelation.GetDestination().GetName()), new ResourceMessage("AdapterMethodReturnType"), 1),
                            new ElementCheck<IMethod>(x => x.IsInterfaceMethod(entityNode) || x.CheckModifier("override"),
                                "MethodOverride", 1),

                        }, x => entityNode.GetMethods(), "AdapterMethod", GroupCheckType.All)

                }, node => node.GetRelations(), "AdapterAdaptee")
            }, x => new List<IEntityNode> { entityNode }, "AdapterClass", GroupCheckType.All);

            return adapterCheck;
        }

        private ElementCheck<IEntityNode> GetImplementsInterfaceOrExtendsClassCheck()
        {
            return new ElementCheck<IEntityNode>(
                    x => x.GetRelations().Any(y => y.GetRelationType() == RelationType.Implements || y.GetRelationType() == RelationType.Extends),
                    "Parent");
        }
    }
}
