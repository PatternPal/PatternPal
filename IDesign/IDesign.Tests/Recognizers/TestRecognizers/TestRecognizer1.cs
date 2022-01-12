using System;
using System.Collections.Generic;
using System.Text;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;

namespace IDesign.Tests.Recognizers.TestRecognizers
{
    public class TestRecognizer : IRecognizer
    {
        public IResult Recognize(IEntity node)
        {
            var result = new Result();

            var groupCheckList = new List<ICheck<IEntity>>()
            {
                new ElementCheck<IEntity>(
                    x => x.CheckMinimalAmountOfMethods(3),
                    "MethodAmountThree",
                    CheckType.KnockOut
                ),

                new ElementCheck<IEntity>(
                    x => x.CheckIsAbstractClassOrInterface(),
                    "NodeAbstractOrInterface",
                    CheckType.Optional
                ),

                new ElementCheck<IEntity>(
                    x => x.CheckRelationType(RelationType.ExtendedBy) ||
                         x.CheckRelationType(RelationType.ImplementedBy),
                    "NodeImplementedOrInherited",
                    CheckType.Optional
                ),
            };

            var groupCheck = new GroupCheck<IEntity, IEntity>(
                groupCheckList,
                x => new List<IEntity> { node },
                ""
            );

            result.Results.Add(groupCheck.Check(node));

            return result;
        }
    }
}
