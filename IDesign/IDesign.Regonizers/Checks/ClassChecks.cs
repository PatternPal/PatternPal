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
        public static bool ImplementsInterface(this IEntityNode node, string name)
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
        public static bool ExtendsClass(this IEntityNode node, string name)
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
        public static bool HasInterface(this IEntityNode node, string name)
        {
            return node.GetRelations()
                .Any(x => x.GetType() == RelationType.ImplementedBy && x.GetDestination().GetName() == name);
        }
        public static IEntityNode GetExtends(this IEntityNode node)
        {
            return node.GetRelations().Where(x => x.GetType() == RelationType.Extends).FirstOrDefault().GetDestination();
        }
        public static bool Extends(this IEntityNode node)
        {
            return node.GetRelations().Any(x => x.GetType() == RelationType.Extends);
        }
        public static IEntityNode GetEdgeNode(this IEntityNode node, string name)
        {
            return node.GetRelations().Where(x => x.GetDestination().GetName() == name).FirstOrDefault().GetDestination();
        }
    }
}
