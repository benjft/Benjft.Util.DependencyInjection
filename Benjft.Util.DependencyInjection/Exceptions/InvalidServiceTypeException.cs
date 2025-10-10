namespace Benjft.Util.DependencyInjection.Exceptions;

/// <summary>
/// Thrown when a type marked with a service attribute specifies an invalid service type.
/// For example, the service type may be abstract or incompatible with the implementing type.
/// </summary>
public class InvalidServiceTypeException : DependencyInjectionAttributeException {
    internal InvalidServiceTypeException(string? message) 
        : base(message) { }
}
