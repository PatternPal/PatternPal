using System;
using System.Collections.Generic;
using System.Text;
using IDesign.Recognizers.Abstractions;
using System.Linq;
 using IDesign.Recognizers.Models;

namespace IDesign.Recognizers.Checks
{
    public static class ClassChecks
    {
        public static bool ImplementsInterface(IEntityNode node, string name)
        {
            if(HasInterface(node, name))
                {
                return true;
            }
            if (Extends(node))
            {
                return ImplementsInterface(GetExtends(node), name);
            }
            else
            {
                return false;
            }
        }
        public static bool ExtendsClass(IEntityNode node, string name)
        {
            if (Extends(node))
            {
                if(GetExtends(node).GetName() == name)
                {
                    return true;
                }
                return ExtendsClass(GetExtends(node), name);
            }
            else { return false; }
        }

        //helper functions
        public static bool HasInterface(IEntityNode node, string name)
        {
            return node.GetRelations()
                .Where(x => x.GetType() == RelationType.Implemented && x.GetDestination().GetName() == name)
                .Any();
        }
        public static IEntityNode GetExtends(IEntityNode node)
        {
            return node.GetRelations().Where(x => x.GetType() == RelationType.Extends).First().GetDestination();
        }
        public static bool Extends(IEntityNode node)
        {
            return node.GetRelations().Where(x => x.GetType() == RelationType.Extends).Any();
        }
    }
}
