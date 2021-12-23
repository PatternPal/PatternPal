using System;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers.Models.Checks.Members
{
    public class MethodCheck : AbstractListCheck<IMethod>
    {
        public MethodCheck Modifiers(params IModifier[] modifiers)
        {
            _checks.Add(new ModifierCheck(modifiers));
            return this;
        }

        public MethodCheck ReturnType(IEntity entity)
        {
            return ReturnType(entity.GetName());
        }

        public MethodCheck ReturnType(string entity)
        {
            return Custom(
                x => x.CheckReturnType(entity), new ResourceMessage("MethodReturnType", entity)
            );
        }

        public MethodCheck Custom(Predicate<IMethod> predicate, ResourceMessage message)
        {
            _checks.Add(
                new ElementCheck<IMethod>(
                    predicate,
                    message
                )
            );
            return this;
        }
    }
}
