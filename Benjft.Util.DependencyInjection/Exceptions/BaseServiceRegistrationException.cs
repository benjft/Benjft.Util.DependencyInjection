namespace Benjft.Util.DependencyInjection.Exceptions;

/// <summary>
/// The base exception for all exceptions thrown by <see cref="Benjft.Util.DependencyInjection.Extensions.ServiceExtensions.AddServicesFromAttributes">AddServicesFromAttributes</see>
/// </summary>
public abstract class BaseServiceRegistrationException(string? message, Exception? innerException)
    : Exception(message, innerException);
