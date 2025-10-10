namespace Benjft.Util.DependencyInjection.Exceptions;

/// <summary>
/// Thrown when a method marked with <see cref="Attributes.ServiceFactoryAttribute"/> has an incompatible delegate signature.
/// For example, keyed factory methods must match <c>Func&lt;IServiceProvider, object?, object&gt;</c>,
/// and non-keyed methods must match <c>Func&lt;IServiceProvider, object&gt;</c>.
/// </summary>
public class FactoryMethodHasWrongSignatureException : InvalidFactoryMethodException {
    internal FactoryMethodHasWrongSignatureException(string? message, Exception? innerException) 
        : base(message, innerException) { }
}
