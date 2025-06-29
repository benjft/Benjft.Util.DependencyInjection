using System.Diagnostics.CodeAnalysis;
using Benjft.Util.DependencyInjection.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly:ExcludeFromCodeCoverage]

namespace Benjft.Util.DependencyInjection.TestFixtures.InvalidOnly;

// Marker type to get this assembly
public sealed class Marker {}

public interface IBar {}
public interface IFoo {}

[ImplementsService(typeof(IBar))]
public abstract class AbstractBad : IBar {}

[Service(FactoryMethod = nameof(NotStatic))]
public class NonStaticFactory
{
    public NonStaticFactory() {}
    public NonStaticFactory(IServiceProvider sp) {}
    public NonStaticFactory NotStatic(IServiceProvider sp) => new NonStaticFactory();
}

public static class WrongSignatureHost
{
    [SingletonServiceFactory]
    public static int CreateWrong() => 42;
}

// Missing factory method referred by attribute
[ImplementsService(typeof(IBar), FactoryMethod = "Create", Order = 10)]
public class MissingFactoryHost
{
    public static object Wrong(IServiceProvider sp) => new object();
}

// Implementation type not assignable to the service type
[ImplementsService(typeof(IBar))]
public class BadServiceType : IFoo { }