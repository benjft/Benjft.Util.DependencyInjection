namespace Benjft.Util.DependencyInjection.Exceptions;

/// <summary>
/// Represents errors that occur when a method marked as a service factory is invalid.
/// This is the base class for more specific factory method exceptions.
/// </summary>
public class InvalidFactoryMethodException : DependencyInjectionAttributeException {
    internal InvalidFactoryMethodException(string? message) 
        : base(message) { }
    internal InvalidFactoryMethodException(string? message, Exception? innerException) 
        : base(message, innerException) { }
}
