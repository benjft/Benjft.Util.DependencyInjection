namespace Benjft.Util.DependencyInjection.Exceptions;

/// <summary>
/// Thrown when a service implementation type is not assignable to the service type
/// </summary>
public class InvalidImplementationTypeException : BaseServiceRegistrationException {
    /// <summary>
    /// Thrown when a service implementation type is not assignable to the service type
    /// </summary>
    public InvalidImplementationTypeException(Type implementationType, Type serviceType) 
        : base($"Implementation type {implementationType.Name} is not assignable to {serviceType.Name}", null) { }
}
