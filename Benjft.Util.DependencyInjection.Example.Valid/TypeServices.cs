using Benjft.Util.DependencyInjection.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Benjft.Util.DependencyInjection.Example.Valid;

public interface IService {
    object? Key { get; }
}

[Service<IService>]
public class Service : IService {
    public object? Key => null;
}

[Service<IService>(ServiceKey = "key1")]
public class KeyedService([ServiceKey] object? key = null) : IService {
    public object? Key => key;
}

public interface IAnotherService {
    object? Key { get; }
}

[Service<IAnotherService>]
public class AnotherService : IAnotherService {
    public object? Key => null;
}

public interface IAThirdService {
    object? Key { get; }
}