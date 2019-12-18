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
            var relations = node.GetRelations();

            IEntityNode entityNode = null;

            var statePatternCheck = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                //check if node is abstract class or interface
                new ElementCheck<IEntityNode>(x => (x.CheckTypeDeclaration(EntityNodeType.Interface) ) |
                (x.CheckTypeDeclaration(EntityNodeType.Class) && x.CheckModifier("abstract")),"State class should be abstract or an interface!"),

                //check state node methods
                new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
                {
                    new ElementCheck<IMethod>(x => x.CheckReturnType("void"), "return type should be void"),
                    new ElementCheck<IMethod>(x => x.GetBody() == null, "Body should be empty!")
                    //TO DO: if abstract class method must be also abstract!
                }, x => x.GetMethods(), "Methods: "),

                //check state node used by relations
                new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                {
                    new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfRelationTypes(RelationType.UsedBy, 1),$"Minimal amount of used by relations should be 1"),

                    //check if field has state as type
                    new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                    {
                        new ElementCheck<IEntityNode>(x => (x.CheckTypeDeclaration(EntityNodeType.Interface)) |
                        (x.CheckTypeDeclaration(EntityNodeType.Class) && x.CheckModifier("abstract")), "type should be an interface or abstract class")
                    }, x => new List<IEntityNode> { node},"Used return type:"),
                    
                    //check context class fields
                    new GroupCheck<IEntityNode, IField>(new List<ICheck<IField>>
                    {
                        //TO DO: check name
                        new ElementCheck<IField>(x => x.CheckMemberModifier("private"), "modifier should be private")
                    }, x=> x.GetFields(), "Fields", GroupCheckType.All)


                },x => x.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.UsedBy)).Select(y => y.GetDestination()), "Check used by relations"),

                //check inheritance
                 new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                 {
                        new ElementCheck<IEntityNode>(x => {entityNode = x; return x.GetMethods().Any(); }, "Has functions"),

                        new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                        {
                            new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
                            {
                                new ElementCheck<IMethod>(x => x.CheckReturnType("void"), "Return type should be void"),
                                new ElementCheck<IMethod>(x => (x.CheckCreationType(entityNode.GetName()) && !(x.CheckCreationType(node.GetName()))), $"new state should not be itself")
                                //TO DO: check of functie de zelfte parameters heeft als de interface/abstracte klasse functie
                                //TO DO: check of de functie de zelfde naam heeft als de overervende functie  
                            }, x=> x.GetMethods(), "Methods:"),

                        },x => entityNode.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.Creates)).Select(y => y.GetDestination()), "Check creates relations"),

                 },x => x.GetRelations().Where(y => (y.GetRelationType().Equals(RelationType.ExtendedBy)) ||(y.GetRelationType().Equals(RelationType.ImplementedBy))
                ).Select(y => y.GetDestination()), "Node is parent of: ", GroupCheckType.All),

            }, x => new List<IEntityNode> { node }, "State"); ;
            result.Results.Add(statePatternCheck.Check(node));
            return result;
        }
    }
}
