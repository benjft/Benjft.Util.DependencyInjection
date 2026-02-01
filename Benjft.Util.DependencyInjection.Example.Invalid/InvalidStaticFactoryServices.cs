using Benjft.Util.DependencyInjection.Attributes;

namespace Benjft.Util.DependencyInjection.Example.Invalid;

public class InvalidStaticFactoryServices {
    [Service<IInvalidService>]
    public static InvalidService GetService() => new();
    
    [Service<InvalidService>]
    public static InvalidService GenericGetService<T>() => new();
    
    [Service(typeof(InvalidGenericService<>))]
    public static InvalidGenericService<T> GetGenericService<T>() => new();
}
