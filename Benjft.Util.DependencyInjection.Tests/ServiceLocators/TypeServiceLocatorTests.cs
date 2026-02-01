using System.Reflection;
using Benjft.Util.DependencyInjection.Exceptions;
using Benjft.Util.DependencyInjection.ServiceLocators;
using Microsoft.Extensions.DependencyInjection;

namespace Benjft.Util.DependencyInjection.Tests.ServiceLocators;

public class TypeServiceLocatorTests {
    [Fact]
    public void FindServicesInAssembly_ReturnsServiceDescriptors_WhenAllRegistrationsValid() {
        var assembly = Assembly.Load(new AssemblyName("Benjft.Util.DependencyInjection.Example.Valid"));
        var serviceLocator = new TypeServiceLocator(ServiceLifetime.Transient);
        
        var serviceDescriptors = serviceLocator.FindServicesInAssembly(assembly).ToArray();
        
        Assert.NotEmpty(serviceDescriptors);
    }

    [Fact]
    public void FindServicesInAssembly_ThrowsInvalidImplementationTypeException_WhenImplementationTypeIsInvalid() {
        var assembly = Assembly.Load(new AssemblyName("Benjft.Util.DependencyInjection.Example.Invalid"));
        var serviceLocator = new TypeServiceLocator(ServiceLifetime.Transient);
        
        Assert.Throws<InvalidImplementationTypeException>(() => serviceLocator.FindServicesInAssembly(assembly).ToArray());
    }
}
