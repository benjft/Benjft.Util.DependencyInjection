namespace Benjft.Util.DependencyInjection.Exceptions;

/// <summary>
/// Thrown when a factory method specified by attribute configuration cannot be found on the target type.
/// </summary>
public class FactoryMethodNotFoundException : InvalidFactoryMethodException {
    internal FactoryMethodNotFoundException(string? message) 
        : base(message) { }
}
