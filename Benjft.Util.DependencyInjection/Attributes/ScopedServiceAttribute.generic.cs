using Microsoft.Extensions.DependencyInjection;

namespace Benjft.Util.DependencyInjection.Attributes;

/// <summary>
/// Marks a class or method as a <see cref="ServiceLifetime.Scoped">Scoped</see> service to be registered with the
/// service collection using <see cref="Benjft.Util.DependencyInjection.Extensions.ServiceExtensions.AddServicesFromAttributes">AddServicesFromAttributes</see>.
/// </summary>
/// <typeparam name="TService">
/// The service type the target should be registered as.
/// </typeparam>
public class ScopedServiceAttribute<TService> : ServiceAttribute<TService> {
    /// <summary>
    /// Marks a class or method as a <see cref="ServiceLifetime.Scoped">Scoped</see> service to be registered with the
    /// service collection using <see cref="Benjft.Util.DependencyInjection.Extensions.ServiceExtensions.AddServicesFromAttributes">AddServicesFromAttributes</see>.
    /// </summary>
    /// <typeparam name="TService">
    /// The service type the target should be registered as.
    /// </typeparam>
    /// <param name="serviceLifetime">
    /// The lifetime should the service have in the DI container.
    /// </param>
    public ScopedServiceAttribute(ServiceLifetime serviceLifetime) : base(serviceLifetime) { }
    
    /// <summary>
    /// Marks a class or method as a <see cref="ServiceLifetime.Scoped">Scoped</see> service to be registered with the
    /// service collection using <see cref="Benjft.Util.DependencyInjection.Extensions.ServiceExtensions.AddServicesFromAttributes">AddServicesFromAttributes</see>.
    /// </summary>
    /// <typeparam name="TService">
    /// The service type the target should be registered as.
    /// </typeparam>
    public ScopedServiceAttribute() : base() { }
}
