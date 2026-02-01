namespace Benjft.Util.DependencyInjection.Exceptions;

/// <summary>
/// Thrown when a ServiceAttribute is used in a way that is not supported.
/// </summary>
public class RegistrationNotSupportedException(string message) : BaseServiceRegistrationException(message, null);
