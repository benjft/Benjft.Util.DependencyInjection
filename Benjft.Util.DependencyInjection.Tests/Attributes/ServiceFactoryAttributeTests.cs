namespace Benjft.Util.DependencyInjection.Tests.Attributes;

public class ServiceFactoryAttributeTests
{
    [Fact]
    public void Constructors_SetProperties_AsDocumented()
    {
        // default
        var a0 = new ServiceFactoryAttribute();
        Assert.Null(a0.ServiceTypeOverride);
        Assert.Null(a0.Lifetime);
        Assert.Null(a0.ServiceKey);
        Assert.Equal(0, a0.Order);

        // with service type
        var a1 = new ServiceFactoryAttribute(typeof(string));
        Assert.Equal(typeof(string), a1.ServiceTypeOverride);
        Assert.Null(a1.Lifetime);

        // with lifetime
        var a2 = new ServiceFactoryAttribute(ServiceLifetime.Scoped);
        Assert.Equal(ServiceLifetime.Scoped, a2.Lifetime);
        Assert.Null(a2.ServiceTypeOverride);

        // with service type and lifetime
        var a3 = new ServiceFactoryAttribute(typeof(int), ServiceLifetime.Singleton);
        Assert.Equal(typeof(int), a3.ServiceTypeOverride);
        Assert.Equal(ServiceLifetime.Singleton, a3.Lifetime);
    }

    [Fact]
    public void InitOnlyProperties_CanBeSet_ViaObjectInitializer()
    {
        var attr = new ServiceFactoryAttribute
        {
            ServiceKey = "k",
            Order = 5,
        };
        Assert.Equal("k", attr.ServiceKey);
        Assert.Equal(5, attr.Order);
    }

    [Fact]
    public void DerivedFactoryAttributes_ExposeExpectedLifetimes_AndGenericCtors()
    {
        Assert.Equal(ServiceLifetime.Transient, new TransientServiceFactoryAttribute().Lifetime);
        Assert.Equal(ServiceLifetime.Scoped, new ScopedServiceFactoryAttribute().Lifetime);
        Assert.Equal(ServiceLifetime.Singleton, new SingletonServiceFactoryAttribute().Lifetime);

        // with explicit service types
        Assert.Equal(ServiceLifetime.Transient, new TransientServiceFactoryAttribute(typeof(string)).Lifetime);
        Assert.Equal(ServiceLifetime.Scoped, new ScopedServiceFactoryAttribute(typeof(string)).Lifetime);
        Assert.Equal(ServiceLifetime.Singleton, new SingletonServiceFactoryAttribute(typeof(string)).Lifetime);

        // generic versions just need to be constructible
        var gT = new TransientServiceFactoryAttribute<string>();
        var gS = new ScopedServiceFactoryAttribute<string>();
        var gSi = new SingletonServiceFactoryAttribute<string>();
        Assert.NotNull(gT);
        Assert.NotNull(gS);
        Assert.NotNull(gSi);
    }
}
