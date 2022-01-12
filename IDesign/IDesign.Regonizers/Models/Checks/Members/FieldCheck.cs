using System.Collections.Generic;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers.Models.Checks.Members
{
    public class FieldCheck : AbstractMemberListCheck<IField, FieldCheck>
    {
        //TODO make a interface for real typed members
        //TODO add include generic to MethodCheck and PropertyCheck
        public FieldCheck Type(IEntity entity, bool includeGeneric = true)
        {
            return Type(entity.GetName(), includeGeneric);
        }

        public FieldCheck Type(string entity, bool includeGeneric = true)
        {
            if (includeGeneric)
                return Custom(
                    x => x.CheckFieldTypeGeneric(new List<string> { entity }),
                    new ResourceMessage("FieldType", entity)
                );
            else return base.Type(entity);
        }

        protected override FieldCheck This() => this;
    }
}
