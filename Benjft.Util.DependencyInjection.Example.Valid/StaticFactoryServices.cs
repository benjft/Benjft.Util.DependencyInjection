using Benjft.Util.DependencyInjection.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Benjft.Util.DependencyInjection.Example.Valid;

public static class StaticFactoryServices {
    [Service<IService>]
    public static IService GetService() => new Service();
    
    [Service<IService>]
    public static IService GetServiceWithKey([ServiceKey] object? key) => new KeyedService(key);
    
    [Service<IService>]
    public static IService GetServiceWithDependencies(IAnotherService service) => new KeyedService(service);
    
    [Service<IService>]
    public static IService GetServiceWithOptionalParameter(IAnotherService? service = null) => new KeyedService(service);
}
