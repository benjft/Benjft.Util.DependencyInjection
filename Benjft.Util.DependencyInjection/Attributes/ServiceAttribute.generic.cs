using Microsoft.Extensions.DependencyInjection;

namespace Benjft.Util.DependencyInjection.Attributes;


/// <summary>
/// Marks a class or method as a service to be registered with the service collection using <see cref="Benjft.Util.DependencyInjection.Extensions.ServiceExtensions.AddServicesFromAttributes">AddServicesFromAttributes</see>.
/// </summary>
/// <typeparam name="TService">
/// The service type the target should be registered as.
/// </typeparam>
public class ServiceAttribute<TService> : ServiceAttribute {
    /// <summary>
    /// Marks a class or method as a service to be registered with the service collection using <see cref="Benjft.Util.DependencyInjection.Extensions.ServiceExtensions.AddServicesFromAttributes">AddServicesFromAttributes</see>.
    /// </summary>
    /// <typeparam name="TService">
    /// The service type the target should be registered as.
    /// </typeparam>
    /// <param name="serviceLifetime">
    /// The lifetime should the service have in the DI container.
    /// </param>
    public ServiceAttribute(ServiceLifetime serviceLifetime) : base(typeof(TService), serviceLifetime) { }
    
    /// <summary>
    /// Marks a class or method as a service to be registered with the service collection using <see cref="Benjft.Util.DependencyInjection.Extensions.ServiceExtensions.AddServicesFromAttributes">AddServicesFromAttributes</see>.
    /// </summary>
    /// <typeparam name="TService">
    /// The service type the target should be registered as.
    /// </typeparam>
    public ServiceAttribute() : base(typeof(TService)) { }
}
