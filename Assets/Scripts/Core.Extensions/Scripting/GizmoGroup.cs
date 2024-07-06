using System;

namespace Core.DevTools.Scripting
{
    public class GizmoGroup
    {
        [Flags]
        public enum GroupType
        {
            ComponentIndicators = 0,
            ComponentData = 1,
        }

        public static GroupType ActiveType = GroupType.ComponentIndicators | GroupType.ComponentData;
        
        public static void Scope(GroupType groupTypes, Action gizmoAction)
        {
            if ((ActiveType & groupTypes) == 0)
            {
                gizmoAction();
                return;
            }
        }
    }
}
