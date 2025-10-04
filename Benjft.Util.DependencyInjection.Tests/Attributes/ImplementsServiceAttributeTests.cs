namespace Benjft.Util.DependencyInjection.Tests.Attributes;

public interface IThing {}
public class Thing : IThing {}

public class ImplementsServiceAttributeTests
{
    [Fact]
    public void Constructor_SetsServiceType_WhenServiceTypeProvided()
    {
        var attr = new ImplementsServiceAttribute(typeof(IThing));
        Assert.Equal(typeof(IThing), attr.ServiceType);
        Assert.Null(attr.Lifetime);
    }

    [Fact]
    public void Constructor_SetsServiceTypeAndLifetime_WhenBothProvided()
    {
        var attr = new ImplementsServiceAttribute(typeof(IThing), ServiceLifetime.Singleton);
        Assert.Equal(typeof(IThing), attr.ServiceType);
        Assert.Equal(ServiceLifetime.Singleton, attr.Lifetime);
    }

    [Fact]
    public void GenericConstructors_SetServiceType_ForTypeParameter()
    {
        var attr = new ImplementsServiceAttribute<IThing>();
        Assert.Equal(typeof(IThing), attr.ServiceType);
        var attr2 = new ImplementsServiceAttribute<IThing>(ServiceLifetime.Scoped);
        Assert.Equal(typeof(IThing), attr2.ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, attr2.Lifetime);
    }

    [Fact]
    public void DerivedAttributes_ExposeExpectedLifetimes_Always()
    {
        Assert.Equal(ServiceLifetime.Transient, new ImplementsTransientServiceAttribute(typeof(IThing)).Lifetime);
        Assert.Equal(ServiceLifetime.Scoped, new ImplementsScopedServiceAttribute(typeof(IThing)).Lifetime);
        Assert.Equal(ServiceLifetime.Singleton, new ImplementsSingletonServiceAttribute(typeof(IThing)).Lifetime);
        Assert.Equal(ServiceLifetime.Transient, new ImplementsTransientServiceAttribute<IThing>().Lifetime);
        Assert.Equal(ServiceLifetime.Scoped, new ImplementsScopedServiceAttribute<IThing>().Lifetime);
        Assert.Equal(ServiceLifetime.Singleton, new ImplementsSingletonServiceAttribute<IThing>().Lifetime);
    }
}