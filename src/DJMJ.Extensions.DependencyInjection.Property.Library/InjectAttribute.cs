using System;

namespace DJMJ.Extensions.DependencyInjection.Property
{
    /// <summary>
    /// Attribute for injectable property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {

    }
}
