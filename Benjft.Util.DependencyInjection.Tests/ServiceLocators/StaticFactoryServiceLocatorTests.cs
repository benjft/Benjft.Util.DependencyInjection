using System.Reflection;
using Benjft.Util.DependencyInjection.Attributes;
using Benjft.Util.DependencyInjection.Example.Invalid;
using Benjft.Util.DependencyInjection.Example.Valid;
using Benjft.Util.DependencyInjection.Exceptions;
using Benjft.Util.DependencyInjection.ServiceLocators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using IService = Benjft.Util.DependencyInjection.Example.Valid.IService;

namespace Benjft.Util.DependencyInjection.Tests.ServiceLocators;

public class StaticFactoryServiceLocatorTests {
    private static readonly StaticFactoryServiceLocator ServiceLocator = new(ServiceLifetime.Transient);
    
    [Fact]
    public void FindServicesInAssembly_ReturnsServiceDescriptors_WhenAllRegistrationsValid() {
        var assembly = Assembly.Load(new AssemblyName("Benjft.Util.DependencyInjection.Example.Valid"));
        
        var serviceDescriptors = ServiceLocator.FindServicesInAssembly(assembly).ToArray();
        
        Assert.NotEmpty(serviceDescriptors);
    }
    
    [Fact]
    public void GetServiceDescriptor_ThrowsInvalidImplementationTypeException_WhenFactoryMethodReturnTypeIsNotAssignableToServiceType() {
        var factoryMethod = typeof(InvalidStaticFactoryServices).GetMethod(nameof(InvalidStaticFactoryServices.GetService))!;
        var serviceAttribute = factoryMethod.GetCustomAttribute<ServiceAttribute>()!;
        
        Assert.Throws<InvalidImplementationTypeException>(() => ServiceLocator.GetServiceDescriptor(factoryMethod, serviceAttribute));
    }
    
    [Fact]
    public void GetServiceDescriptor_ThrowsRegistrationNotSupportedException_WhenServiceIsOpenGeneric() {
        var factoryMethod = typeof(InvalidStaticFactoryServices).GetMethod(nameof(InvalidStaticFactoryServices.GetGenericService))!;
        var serviceAttribute = factoryMethod.GetCustomAttribute<ServiceAttribute>()!;
        
        var exception = Assert.Throws<RegistrationNotSupportedException>(() => ServiceLocator.GetServiceDescriptor(factoryMethod, serviceAttribute));
        Assert.Equal("Service factories are not supported for open generic types", exception.Message);
    }
    
    [Fact]
    public void GetServiceDescriptor_ThrowsRegistrationNotSupportedException_WhenFactoryMethodIsGeneric() {
        var factoryMethod = typeof(InvalidStaticFactoryServices).GetMethod(nameof(InvalidStaticFactoryServices.GenericGetService))!;
        var serviceAttribute = factoryMethod.GetCustomAttribute<ServiceAttribute>()!;
        
        var exception = Assert.Throws<RegistrationNotSupportedException>(() => ServiceLocator.GetServiceDescriptor(factoryMethod, serviceAttribute));
        Assert.Equal("Generic factory methods are not supported", exception.Message);
    }

    [Fact]
    public void GetServiceDescriptor_ReturnedServiceDescriptorWorks_WhenFactoryHasNoParameters() {
        var factoryMethod = typeof(StaticFactoryServices).GetMethod(nameof(StaticFactoryServices.GetService))!;
        var serviceAttribute = factoryMethod.GetCustomAttribute<ServiceAttribute>()!;
        
        var serviceDescriptor = ServiceLocator.GetServiceDescriptor(factoryMethod, serviceAttribute);
        
        Assert.NotNull(serviceDescriptor);
        var service = new ServiceCollection().Add(serviceDescriptor).BuildServiceProvider().GetService<IService>();

        Assert.NotNull(service);
        Assert.IsType<IService>(service, false);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("key")]
    [InlineData(123)]
    public void GetServiceDescriptor_ReturnedServiceDescriptorWorks_WhenFactoryHasKeyInParameters(object? key) {
        var factoryMethod = typeof(StaticFactoryServices).GetMethod(nameof(StaticFactoryServices.GetServiceWithKey))!;
        var serviceAttribute = factoryMethod.GetCustomAttribute<ServiceAttribute>()!;
        serviceAttribute.ServiceKey = key;
        
        var serviceDescriptor = ServiceLocator.GetServiceDescriptor(factoryMethod, serviceAttribute);
        
        Assert.NotNull(serviceDescriptor);
        var service = new ServiceCollection().Add(serviceDescriptor).BuildServiceProvider().GetKeyedService<IService>(key);
        
        Assert.NotNull(service);
        Assert.IsType<IService>(service, false);
        Assert.Equal(key, service.Key);
    }
    
    [Fact]
    public void GetServiceDescriptor_ReturnedServiceDescriptorWorks_WhenFactoryHasAnotherServiceInParameters() {
        var factoryMethod = typeof(StaticFactoryServices).GetMethod(nameof(StaticFactoryServices.GetServiceWithDependencies))!;
        var serviceAttribute = factoryMethod.GetCustomAttribute<ServiceAttribute>()!;
        
        var serviceDescriptor = ServiceLocator.GetServiceDescriptor(factoryMethod, serviceAttribute);
        
        Assert.NotNull(serviceDescriptor);
        var service = new ServiceCollection()
           .Add(serviceDescriptor)
           .AddTransient<IAnotherService, AnotherService>()
           .BuildServiceProvider()
           .GetService<IService>();
        
        Assert.NotNull(service);
        Assert.IsType<IService>(service, false);
    }
    
    [Fact]
    public void GetServiceDescriptor_ReturnedServiceDescriptorErrors_WhenFactoryHasMissingServiceInParameters() {
        var factoryMethod = typeof(StaticFactoryServices).GetMethod(nameof(StaticFactoryServices.GetServiceWithDependencies))!;
        var serviceAttribute = factoryMethod.GetCustomAttribute<ServiceAttribute>()!;
        
        var serviceDescriptor = ServiceLocator.GetServiceDescriptor(factoryMethod, serviceAttribute);
        
        Assert.NotNull(serviceDescriptor);
        var serviceProvider = new ServiceCollection()
           .Add(serviceDescriptor)
           .BuildServiceProvider();
        
        Assert.Throws<InvalidOperationException>(() => serviceProvider.GetRequiredService<IService>());
    }

    [Fact]
    public void GetServiceDescriptor_ReturnedServiceDescriptorWorks_WhenOptionalParameterIsMissingFromProvider() {
        var factoryMethod = typeof(StaticFactoryServices).GetMethod(nameof(StaticFactoryServices.GetServiceWithOptionalParameter))!;
        var serviceAttribute = factoryMethod.GetCustomAttribute<ServiceAttribute>()!;
        
        var serviceDescriptor = ServiceLocator.GetServiceDescriptor(factoryMethod, serviceAttribute);
        
        Assert.NotNull(serviceDescriptor);
        var service = new ServiceCollection()
           .Add(serviceDescriptor)
           .BuildServiceProvider()
           .GetService<IService>();
        
        Assert.NotNull(service);
        Assert.IsType<IService>(service, false);
        Assert.Null(service.Key);
    }

    [Fact]
    public void GetServiceDescriptor_ReturnedServiceDescriptorWorks_WhenOptionalParameterIsIncludedFromProvider() {
        var factoryMethod = typeof(StaticFactoryServices).GetMethod(nameof(StaticFactoryServices.GetServiceWithOptionalParameter))!;
        var serviceAttribute = factoryMethod.GetCustomAttribute<ServiceAttribute>()!;
        
        var serviceDescriptor = ServiceLocator.GetServiceDescriptor(factoryMethod, serviceAttribute);
        
        Assert.NotNull(serviceDescriptor);
        var service = new ServiceCollection()
           .Add(serviceDescriptor)
           .AddTransient<IAnotherService, AnotherService>()
           .BuildServiceProvider()
           .GetService<IService>();
        
        Assert.NotNull(service);
        Assert.IsType<IService>(service, false);
        Assert.IsType<IAnotherService>(service.Key, false);
    }
}
