namespace Benjft.Util.DependencyInjection.Exceptions;

/// <summary>
/// Thrown when a method marked with <see cref="Attributes.ServiceFactoryAttribute"/> is not static.
/// Factory methods must be static so they can be invoked without an instance.
/// </summary>
public class FactoryMethodNotStaticException : InvalidFactoryMethodException {
    internal FactoryMethodNotStaticException(string? message) 
        : base(message) { }
}
