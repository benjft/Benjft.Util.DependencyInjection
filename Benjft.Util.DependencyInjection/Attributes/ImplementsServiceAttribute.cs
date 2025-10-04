using Microsoft.Extensions.DependencyInjection;

namespace Benjft.Util.DependencyInjection.Attributes;

    /// <summary>
    /// Attribute to mark a class for automatic registration as an implementation of a specified service type.
    /// </summary>
    public class ImplementsServiceAttribute : ServiceAttribute {
    /// <summary>
    /// Gets the service type that this class implements.
    /// </summary>
    public Type ServiceType { get; }

    /// <summary>
    /// Creates a new ImplementsServiceAttribute with a specified service type and lifetime.
    /// </summary>
    /// <param name="serviceType">The service type that this class implements.</param>
    /// <param name="lifetime">The lifetime for the registered service.</param>
    public ImplementsServiceAttribute(Type serviceType, ServiceLifetime lifetime) : base(lifetime) {
        ServiceType = serviceType;
    }

    /// <summary>
    /// Creates a new ImplementsServiceAttribute with a specified service type.
    /// </summary>
    /// <param name="serviceType">The service type that this class implements.</param>
    public ImplementsServiceAttribute(Type serviceType) : base() {
        ServiceType = serviceType;
    }
}

    /// <summary>
    /// Generic attribute to mark a class for automatic registration as an implementation of the specified service type.
    /// </summary>
    /// <typeparam name="TService">The service type that this class implements.</typeparam>
    public class ImplementsServiceAttribute<TService> : ImplementsServiceAttribute {
    /// <summary>
    /// Creates a new ImplementsServiceAttribute for the specified service type.
    /// </summary>
    public ImplementsServiceAttribute() : base(typeof(TService)) { }

    /// <summary>
    /// Creates a new ImplementsServiceAttribute for the specified service type with a specified lifetime.
    /// </summary>
    /// <param name="lifetime">The lifetime for the registered service.</param>
    public ImplementsServiceAttribute(ServiceLifetime lifetime) : base(typeof(TService), lifetime) { }
}

/// <summary>
/// Attribute to mark a class for automatic registration as a transient implementation of a specified service type.
/// </summary>
/// <param name="serviceType">The service type that this class implements.</param>
public class ImplementsTransientServiceAttribute(Type serviceType) : ImplementsServiceAttribute(serviceType, ServiceLifetime.Transient);

/// <summary>
/// Attribute to mark a class for automatic registration as a scoped implementation of a specified service type.
/// </summary>
/// <param name="serviceType">The service type that this class implements.</param>
public class ImplementsScopedServiceAttribute(Type serviceType) : ImplementsServiceAttribute(serviceType, ServiceLifetime.Scoped);

/// <summary>
/// Attribute to mark a class for automatic registration as a singleton implementation of a specified service type.
/// </summary>
/// <param name="serviceType">The service type that this class implements.</param>
public class ImplementsSingletonServiceAttribute(Type serviceType) : ImplementsServiceAttribute(serviceType, ServiceLifetime.Singleton);

/// <summary>
/// Generic attribute to mark a class for automatic registration as a transient implementation of type <typeparamref name="TService"/>.
/// </summary>
/// <typeparam name="TService">The service type that this class implements.</typeparam>
public class ImplementsTransientServiceAttribute<TService>() : ImplementsServiceAttribute<TService>(ServiceLifetime.Transient);

/// <summary>
/// Generic attribute to mark a class for automatic registration as a scoped implementation of type <typeparamref name="TService"/>.
/// </summary>
/// <typeparam name="TService">The service type that this class implements.</typeparam>
public class ImplementsScopedServiceAttribute<TService>() : ImplementsServiceAttribute<TService>(ServiceLifetime.Scoped);

/// <summary>
/// Generic attribute to mark a class for automatic registration as a singleton implementation of type <typeparamref name="TService"/>.
/// </summary>
/// <typeparam name="TService">The service type that this class implements.</typeparam>
public class ImplementsSingletonServiceAttribute<TService>() : ImplementsServiceAttribute<TService>(ServiceLifetime.Singleton);
