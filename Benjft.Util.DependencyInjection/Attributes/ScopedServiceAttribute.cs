using Microsoft.Extensions.DependencyInjection;

namespace Benjft.Util.DependencyInjection.Attributes;

/// <summary>
/// Marks a class or method as a <see cref="ServiceLifetime.Scoped">Scoped</see> service to be registered with the
/// service collection using <see cref="Benjft.Util.DependencyInjection.Extensions.ServiceExtensions.AddServicesFromAttributes">AddServicesFromAttributes</see>.
/// </summary>
public class ScopedServiceAttribute : ServiceAttribute {
    /// <summary>
    /// Marks a class or method as a <see cref="ServiceLifetime.Scoped">Scoped</see> service to be registered with the
    /// service collection using <see cref="Benjft.Util.DependencyInjection.Extensions.ServiceExtensions.AddServicesFromAttributes">AddServicesFromAttributes</see>.
    /// </summary>
    /// <param name="serviceType">
    /// The service type the target should be registered as.
    /// </param>
    /// <param name="serviceLifetime">
    /// The lifetime should the service have in the DI container.
    /// </param>
    public ScopedServiceAttribute(Type serviceType, ServiceLifetime serviceLifetime) : base(serviceType, serviceLifetime) { }
    
    /// <summary>
    /// Marks a class or method as a <see cref="ServiceLifetime.Scoped">Scoped</see> service to be registered with the
    /// service collection using <see cref="Benjft.Util.DependencyInjection.Extensions.ServiceExtensions.AddServicesFromAttributes">AddServicesFromAttributes</see>.
    /// </summary>
    /// <param name="serviceType">
    /// The service type the target should be registered as.
    /// </param>
    public ScopedServiceAttribute(Type serviceType) : base(serviceType) { }
}
