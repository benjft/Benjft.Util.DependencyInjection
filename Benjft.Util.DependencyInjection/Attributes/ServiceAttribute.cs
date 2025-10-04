using Microsoft.Extensions.DependencyInjection;

namespace Benjft.Util.DependencyInjection.Attributes;

/// <summary>
/// Base attribute to mark a class for automatic registration with the dependency injection container.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ServiceAttribute() : Attribute() {
    /// <summary>
    /// Creates a new ServiceAttribute with a specified lifetime.
    /// </summary>
    /// <param name="lifetime">The lifetime for the registered service.</param>
    public ServiceAttribute(ServiceLifetime lifetime) : this() {
        Lifetime = lifetime;
    }
    
    /// <summary>
    /// Gets the lifetime for the registered service.
    /// If null, the default lifetime specified in the extension method will be used.
    /// </summary>
    public ServiceLifetime? Lifetime { get; } = null;

    /// <summary>
    /// Gets or initializes the name of a static factory method to use for creating instances.
    /// If specified, the factory method will be used instead of the default constructor.
    /// </summary>
    public string? FactoryMethod { get; init; }

    /// <summary>
    /// Gets or initializes the service key for keyed service registration.
    /// If null, the service will be registered as a non-keyed service.
    /// </summary>
    public object? ServiceKey { get; init; }

    /// <summary>
    /// Gets or initializes the order in which services are registered when multiple implementations exist.
    /// Services with lower order values are registered first.
    /// </summary>
    public int Order { get; init; } = 0;
}

/// <summary>
/// Attribute to mark a class for automatic registration as a transient service.
/// A new instance will be created each time the service is requested.
/// </summary>
public class TransientServiceAttribute() : ServiceAttribute(ServiceLifetime.Transient);

/// <summary>
/// Attribute to mark a class for automatic registration as a scoped service.
/// One instance will be created per dependency injection scope.
/// </summary>
public class ScopedServiceAttribute() : ServiceAttribute(ServiceLifetime.Scoped);

/// <summary>
/// Attribute to mark a class for automatic registration as a singleton service.
/// A single instance will be created for the entire application lifetime.
/// </summary>
public class SingletonServiceAttribute() : ServiceAttribute(ServiceLifetime.Singleton);