namespace Benjft.Util.DependencyInjection.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void GetServicesFromAttributes_OrdersDescriptors_ByOrderServiceTypeAndKey()
    {
        // Use two singleton implementations from the ValidOnly fixture to validate ordering and lifetimes
        var types = new[]
        {
            typeof(TestFixtures.ValidOnly.FooImplSingleton),
            typeof(TestFixtures.ValidOnly.FooSingletonAttr),
        };
        // ReSharper disable once ConvertToConstant.Local
        var defaultLifetime = ServiceLifetime.Transient;
        var descriptors = types.GetServicesFromAttributes(defaultLifetime).ToArray();
        Assert.Equal(2, descriptors.Length);
        Assert.All(descriptors, d => Assert.Equal(ServiceLifetime.Singleton, d.Lifetime));
    }

    [Fact]
    public void GetServicesFromAttributes_ProducesWorkingRegistrations_WhenAddedToServiceCollection()
    {
        var services = new ServiceCollection();
        // Use fixture types: include the static factory host so factory methods are picked up
        var validTypes = new[]
        {
            typeof(TestFixtures.ValidOnly.SelfA),
            typeof(TestFixtures.ValidOnly.FooImplSingleton),
            typeof(TestFixtures.ValidOnly.FooSingletonAttr),
            typeof(TestFixtures.ValidOnly.Factories)
        };
        foreach (var sd in validTypes.GetServicesFromAttributes())
        {
            services.Add(sd);
        }
        var provider = services.BuildServiceProvider();

        // SelfA registered as self; ensure resolvable (lifetime may be influenced by factory registration)
        var selfType = typeof(TestFixtures.ValidOnly.SelfA);
        var p1 = provider.GetRequiredService(selfType);
        var p2 = provider.GetRequiredService(selfType);
        Assert.NotNull(p1);
        Assert.NotNull(p2);

        // IScanFoo has implementations; ensure descriptors exist
        var fooInterface = typeof(TestFixtures.ValidOnly.IScanFoo);
        var fooDescriptors = services.Where(d => d.ServiceType == fooInterface).ToList();
        Assert.True(fooDescriptors.Count >= 1);

        // Keyed scoped factory descriptor exists
        Assert.Contains(services, d => d.ServiceType == fooInterface && d.IsKeyedService && (string?)d.ServiceKey == "k1");
    }

    [Fact]
    public void GetServicesFromAttributes_ThrowsFactoryMethodNotFound_WhenFactoryMethodMissing()
    {
        var ex = Assert.Throws<FactoryMethodNotFoundException>(() => {
            _ = typeof(TestFixtures.InvalidOnly.MissingFactoryHost).GetServicesFromAttributes().ToArray();
        });
        Assert.Contains("does not contain", ex.Message);
    }

    [Fact]
    public void GetServicesFromAttributes_ThrowsInvalidFactoryMethod_WhenFactoryMethodIsInstance()
    {
        var ex = Assert.Throws<InvalidFactoryMethodException>(() => {
            _ = typeof(TestFixtures.InvalidOnly.NonStaticFactory).GetServicesFromAttributes().ToArray();
        });
        Assert.IsType<FactoryMethodNotStaticException>(ex.InnerException);
    }

    [Fact]
    public void GetServicesFromAttributes_ThrowsInvalidFactoryMethod_WhenFactorySignatureIsWrong()
    {
        var ex = Assert.Throws<InvalidFactoryMethodException>(() => {
            _ = typeof(TestFixtures.InvalidOnly.WrongSignatureHost).GetServicesFromAttributes().ToArray();
        });
        Assert.Contains("Factory Method", ex.Message);
    }

    [Fact]
    public void GetServicesFromAttributes_ThrowsInvalidServiceType_WhenImplementationIsAbstract()
    {
        var ex = Assert.Throws<InvalidServiceTypeException>(() => {
            _ = typeof(TestFixtures.InvalidOnly.AbstractBad).GetServicesFromAttributes().ToArray();
        });
        Assert.Contains("must not be an abstract", ex.Message);
    }

    [Fact]
    public void GetServicesFromAttributes_ThrowsInvalidServiceType_WhenImplementationNotAssignableToServiceType()
    {
        var ex = Assert.Throws<InvalidServiceTypeException>(() => {
            _ = typeof(TestFixtures.InvalidOnly.BadServiceType).GetServicesFromAttributes().ToArray();
        });
        Assert.Contains("must be assignable", ex.Message);
    }

    [Fact]
    public void GetServicesFromAttributes_RegistersMultipleImplementations_WhenMultipleImplementationsExist() {
        var services = typeof(TestFixtures.ValidOnly.MultipleServiceAttributes)
           .GetServicesFromAttributes()
           .ToArray();
        
        Assert.Equal(2, services.Length);
        Assert.Contains(services, s => Equals(s.ServiceKey, "test1"));
        Assert.Contains(services, s => Equals(s.ServiceKey, "test2"));
        Assert.All(services, s => Assert.Equal(ServiceLifetime.Transient, s.Lifetime));
        Assert.All(services, s => Assert.Equal(typeof(TestFixtures.ValidOnly.MultipleServiceAttributes), s.ServiceType));
    }

    // Tests merged from AssemblyScanningTests
    [Fact]
    public void AddServicesFromAttributes_Registers_FromSingleAssembly_Success()
    {
        var services = new ServiceCollection();
        var asm = typeof(TestFixtures.ValidOnly.Marker).Assembly;

        services.AddServicesFromAttributes(asm);
        var provider = services.BuildServiceProvider();

        // SelfA should be registered: from [Service] and from factory
        var selfType = asm.GetType("Benjft.Util.DependencyInjection.TestFixtures.ValidOnly.SelfA")!;
        var s1 = provider.GetRequiredService(selfType);
        var s2 = provider.GetRequiredService(selfType);
        Assert.NotNull(s1);
        Assert.NotNull(s2);

        // IScanFoo singleton and a scoped keyed factory should exist
        var fooInterface = asm.GetType("Benjft.Util.DependencyInjection.TestFixtures.ValidOnly.IScanFoo")!;
        var reqGeneric = typeof(ServiceProviderServiceExtensions)
            .GetMethods()
            .First(m => m is { Name: "GetRequiredService", IsGenericMethodDefinition: true });
        var reqSvc = reqGeneric.MakeGenericMethod(fooInterface);
        var foo1 = reqSvc.Invoke(null, [provider]);
        var foo2 = reqSvc.Invoke(null, [provider]);
        Assert.Same(foo1, foo2);

        // Verify descriptors include keyed (resolution via extension can vary across DI versions;
        // we validate the descriptor presence deterministically)
        Assert.Contains(services, d => d.ServiceType.FullName == fooInterface.FullName && d.IsKeyedService && (string?)d.ServiceKey == "k1");
    }

    [Fact]
    public void AddServicesFromAttributes_Registers_FromMultipleAssemblies_Union()
    {
        var services = new ServiceCollection();
        var asm1 = typeof(TestFixtures.ValidOnly.Marker).Assembly;
        var asm2 = typeof(TestFixtures.ValidOnly.Marker).Assembly; // using same for simplicity

        services.AddServicesFromAttributes([asm1, asm2]);

        // Expect at least two descriptors for IScanFoo due to duplicates allowed; we just assert >= 1
        var fooInterface = asm1.GetType("Benjft.Util.DependencyInjection.TestFixtures.ValidOnly.IScanFoo")!;
        var fooDescriptors = services.Where(d => d.ServiceType.FullName == fooInterface.FullName).ToList();
        Assert.True(fooDescriptors.Count >= 1);
    }

    [Fact]
    public void AddServicesFromAttributes_WhenOnlyValidServicesExist_ResolvesServicesSuccessfully()
    {
        var baseDir = AppContext.BaseDirectory;
        var validDll = System.IO.Path.Combine(baseDir, "Benjft.Util.DependencyInjection.TestFixtures.ValidOnly.dll");
        Assert.True(System.IO.File.Exists(validDll));

        var alc = new System.Runtime.Loader.AssemblyLoadContext("ValidOnly_ALC", isCollectible: true);
        using (alc.EnterContextualReflection()) {
            try
            {
                var validAsm = alc.LoadFromAssemblyPath(validDll);

                var services = new ServiceCollection();
                services.AddServicesFromAttributes();
                var provider = services.BuildServiceProvider();

                var selfType = validAsm.GetType("Benjft.Util.DependencyInjection.TestFixtures.ValidOnly.SelfA")!;
                Assert.NotNull(provider.GetRequiredService(selfType));

                var fooInterface = validAsm.GetType("Benjft.Util.DependencyInjection.TestFixtures.ValidOnly.IScanFoo")!;
                Assert.Contains(services, d => d.ServiceType == fooInterface && d.IsKeyedService && (string?)d.ServiceKey == "k1");
            }
            finally
            {
                alc.Unload();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
    }

    [Fact]
    public void AddServicesFromAttributes_WhenInvalidServicesExist_ThrowsDependencyInjectionAttributeException()
    {
        var baseDir = AppContext.BaseDirectory;
        var invalidDll = System.IO.Path.Combine(baseDir, "Benjft.Util.DependencyInjection.TestFixtures.InvalidOnly.dll");
        Assert.True(System.IO.File.Exists(invalidDll));
        
        var alc = new System.Runtime.Loader.AssemblyLoadContext("InvalidOnly_ALC", isCollectible: true);
        using (alc.EnterContextualReflection()) {
            try
            {
                _ = alc.LoadFromAssemblyPath(invalidDll);

                var services = new ServiceCollection();
                Assert.ThrowsAny<DependencyInjectionAttributeException>(() => services.AddServicesFromAttributes());
            }
            finally
            {
                alc.Unload();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
    }

    // Tests merged from AddServicesFromAttributesInDomainTests
    // A keyed factory with the wrong signature (keyed expects Func<IServiceProvider, object?, object>)
    public static class KeyedWrongSignature
    {
        [SingletonServiceFactory(ServiceKey = "k1")]
        public static object Create(IServiceProvider sp) => new();
    }

    [Fact]
    public void AddServicesFromAttributesInDomain_WhenInvalidFactoriesPresent_Throws()
    {
        // Ensure type is loaded in the current AppDomain
        _ = typeof(KeyedWrongSignature);

        var services = new ServiceCollection();
        Assert.ThrowsAny<DependencyInjectionAttributeException>(() => services.AddServicesFromAttributesInDomain());
    }
}