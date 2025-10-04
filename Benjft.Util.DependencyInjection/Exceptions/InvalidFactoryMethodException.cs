namespace Benjft.Util.DependencyInjection.Exceptions;

public class InvalidFactoryMethodException : DependencyInjectionAttributeException {
    internal InvalidFactoryMethodException(string? message) 
        : base(message) { }
    internal InvalidFactoryMethodException(string? message, Exception? innerException) 
        : base(message, innerException) { }
}
