namespace Benjft.Util.DependencyInjection.Exceptions;

public class FactoryMethodHasWrongSignatureException : InvalidFactoryMethodException {
    internal FactoryMethodHasWrongSignatureException(string? message, Exception? innerException) 
        : base(message, innerException) { }
}
