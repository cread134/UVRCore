using System;

namespace Core.DevTools.Scripting
{
    public class GizmoGroup
    {
        public enum GroupType
        {
            ComponentIndicators,
            ComponentData,
        }
        
        public static void Scope(GroupType[] groupTypes, Action gizmoAction)
        {
        }
    }
}
