using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models.Output;
using IDesign.Recognizers.Models;
using IDesign.Recognizers.Models.ElementChecks;
using System;

namespace IDesign.Recognizers
{
    public class ObserverRecognizer : IRecognizer
    {
        public IResult Recognize(IEntityNode entityNode)
        {
            var result1 = ObserverWithSubjectInterfaceCheck(entityNode);
            var result2 = ObserverWithoutSubjectInterfaceCheck(entityNode);

            if (result1.GetScore() >= result2.GetScore())
                return result1;
            else 
                return result2;
        }

        /// <summary>
        ///     Checks if a given node is an implementation of observer pattern with an subject interface
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IResult ObserverWithSubjectInterfaceCheck(IEntityNode node)
        {
            var result = new Result();

            //Tests for implementation of observer pattern with an subject interface
            var checks = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfMethods(1), new ResourceMessage("MethodAny")),
                new ElementCheck<IEntityNode>(x => x.CheckRelationType(RelationType.ImplementedBy), new ResourceMessage("NodeImplementedByAny")),
                new ElementCheck<IEntityNode>(x => x.CheckRelationType(RelationType.UsedBy), new ResourceMessage("NodeUsedByAny")),

                new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                {
                    new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfMethods(3), new ResourceMessage("MethodAmountThree")),
                    new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfMethodsWithParameter(new List<string> { node.GetName() }, 2), new ResourceMessage("ObserverMethodParameters", new []{node.GetName()})),
                    new ElementCheck<IEntityNode>(x => x.CheckRelationType(RelationType.ImplementedBy), new ResourceMessage("NodeImplementedByAny")),

                    new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                    {
                        new ElementCheck<IEntityNode>(x => x.CheckTypeDeclaration(EntityNodeType.Class), new ResourceMessage("NodeTypeClass")),
                        new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfMethods(3), new ResourceMessage("MethodAmountThree")),

                        new GroupCheck<IEntityNode, IField>(new List<ICheck<IField>>
                        {
                            new ElementCheck<IField>(x => x.CheckFieldType(new List<string> { $"List<{ node.GetName() }>" }), new ResourceMessage("FieldType", new []{node.GetName()})),
                        }, d => d.GetFields(), "Concrete subject field"),
                    }, c => c.GetRelations().Where(x => x.GetRelationType() == RelationType.ImplementedBy).Select(x => x.GetDestination()), "Concrete subject", GroupCheckType.All),
                }, b => node.GetRelations().Where(x => x.GetRelationType() == RelationType.UsedBy).Select(x => x.GetDestination()).ToList(), "Subject interface"),
            }, a => new List<IEntityNode> { node }, "Observer");

            result.Results.Add(checks.Check(node));

            return result;
        }

        /// <summary>
        ///     Checks if a given node is an implementation of observer pattern without an subject interface
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IResult ObserverWithoutSubjectInterfaceCheck(IEntityNode node)
        {
            var result = new Result();
            
            //Tests for implementation of observer pattern without an subject interface
            var checks = new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
            {
                new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfMethods(1), new ResourceMessage("MethodAny")),
                new ElementCheck<IEntityNode>(x => x.CheckRelationType(RelationType.ImplementedBy), new ResourceMessage("NodeImplementedByAny")),
                new ElementCheck<IEntityNode>(x => x.CheckRelationType(RelationType.UsedBy), new ResourceMessage("NodeImplementedByAny")),

                new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                {
                    new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfMethods(3),new ResourceMessage("MethodAmountThree")),
                    new ElementCheck<IEntityNode>(x => x.CheckMinimalAmountOfMethodsWithParameter(new List<string> { node.GetName() }, 2), new ResourceMessage("ObserverMethodParameters", new []{node.GetName()})),
                    new ElementCheck<IEntityNode>(x => x.CheckEntityNodeType(EntityNodeType.Class), new ResourceMessage("NodeImplementedByAny")),

                    new GroupCheck<IEntityNode, IField>(new List<ICheck<IField>>
                    {
                        new ElementCheck<IField>(x => x.CheckFieldType(new List<string> { $"List<{ node.GetName() }>" }), new ResourceMessage("FieldType", new []{node.GetName()})),
                    }, d => d.GetFields(), "Concrete subject fields"),

                    //Check to make sure a subject with an interface doesn't get mistaken for a concrete subject without an subject interface
                    new GroupCheck<IEntityNode, IEntityNode>(new List<ICheck<IEntityNode>>
                    {
                        new ElementCheck<IEntityNode>(x => x.CheckMaximumAmountOfMethodsWithParameter(new List<string> { node.GetName() }, 0), new ResourceMessage("MethodParameterNone", new[]{node.GetName()})),
                    }, c => c.GetRelations().Where(x => x.GetRelationType() == RelationType.ImplementedBy).Select(x => x.GetDestination()).ToList(), "Subject interface"),
                }, b => node.GetRelations().Where(x => x.GetRelationType() == RelationType.UsedBy).Select(x => x.GetDestination()).ToList(), "Concrete subject"),
            }, a => new List<IEntityNode> { node }, "Observer");

            result.Results.Add(checks.Check(node));

            return result;
        }
    }
}
