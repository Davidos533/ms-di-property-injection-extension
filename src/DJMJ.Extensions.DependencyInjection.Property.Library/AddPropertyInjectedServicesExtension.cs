namespace Microsoft.Extensions.DependencyInjection
{
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Adding property injectable dependecies
    /// </summary>
    public static class AddPropertyInjectedServicesExtension
    {
        private readonly static string PROPERTY_NAME_OF_TYPE_IN_TARGET_IN_FACTORY_METHOD = "serviceImplementationType";

        /// <summary>
        /// Add property injectable depencies for already added services
        /// </summary>
        /// <param name="services">Services</param>
        /// <returns>Services</returns>
        public static IServiceCollection AddPropertyInjectedServices(this IServiceCollection services)
        {
            // Get array for independent iteration
            var servicesList = services.ToArray();

            // Search services with injectable properties to add properties assigning
            foreach (var service in servicesList)
            {
                var implementationFactory = service?.ImplementationFactory;
                var implementationInstance = service?.ImplementationInstance;

                // Getting dependency implementation type
                var implementationType = service?.ImplementationType ??
                    implementationFactory?.GetType()
                        ?.GenericTypeArguments
                        ?.LastOrDefault() ??
                    implementationInstance?.GetType();

                // Getting type from factory method if it's not getted earlier
                if (implementationType == typeof(object))
                {
                    implementationType = implementationFactory?.Target
                        ?.GetType()
                        ?.GetField(PROPERTY_NAME_OF_TYPE_IN_TARGET_IN_FACTORY_METHOD)
                        ?.GetValue(implementationFactory?.Target) as Type;
                }

                if (implementationType is null)
                {
                    continue;
                }

                // Searching injectable properties in dependency implementation type
                var injectableProperties = implementationType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.CanWrite && x.GetCustomAttributes(typeof(InjectAttribute), false).Any())
                    .ToList();

                if (!injectableProperties.Any())
                {
                    continue;
                }

                Func<IServiceProvider, object> createInstanceFunc;

                // Dependency implementation creating by factory
                if (implementationFactory != null)
                {
                    createInstanceFunc = (services) =>
                    {
                        var serviceInstance = implementationFactory.Invoke(services);

                        return serviceInstance;
                    };
                }
                // Dependency implementation singleton (already created)
                else if (implementationInstance != null)
                {
                    createInstanceFunc = (services) =>
                    {
                        return implementationInstance;
                    };
                }
                // Dependency implementation creating by DI
                else
                {
                    createInstanceFunc = (services) =>
                    {
                        // Getting constructor parameters
                        var ctorParameters = implementationType.GetConstructors()
                            .First()
                            .GetParameters()
                            .Select(x => services.GetRequiredService(x.ParameterType))
                            .ToArray();

                        var serviceInstance = Activator.CreateInstance(implementationType, ctorParameters)!;

                        return serviceInstance;
                    };
                }
                
                // Getting service descriptor parameters
                var serviceType = service?.ServiceType ?? implementationType;
                var serviceLifeTime = service!.Lifetime;

                // Replacing dependency in service collection
                services.Replace(new ServiceDescriptor(serviceType, (services) =>
                {
                    // Creating instance
                    var serviceInstance = createInstanceFunc(services);

                    // Assigning dependencies
                    foreach (var injectableProperty in injectableProperties)
                    {
                        // Get dependency implementation
                        var dependencyInstance = services.GetService(injectableProperty.PropertyType);

                        if (dependencyInstance is null)
                        {
                            throw new InvalidOperationException(
                                $"Cannot provide value for property: '{injectableProperty.Name}' on type '{serviceInstance?.GetType()?.FullName}'. No service for type '{injectableProperty.PropertyType.FullName}' has been registered.");
                        }

                        // Set dependency in property
                        injectableProperty.SetValue(serviceInstance, dependencyInstance);
                    }
                    return serviceInstance;
                }, serviceLifeTime));
            }

            return services;
        }
    }
}
