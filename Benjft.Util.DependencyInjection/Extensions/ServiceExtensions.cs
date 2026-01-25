using System.Reflection;
using Benjft.Util.DependencyInjection.ServiceLocators;
using Microsoft.Extensions.DependencyInjection;

namespace Benjft.Util.DependencyInjection.Extensions;

/// <summary>
/// Extension methods for finding services and adding them to the IServiceCollection
/// </summary>
public static class ServiceExtensions {
    extension(IServiceCollection serviceCollection) {
        /// <summary>
        /// Scan the passed assemblies for service registrations using <see cref="Benjft.Util.DependencyInjection.Attributes.ServiceAttribute">ServiceAttribute</see>.
        /// </summary>
        /// <param name="assemblies">
        /// The assemblies to find services in.
        /// </param>
        /// <param name="defaultLifetime">
        /// The lifetime to use for services that have none specified.
        /// Defaults to <see cref="ServiceLifetime.Transient">Transient</see>.
        /// </param>
        /// <returns>
        /// The ServiceCollection for method chaining.
        /// </returns>
        public IServiceCollection AddServicesFromAttributes(Assembly[] assemblies, ServiceLifetime defaultLifetime = ServiceLifetime.Transient) {
            var serviceLocators = new List<IServiceLocator> {
                new TypeServiceLocator(defaultLifetime), new StaticFactoryServiceLocator(defaultLifetime),
            };

            var serviceDescriptors = from assembly in assemblies
                                     from serviceLocator in serviceLocators
                                     from orderAndDescriptor in serviceLocator.FindServicesInAssembly(assembly)
                                     orderby orderAndDescriptor.registrationOrder
                                     select orderAndDescriptor.serviceDescriptor;

            foreach (var descriptor in serviceDescriptors) {
                serviceCollection.Add(descriptor);
            }
            return serviceCollection;
        }
    }
}
