﻿using IDesign.Recognizers.Abstractions;
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
                (x.CheckTypeDeclaration(EntityNodeType.Class) && x.CheckModifier("abstract")),new ResourceMessage("NodeAbstractOrInterface")),

                //check state node methods
                new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
                {
                    new ElementCheck<IMethod>(x => x.CheckReturnType("void"), "return type should be void"),
                    new ElementCheck<IMethod>(x => x.GetBody() == null, new ResourceMessage("MethodBodyEmpty"))
                    //TO DO: if abstract class method must be also abstract!
                }, x => x.GetMethods(), "Methods: "),

                //check state node used by relations
                new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                {
                    new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfRelationTypes(RelationType.UsedBy, 1),new ResourceMessage("NodeUses1")),

                    //check if field has state as type
                    new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                    {
                        new ElementCheck<IEntityNode>(x => (x.CheckTypeDeclaration(EntityNodeType.Interface)) |
                        (x.CheckTypeDeclaration(EntityNodeType.Class) && x.CheckModifier("abstract")), new ResourceMessage("NodeAbstractOrInterface"))
                    }, x => new List<IEntityNode> { node},"Used return type:"),
                    
                    //check context class fields
                    new GroupCheck<IEntityNode, IField>(new List<ICheck<IField>>
                    {
                        //TO DO: check name
                        new ElementCheck<IField>(x => x.CheckMemberModifier("private"), new ResourceMessage("FieldModifierPrivate"))
                    }, x=> x.GetFields(), "Fields", GroupCheckType.All)


                },x => x.GetRelations().Where(y => y.GetRelationType().Equals(RelationType.UsedBy)).Select(y => y.GetDestination()), "Check used by relations"),

                //check inheritance
                 new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                 {
                     new ElementCheck<IEntityNode>(x => !x.GetRelations().Any(y => y.GetRelationType() ==RelationType.Creates),new ResourceMessage("NodeDoesNotCreate")),
                     new ElementCheck<IEntityNode>(x => !x.GetRelations().Any(y => y.GetRelationType() ==RelationType.Uses), new ResourceMessage("NodeDoesNotUse")),
                     
                     new GroupCheck<IEntityNode, IMethod>(new List<ICheck<IMethod>>
                     {
                        //TO DO: check of functie de zelfte parameters heeft als de interface/abstracte klasse functie
                        //TO DO: check of de functie de zelfde naam heeft als de overervende functie  
                     }, x=> x.GetMethods(), "Methods:"),

                 },x => x.GetRelations().Where(y => (y.GetRelationType().Equals(RelationType.ExtendedBy)) ||(y.GetRelationType().Equals(RelationType.ImplementedBy))
                ).Select(y => y.GetDestination()), "Node is parent of: ", GroupCheckType.All),

            }, x => new List<IEntityNode> { node }, "Strategy"); ; ;
            result.Results.Add(strategyPatternCheck.Check(node));
            return result;
        }
    }
}
