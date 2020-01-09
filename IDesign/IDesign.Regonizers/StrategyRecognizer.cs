using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models;
using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;

namespace IDesign.Recognizers
{
    public class StrategyRecognizer : IRecognizer
    {
        Result result;

        public IResult Recognize(IEntityNode node)
        {
            result = new Result();
            var relations = node.GetRelations();

            var strategyPatternCheck = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                //check if node is abstract class or interface
                new ElementCheck<IEntityNode>(x => (x.CheckTypeDeclaration(EntityNodeType.Interface) ) |
                (x.CheckTypeDeclaration(EntityNodeType.Class) && x.CheckModifier("abstract")),"State class should be abstract or an interface!",2),

                //check state node methods
                new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
                {
                    new ElementCheck<IMethod>(x => x.GetBody() == null, "Body should be empty!",1)
                }, x => x.GetMethodsAndProperties(), "Methods: "),

                //check state node used by relations
                new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                {
                    new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfRelationTypes(RelationType.UsedBy, 1),$"Minimal amount of used by relations should be 1",1),

                    //check if field has state as type
                    new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                    {
                        new ElementCheck<IEntityNode>(x => (x.CheckTypeDeclaration(EntityNodeType.Interface)) |
                        (x.CheckTypeDeclaration(EntityNodeType.Class) && x.CheckModifier("abstract")), "type should be an interface or abstract class",1)
                    }, x => new List<IEntityNode> { node},"Used return type:"),
                    
                    //check context class fields
                    new GroupCheck<IEntityNode, IField>(new List<ICheck<IField>>
                    {
                        new ElementCheck<IField>(x => x.CheckMemberModifier("private"), "modifier should be private",0.5f)
                    }, x=> x.GetFields(), "Fields", GroupCheckType.All)

                },x => x.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.UsedBy)).Select(y => y.GetDestination()), "Check used by relations"),

                //check inheritance
                 new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                 {
                     new ElementCheck<IEntityNode>(x => !x.GetRelations().Any(y => y.GetRelationType() ==RelationType.Creates),"Node should not make other strategys",2),
                     new ElementCheck<IEntityNode>(x => !x.GetRelations().Any(y => y.GetRelationType() ==RelationType.Uses),"Node should not have usings",1),
                 },x => x.GetRelations().Where(y => (y.GetRelationType().Equals(RelationType.ExtendedBy)) ||(y.GetRelationType().Equals(RelationType.ImplementedBy))
                ).Select(y => y.GetDestination()), "Node is parent of: ", GroupCheckType.All),

            }, x => new List<IEntityNode> { node }, "Strategy"); ; ;
            result.Results.Add(strategyPatternCheck.Check(node));
            return result;
        }
    }
}
