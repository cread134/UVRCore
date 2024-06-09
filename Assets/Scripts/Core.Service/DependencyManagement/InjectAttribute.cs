using System;

namespace Core.Service.DependencyManagement
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InjectAttribute : Attribute
    {
    }

}
