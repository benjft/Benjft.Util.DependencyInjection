using Microsoft.Extensions.DependencyInjection;

namespace Benjft.Util.DependencyInjection.Attributes;

/// <summary>
/// Attribute to mark a static method as a factory method for dependency injection.
/// The method will be used to create instances of the service type when requested from the service provider.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ServiceFactoryAttribute() : Attribute {
    /// <summary>
    /// Creates a new ServiceFactoryAttribute with a specified service type.
    /// </summary>
    /// <param name="serviceType">The service type that will be used instead of the method return type.</param>
    public ServiceFactoryAttribute(Type serviceType) : this() {
        ServiceTypeOverride = serviceType;
    }
    
    /// <summary>
    /// Creates a new ServiceFactoryAttribute with a specified lifetime.
    /// </summary>
    /// <param name="lifetime">The lifetime for the registered service.</param>
    public ServiceFactoryAttribute(ServiceLifetime lifetime) : this() {
        Lifetime = lifetime;
    }
    
    
    /// <summary>
    /// Creates a new ServiceFactoryAttribute with a specified service type and lifetime.
    /// </summary>
    /// <param name="serviceType">The service type that will be used instead of the method return type.</param>
    /// <param name="lifetime">The lifetime for the registered service.</param>
    public ServiceFactoryAttribute(Type serviceType, ServiceLifetime lifetime) : this() {
        ServiceTypeOverride = serviceType;
        Lifetime = lifetime;
    }
    
    /// <summary>
    /// Gets or initializes the service type that will be registered instead of the method return type.
    /// If null, the method return type will be used as the service type.
    /// </summary>
    public Type? ServiceTypeOverride { get; init; } = null;

    /// <summary>
    /// Gets or initializes the lifetime for the registered service.
    /// If null, the default lifetime specified in the extension method will be used.
    /// </summary>
    public ServiceLifetime? Lifetime { get; init; } = null;

    /// <summary>
    /// Gets or initializes the service key for keyed service registration.
    /// If null, the service will be registered as a non-keyed service.
    /// </summary>
    public object? ServiceKey { get; init; } = null;

    /// <summary>
    /// Gets or initializes the order in which services are registered when multiple implementations exist.
    /// Services with lower order values are registered first.
    /// </summary>
    public int Order { get; init; } = 0;
}

/// <summary>
/// Attribute to mark a static method as a factory method for a transient service.
/// The method will be used to create new instances of the service type each time it is requested.
/// </summary>
public class TransientServiceFactoryAttribute : ServiceFactoryAttribute {
    /// <summary>
    /// Creates a new TransientServiceFactoryAttribute.
    /// </summary>
    public TransientServiceFactoryAttribute() : base(ServiceLifetime.Transient) { }

    /// <summary>
    /// Creates a new TransientServiceFactoryAttribute with a specified service type.
    /// </summary>
    /// <param name="serviceType">The service type that will be used instead of the method return type.</param>
    public TransientServiceFactoryAttribute(Type serviceType) : base(serviceType, ServiceLifetime.Transient) { }
}

/// <summary>
/// Attribute to mark a static method as a factory method for a scoped service.
/// The method will be used to create one instance of the service type per scope.
/// </summary>
public class ScopedServiceFactoryAttribute : ServiceFactoryAttribute {
    /// <summary>
    /// Creates a new ScopedServiceFactoryAttribute.
    /// </summary>
    public ScopedServiceFactoryAttribute() : base(ServiceLifetime.Scoped) { }

    /// <summary>
    /// Creates a new ScopedServiceFactoryAttribute with a specified service type.
    /// </summary>
    /// <param name="serviceType">The service type that will be used instead of the method return type.</param>
    public ScopedServiceFactoryAttribute(Type serviceType) : base(serviceType, ServiceLifetime.Scoped) { }
}

/// <summary>
/// Attribute to mark a static method as a factory method for a singleton service.
/// The method will be used to create a single instance of the service type for the entire application.
/// </summary>
public class SingletonServiceFactoryAttribute : ServiceFactoryAttribute {
    /// <summary>
    /// Creates a new SingletonServiceFactoryAttribute.
    /// </summary>
    public SingletonServiceFactoryAttribute() : base(ServiceLifetime.Singleton) { }

    /// <summary>
    /// Creates a new SingletonServiceFactoryAttribute with a specified service type.
    /// </summary>
    /// <param name="serviceType">The service type that will be used instead of the method return type.</param>
    public SingletonServiceFactoryAttribute(Type serviceType) : base(serviceType, ServiceLifetime.Singleton) { }
}

/// <summary>
/// Generic attribute to mark a static method as a factory method for a transient service of type <typeparamref name="TService"/>.
/// </summary>
/// <typeparam name="TService">The service type to register.</typeparam>
public class TransientServiceFactoryAttribute<TService>() : TransientServiceFactoryAttribute(typeof(TService));

/// <summary>
/// Generic attribute to mark a static method as a factory method for a scoped service of type <typeparamref name="TService"/>.
/// </summary>
/// <typeparam name="TService">The service type to register.</typeparam>
public class ScopedServiceFactoryAttribute<TService>() : ScopedServiceFactoryAttribute(typeof(TService));

/// <summary>
/// Generic attribute to mark a static method as a factory method for a singleton service of type <typeparamref name="TService"/>.
/// </summary>
/// <typeparam name="TService">The service type to register.</typeparam>
public class SingletonServiceFactoryAttribute<TService>() : SingletonServiceFactoryAttribute(typeof(TService));