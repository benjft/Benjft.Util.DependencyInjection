using System.Reflection;
using Benjft.Util.DependencyInjection.Attributes;
using Benjft.Util.DependencyInjection.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Benjft.Util.DependencyInjection.ServiceLocators;

internal class StaticFactoryServiceLocator(ServiceLifetime defaultLifetime) : IServiceLocator {
    public IEnumerable<(int, ServiceDescriptor)> FindServicesInAssembly(Assembly assembly) {
        return from attributeAndMethod in GetDecoratedStaticMethodsInAssembly(assembly)
               let factoryMethod = attributeAndMethod.factoryMethod
               let attribute = attributeAndMethod.attribute
               let serviceDescriptor = GetServiceDescriptor(factoryMethod, attribute)
               select (attribute.RegistrationOrder, serviceDescriptor);
    }

    private static IEnumerable<(MethodInfo factoryMethod, ServiceAttribute attribute)> GetDecoratedStaticMethodsInAssembly(Assembly assembly) {
        return from type in assembly.GetTypes()
               from method in type.GetMethods()
               where method.IsStatic
               from attribute in method.GetCustomAttributes<ServiceAttribute>()
               select (method, attribute);
    }

    internal ServiceDescriptor GetServiceDescriptor(MethodInfo factoryMethod, ServiceAttribute attribute) {
        if (attribute.ServiceType.IsGenericTypeDefinition) {
            throw new RegistrationNotSupportedException("Service factories are not supported for open generic types");
        }
        
        if (!factoryMethod.ReturnType.IsAssignableTo(attribute.ServiceType)) {
            throw new InvalidImplementationTypeException(factoryMethod.ReturnType, attribute.ServiceType);
        }
        
        if (factoryMethod.IsGenericMethodDefinition) {
            throw new RegistrationNotSupportedException("Generic factory methods are not supported");
        }
        
        var factoryDelegate = CreateFactoryDelegate(factoryMethod);
        return new ServiceDescriptor(attribute.ServiceType, attribute.ServiceKey, factoryDelegate, attribute.ServiceLifetime ?? defaultLifetime);
    }

    private static Func<IServiceProvider, object?, object> CreateFactoryDelegate(MethodInfo methodInfo) {
        var parametersInfo = methodInfo.GetParameters();

        return FactoryInvoker;
        object FactoryInvoker(IServiceProvider sp, object? key) {
            var parameters = parametersInfo.Select(p => GetParameterValue(p, key, sp)).ToArray();
            return methodInfo.Invoke(null, parameters) ?? throw new Exception($"Factory method {methodInfo.Name} returned null");
        }
    }
    
    private static object? GetParameterValue(ParameterInfo parameterInfo, object? serviceKey, IServiceProvider serviceProvider) {
        if (parameterInfo.GetCustomAttribute<ServiceKeyAttribute>() != null) {
            return serviceKey;
        }

        var fromKey = parameterInfo.GetCustomAttribute<FromKeyedServicesAttribute>()?.Key;
        var isRequired = !parameterInfo.HasDefaultValue;
        
        if (isRequired) {
            return serviceProvider.GetRequiredKeyedService(parameterInfo.ParameterType, fromKey);
        }

        return serviceProvider.GetKeyedServices(parameterInfo.ParameterType, fromKey).LastOrDefault() ?? parameterInfo.DefaultValue;
    }
}
