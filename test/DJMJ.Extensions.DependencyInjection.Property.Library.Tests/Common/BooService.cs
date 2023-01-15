namespace DJMJ.Extensions.DependencyInjection.Property.Library.Tests.Common
{
    /// <inheritdoc cref="IBooService"/>
    public class BooService : IBooService
    {
        /// <inheritdoc/>
        public object Boo()
        {
            return new object();
        }
    }
}
