namespace Microsoft.Extensions.DependencyInjection
{
    using System;

    /// <summary>
    /// Attribute for injectable property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {

    }
}
