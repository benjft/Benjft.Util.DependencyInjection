using System.Diagnostics.CodeAnalysis;
using Benjft.Util.DependencyInjection.Attributes;

[assembly:ExcludeFromCodeCoverage]

namespace Benjft.Util.DependencyInjection.TestFixtures.InvalidOnly;

public interface IBar;
public interface IFoo;

[ImplementsService(typeof(IBar))]
public abstract class AbstractBad : IBar;

[Service(FactoryMethod = nameof(NotStatic))]
public class NonStaticFactory
{
    public NonStaticFactory() {}
    public NonStaticFactory(IServiceProvider sp) {}
    #pragma warning disable CA1822
    public NonStaticFactory NotStatic(IServiceProvider _) => new();
    #pragma warning restore CA1822
}

public static class WrongSignatureHost
{
    [SingletonServiceFactory]
    public static int CreateWrong() => 42;
}

// Missing factory method referred by attribute
[ImplementsService(typeof(IBar), FactoryMethod = "Create", Order = 10)]
public class MissingFactoryHost;

// Implementation type not assignable to the service type
[ImplementsService(typeof(IBar))]
public class BadServiceType : IFoo;