using Microsoft.Extensions.DependencyInjection;

namespace Benjft.Util.DependencyInjection.Attributes;

/// <summary>
/// Marks a class or method as a <see cref="ServiceLifetime.Transient">Transient</see> service to be registered with the
/// service collection using <see cref="Benjft.Util.DependencyInjection.Extensions.ServiceExtensions.AddServicesFromAttributes">AddServicesFromAttributes</see>.
/// </summary>
public class TransientServiceAttribute : ServiceAttribute {
    /// <summary>
    /// Marks a class or method as a <see cref="ServiceLifetime.Transient">Transient</see> service to be registered with the
    /// service collection using <see cref="Benjft.Util.DependencyInjection.Extensions.ServiceExtensions.AddServicesFromAttributes">AddServicesFromAttributes</see>.
    /// </summary>
    /// <param name="serviceType">
    /// The service type the target should be registered as.
    /// </param>
    /// <param name="serviceLifetime">
    /// The lifetime should the service have in the DI container.
    /// </param>
    public TransientServiceAttribute(Type serviceType, ServiceLifetime serviceLifetime) : base(serviceType, serviceLifetime) { }
    
    /// <summary>
    /// Marks a class or method as a <see cref="ServiceLifetime.Transient">Transient</see> service to be registered with the
    /// service collection using <see cref="Benjft.Util.DependencyInjection.Extensions.ServiceExtensions.AddServicesFromAttributes">AddServicesFromAttributes</see>.
    /// </summary>
    /// <param name="serviceType">
    /// The service type the target should be registered as.
    /// </param>
    public TransientServiceAttribute(Type serviceType) : base(serviceType) { }
}
