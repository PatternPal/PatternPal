using System;
using System.Collections.Generic;
using IDesign.Recognizers.Checks;
using IDesign.Recognizers.Models.ElementChecks;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers.Models.Checks.Members
{
    public class FieldCheck : AbstractListCheck<IField>
    {
        public FieldCheck Modifiers(params IModifier[] modifiers)
        {
            _checks.Add(new ModifierCheck(modifiers));
            return this;
        }

        public FieldCheck Type(IEntity entity, bool includeGeneric = true)
        {
            return Type(entity.GetName(), includeGeneric);
        }

        public FieldCheck Type(string entity, bool includeGeneric = true)
        {
            if (includeGeneric) return Custom(
                x => x.CheckFieldTypeGeneric(new List<string> { entity }),
                new ResourceMessage("FieldType", entity)
            );
            else return Custom(
                x => x.CheckFieldType(new List<string> { entity }),
                new ResourceMessage("FieldType", entity)
            );
        }

        public FieldCheck Custom(Predicate<IField> predicate, ResourceMessage message)
        {
            _checks.Add(
                new ElementCheck<IField>(
                    predicate,
                    message
                )
            );
            return this;
        }
    }
}
