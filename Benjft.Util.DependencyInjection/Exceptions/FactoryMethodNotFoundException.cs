namespace Benjft.Util.DependencyInjection.Exceptions;

public class FactoryMethodNotFoundException : InvalidFactoryMethodException {
    internal FactoryMethodNotFoundException(string? message) 
        : base(message) { }
}
