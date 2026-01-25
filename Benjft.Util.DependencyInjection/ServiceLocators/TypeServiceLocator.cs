using System.Reflection;
using Benjft.Util.DependencyInjection.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Benjft.Util.DependencyInjection.ServiceLocators;

internal class TypeServiceLocator(ServiceLifetime defaultLifetime) : IServiceLocator {
    public IEnumerable<(int, ServiceDescriptor)> FindServicesInAssembly(Assembly assembly) {
        return from typeAndAttribute in GetDecoratedTypesInAssembly(assembly)
               let implementationType = typeAndAttribute.implementationType
               let attribute = typeAndAttribute.attribute
               let serviceDescriptor = GetServiceDescriptor(implementationType, attribute)
               select (attribute.RegistrationOrder, serviceDescriptor);
    }

    private static IEnumerable<(Type implementationType, ServiceAttribute attribute)> GetDecoratedTypesInAssembly(Assembly assembly) {
        return from type in assembly.GetTypes()
               from attribute in type.GetCustomAttributes<ServiceAttribute>()
               select (type, attribute);
    }

    private ServiceDescriptor GetServiceDescriptor(Type implementationType, ServiceAttribute attribute) {
        if (!implementationType.IsAssignableTo(attribute.ServiceType)) {
            throw new Exception($"Implementation type {implementationType.Name} is not assignable to {attribute.ServiceType.Name}");
        }

        if (attribute.ServiceKey != null) {
            return ServiceDescriptor.DescribeKeyed(attribute.ServiceType, attribute.ServiceKey, implementationType, attribute.ServiceLifetime ?? defaultLifetime);
        }
        
        return ServiceDescriptor.Describe(attribute.ServiceType, implementationType, attribute.ServiceLifetime ?? defaultLifetime);
    }
}
