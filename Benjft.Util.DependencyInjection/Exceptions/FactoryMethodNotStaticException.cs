namespace Benjft.Util.DependencyInjection.Exceptions;

public class FactoryMethodNotStaticException : InvalidFactoryMethodException {
    internal FactoryMethodNotStaticException(string? message) 
        : base(message) { }
}
