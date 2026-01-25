using Microsoft.Extensions.DependencyInjection;

namespace Benjft.Util.DependencyInjection.Attributes;

/// <summary>
/// Marks a class or method as a service to be registered with the service collection using <see cref="Benjft.Util.DependencyInjection.Extensions.ServiceExtensions.AddServicesFromAttributes">AddServicesFromAttributes</see>.
/// </summary>
/// <param name="serviceType">
/// The service type the target should be registered as.
/// </param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class ServiceAttribute(Type serviceType) : Attribute {
    /// <summary>
    /// Marks a class or method as a service to be registered with the service collection using <see cref="Benjft.Util.DependencyInjection.Extensions.ServiceExtensions.AddServicesFromAttributes">AddServicesFromAttributes</see>.
    /// </summary>
    /// <param name="serviceType">
    /// The service type the target should be registered as.
    /// </param>
    /// <param name="serviceLifetime">
    /// The lifetime should the service have in the DI container.
    /// </param>
    public ServiceAttribute(Type serviceType, ServiceLifetime serviceLifetime) : this(serviceType) {
        ServiceLifetime = serviceLifetime;
    }
    
    /// <summary>
    /// The service type the target should be registered as.
    /// </summary>
    public Type ServiceType { get; private set;} = serviceType;
    
    /// <summary>
    /// The lifetime should the service have in the DI container.
    /// If null, the default lifetime when scanning assemblies will be used.
    /// </summary>
    public ServiceLifetime? ServiceLifetime { get; private set; }
    
    /// <summary>
    /// The order in which services should be added to the DI container.
    /// Only relevant for when there are multiple services of the same service type as the last registered will be the
    /// default for <see cref="IServiceProvider.GetService"/>
    /// </summary>
    public int RegistrationOrder { get; set; } = 0;
    
    /// <summary>
    /// The service key that identifies this service.
    /// </summary>
    public object? ServiceKey { get; set; } = null;
}
