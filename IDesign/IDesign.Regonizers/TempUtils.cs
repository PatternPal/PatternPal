using System;
using System.Collections.Generic;
using System.Linq;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers
{
    [Obsolete]
    public static class TempUtils
    {
        public static IEnumerable<IField> GetFields(this IEntity entity)
        {
            if (entity is IClass cls)
            {
                return cls.GetFields();
            }

            return Array.Empty<IField>();
        }

        public static IEnumerable<IMethod> GetConstructors(this IEntity entity)
        {
            if (entity is IClass cls)
            {
                return cls.GetConstructors().Select(c => c.AsMethod());
            }

            return Array.Empty<IMethod>();
        }
    }
}
