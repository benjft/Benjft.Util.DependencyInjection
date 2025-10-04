namespace Benjft.Util.DependencyInjection.Exceptions;

public class InvalidServiceTypeException : DependencyInjectionAttributeException {
    internal InvalidServiceTypeException(string? message) 
        : base(message) { }
}
