using Microsoft.Extensions.DependencyInjection;

namespace DJMJ.Extensions.DependencyInjection.Property.Library.Tests.Common
{
    /// <inheritdoc cref="IFooService"/>
    public class FooService : IFooService
    {
        public Guid Id { get; set; }

        [Inject]
        public IBooService BooService { get; set; }

        public FooService()
        {
            Id = Guid.NewGuid();
        }

        /// <inheritdoc/>
        public object Foo()
        {
            return new object();
        }
    }
}
