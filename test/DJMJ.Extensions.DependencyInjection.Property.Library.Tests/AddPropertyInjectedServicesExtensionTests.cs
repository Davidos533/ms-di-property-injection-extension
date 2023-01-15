using DJMJ.Extensions.DependencyInjection.Property.Library.Tests.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System.Reflection;

namespace DJMJ.Extensions.DependencyInjection.Property.Library.Tests
{
    /// <summary>
    /// Tests on extension method for IServiceCollection to add property injectable dependecies
    /// </summary>
    public class AddPropertyInjectedServicesExtensionTests
    {
        [Test]
        public void AddPropertyInjectedServices_ServiceCollectionWithoutServicesWithInjectableProperties_UnchangedCollection()
        {
            // arrange
            var serviceCollection = new ServiceCollection();
            
            var createInstance = (IServiceProvider services) =>
            {
                return Activator.CreateInstance<DooService>();
            };

            int serviceCount = 0;

            serviceCollection.AddTransient<IBooService, BooService>();
            serviceCollection.AddTransient<IDooService>(createInstance);

            serviceCount = serviceCollection.Count;

            // act
            serviceCollection.AddPropertyInjectedServices();

            // assert
            var implementationFactoryOfAutoImplementService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IBooService))?.ImplementationFactory;
            Assert.IsNull(implementationFactoryOfAutoImplementService);

            var implementationFactory = serviceCollection.FirstOrDefault(x => x.ImplementationFactory is not null)?.ImplementationFactory;
            Assert.NotNull(implementationFactory);

            var newMethadataToken = implementationFactory.Method.GetMetadataToken();
            var oldMethadataToken = createInstance.Method.GetMetadataToken();

            Assert.True(newMethadataToken == oldMethadataToken);

            Assert.True(serviceCount == serviceCollection.Count);
        }

        [Test]
        public void AddPropertyInjectedServices_ServiceCollectionWithServicesWithInjectableProperties_ChangedCollection()
        {
            // arrange
            var hostBuilder = Host.CreateDefaultBuilder();

            int serviceCount = 0;

            hostBuilder.ConfigureServices((services)=>
            {
                var createInstance = (IServiceProvider services) =>
                {
                    return Activator.CreateInstance<DooService>();
                };

                services.AddTransient<IBooService, BooService>();
                services.AddTransient<IDooService>(createInstance);
                services.AddTransient<IFooService, FooService>();

                serviceCount = services.Count;

                // act
                services.AddPropertyInjectedServices();

                // assert
                var serviceDescriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IFooService));
                Assert.IsNotNull(serviceDescriptor?.ImplementationFactory);
                Assert.True(serviceCount == services.Count);
            });

            var host = hostBuilder.Build();

            var fooService = host.Services.GetRequiredService<IFooService>() as FooService;

            Assert.NotNull(fooService?.BooService);
        }

        [Test]
        public void AddPropertyInjectedServices_ServiceCollectionWithServicesWithInjectablePropertiesByFactoryMethod_ChangedCollection()
        {
            // arrange
            var hostBuilder = Host.CreateDefaultBuilder();
            var createInstance = (IServiceProvider services) =>
            {
                return Activator.CreateInstance<FooService>();
            };

            int serviceCount = 0;

            hostBuilder.ConfigureServices((services) =>
            {
                services.AddTransient<IBooService, BooService>();
                services.AddTransient<IDooService, DooService>();
                services.AddTransient<IFooService>(createInstance);

                serviceCount = services.Count;

                // act
                services.AddPropertyInjectedServices();

                // assert
                var serviceDescriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IFooService));
                Assert.IsNotNull(serviceDescriptor?.ImplementationFactory);

                var oldMethadataToken = createInstance.Method.GetMetadataToken();
                var newMethadataToken = serviceDescriptor.ImplementationFactory.Method.GetMetadataToken();

                Assert.True(oldMethadataToken != newMethadataToken);
                Assert.True(serviceCount == services.Count);
            });

            var host = hostBuilder.Build();

            var fooService = host.Services.GetRequiredService<IFooService>() as FooService;

            Assert.NotNull(fooService?.BooService);
        }

        [Test]
        public void AddPropertyInjectedServices_ServiceCollectionWithServicesWithInjectablePropertiesAsSingleton_ChangedCollection()
        {
            // arrange
            var hostBuilder = Host.CreateDefaultBuilder();

            int serviceCount = 0;

            hostBuilder.ConfigureServices((services) =>
            {
                services.AddSingleton<IBooService, BooService>();
                services.AddSingleton<IDooService, DooService>();
                services.AddSingleton<IFooService, FooService>();

                serviceCount = services.Count;

                // act
                services.AddPropertyInjectedServices();

                // assert
                var serviceDescriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IFooService));
                Assert.IsNotNull(serviceDescriptor?.ImplementationFactory);
            });

            var host = hostBuilder.Build();

            var fooServiceOne = host.Services.GetRequiredService<IFooService>() as FooService;
            var fooServiceTwo = host.Services.GetRequiredService<IFooService>() as FooService;

            Assert.NotNull(fooServiceOne?.BooService);
            Assert.NotNull(fooServiceTwo?.BooService);
            Assert.True(fooServiceOne.Id == fooServiceTwo.Id);
        }
    }
}
