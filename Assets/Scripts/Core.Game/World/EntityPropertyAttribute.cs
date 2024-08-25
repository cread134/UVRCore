using System;

namespace Core.Game.World
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class EntityPropertyAttribute : System.Attribute
    {
        public EntityPropertyAttribute()
        {
        }
    }
}
