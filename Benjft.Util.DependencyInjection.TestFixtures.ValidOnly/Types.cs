using System.Diagnostics.CodeAnalysis;
using Benjft.Util.DependencyInjection.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly:ExcludeFromCodeCoverage]

namespace Benjft.Util.DependencyInjection.TestFixtures.ValidOnly;

// Marker type to get this assembly
public sealed class Marker {}

public interface IScanFoo {}
public class ScanFoo : IScanFoo {}

[Service]
public class SelfA {}

[ImplementsSingletonService(typeof(IScanFoo))]
public class FooImplSingleton : IScanFoo {}

[ImplementsSingletonService(typeof(IScanFoo))]
public class FooSingletonAttr : IScanFoo {}

public static class Factories
{
    [SingletonServiceFactory]
    public static SelfA CreateSelf(IServiceProvider sp) => new SelfA();

    [ScopedServiceFactory(typeof(IScanFoo), ServiceKey = "k1", Order = 5)]
    public static IScanFoo CreateKeyed(IServiceProvider sp, object? key) => new ScanFoo();
}

[TransientService(ServiceKey = "test1", Order = 1)]
[TransientService(ServiceKey = "test2", Order = 2)]
public class MultipleServiceAttributes {}