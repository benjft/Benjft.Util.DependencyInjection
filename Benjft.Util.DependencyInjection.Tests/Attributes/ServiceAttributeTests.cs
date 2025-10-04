namespace Benjft.Util.DependencyInjection.Tests.Attributes;

public class ServiceAttributeTests
{
    [Fact]
    public void AttributeUsage_ReturnsClassAndAllowMultiple_WhenRead()
    {
        var usage = typeof(ServiceAttribute).GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Cast<AttributeUsageAttribute>()
            .Single();

        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
        Assert.True(usage.AllowMultiple);
    }

    private class Dummy {}

    [Fact]
    public void Constructor_SetsLifetime_WhenLifetimeProvided()
    {
        var attr = new ServiceAttribute(ServiceLifetime.Scoped);
        Assert.Equal(ServiceLifetime.Scoped, attr.Lifetime);
    }

    [Fact]
    public void Constructor_SetsDefaultPropertyValues_WhenUsingParameterlessCtor()
    {
        var attr = new ServiceAttribute();
        Assert.Null(attr.Lifetime);
        Assert.Null(attr.FactoryMethod);
        Assert.Null(attr.ServiceKey);
        Assert.Equal(0, attr.Order);
    }

    [Fact]
    public void DerivedAttributes_ExposeExpectedLifetimes_Always()
    {
        Assert.Equal(ServiceLifetime.Transient, new TransientServiceAttribute().Lifetime);
        Assert.Equal(ServiceLifetime.Scoped, new ScopedServiceAttribute().Lifetime);
        Assert.Equal(ServiceLifetime.Singleton, new SingletonServiceAttribute().Lifetime);
    }
}