using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Benjft.Util.DependencyInjection.Attributes;
using Benjft.Util.DependencyInjection.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Benjft.Util.DependencyInjection.Extensions;

    /// <summary>
    /// Extension methods for adding services to the <see cref="IServiceCollection"/> from attributes.
    /// </summary>
    public static class ServiceCollectionExtensions {
    private static Assembly ThisAssembly => typeof(ServiceCollectionExtensions).Assembly;

    /// <summary>
    /// Adds services to the service collection from attributes in all assemblies that reference this assembly.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="defaultLifetime">The default lifetime to use for services that don't specify one.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddServicesFromAttributes(this IServiceCollection services, ServiceLifetime defaultLifetime = ServiceLifetime.Transient) {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetReferencedAssemblies().Contains(ThisAssembly.GetName()));
        return AddServicesFromAttributes(services, assemblies, defaultLifetime);
    }

            /// <summary>
            /// Adds services to the service collection from attributes in the specified assembly.
            /// </summary>
            /// <param name="services">The service collection to add services to.</param>
            /// <param name="assembly">The assembly to scan for attributes.</param>
            /// <param name="defaultLifetime">The default lifetime to use for services that don't specify one.</param>
            /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddServicesFromAttributes(
        this IServiceCollection services,
        Assembly assembly,
        ServiceLifetime defaultLifetime = ServiceLifetime.Transient) {
        return AddServicesFromAttributes(services, [assembly], defaultLifetime);
    }

            /// <summary>
            /// Adds services to the service collection from attributes in the specified assemblies.
            /// </summary>
            /// <param name="services">The service collection to add services to.</param>
            /// <param name="assemblies">The assemblies to scan for attributes.</param>
            /// <param name="defaultLifetime">The default lifetime to use for services that don't specify one.</param>
            /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddServicesFromAttributes(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime defaultLifetime = ServiceLifetime.Transient) {

        var serviceDescriptors = assemblies.SelectMany(a => a.GetTypes())
                                           .GetServicesFromAttributes(defaultLifetime);

        foreach (var serviceDescriptor in serviceDescriptors) {
            services.Add(serviceDescriptor);
        }
        
        return services;
    }

            /// <summary>
            /// Gets service descriptors from attributes on the specified types.
            /// </summary>
            /// <param name="types">The types to check for service attributes.</param>
            /// <param name="defaultLifetime">The default lifetime to use for services that don't specify one.</param>
            /// <returns>A collection of service descriptors created from the attributes.</returns>
    public static IEnumerable<ServiceDescriptor> GetServicesFromAttributes(
        this IEnumerable<Type> types,
        ServiceLifetime defaultLifetime = ServiceLifetime.Transient) {
        return from type in types
               from serviceDescriptorWrapper in type.GetServiceDescriptors(defaultLifetime)
               orderby serviceDescriptorWrapper
               select serviceDescriptorWrapper.ServiceDescriptor;
    }

            /// <summary>
            /// Gets service descriptors from attributes on the specified type.
            /// </summary>
            /// <param name="type">The type to check for service attributes.</param>
            /// <param name="defaultLifetime">The default lifetime to use for services that don't specify one.</param>
            /// <returns>A collection of service descriptors created from the attributes.</returns>
    public static IEnumerable<ServiceDescriptor> GetServicesFromAttributes(
        this Type type,
        ServiceLifetime defaultLifetime = ServiceLifetime.Transient) {
        return from serviceDescriptorWrapper in type.GetServiceDescriptors(defaultLifetime)
               orderby serviceDescriptorWrapper
               select serviceDescriptorWrapper.ServiceDescriptor;
    }
    
    private static IEnumerable<ServiceDescriptorWrapper> GetServiceDescriptors(this Type type, ServiceLifetime defaultLifetime) {
        foreach (var serviceDescriptor in type.GetFactoryServiceDescriptors(defaultLifetime)) 
            yield return serviceDescriptor;
        foreach (var serviceDescriptor in type.GetTypeServiceDescriptors(defaultLifetime)) 
            yield return serviceDescriptor;
    }

    private static IEnumerable<ServiceDescriptorWrapper> GetFactoryServiceDescriptors(this Type type, ServiceLifetime defaultLifetime) => 
        from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public)
        from attribute in method.GetCustomAttributes<ServiceFactoryAttribute>()
        select new ServiceDescriptorWrapper(
            GetFactoryServiceDescriptor(method.ReturnType, method, attribute.ServiceTypeOverride, attribute.ServiceKey, attribute.Lifetime ?? defaultLifetime), 
            attribute.Order);

    private static ServiceDescriptor GetFactoryServiceDescriptor(
        this Type type,
        MethodInfo method,
        Type? serviceTypeOverride,
        object? serviceKey,
        ServiceLifetime lifetime) {

        try {
            ValidateFactoryMethodIsStatic(method);
            
            var serviceType = serviceTypeOverride ?? method.ReturnType;
            ValidateServiceType(type, serviceType);

            return CreateFactoryServiceDescriptor(method, serviceKey, lifetime, serviceType);
        } catch (InvalidFactoryMethodException e) {
            throw new InvalidFactoryMethodException($"Factory Method {type.Name}::{method.Name} is not a valid factory method.", e);
        }
    }

    private static ServiceDescriptor CreateFactoryServiceDescriptor(
        MethodInfo method,
        object? serviceKey,
        ServiceLifetime lifetime,
        Type serviceType) {
        if (serviceKey != null) {
            try {
                var invoker = method.CreateDelegate<Func<IServiceProvider, object?, object>>();
                return ServiceDescriptor.DescribeKeyed(serviceType, serviceKey, invoker, lifetime);
            } catch (ArgumentException e) {
                throw new FactoryMethodHasWrongSignatureException("Keyed Factory Method must be assignable to Func<IServiceProvider, object?, object>.", e);
            }
        }

        try {
            var invoker = method.CreateDelegate<Func<IServiceProvider, object>>();
            return ServiceDescriptor.Describe(serviceType, invoker, lifetime);
        } catch (ArgumentException e) {
            throw new FactoryMethodHasWrongSignatureException("Factory Method must be assignable to Func<IServiceProvider, object>.", e);
        }
    }

    private static IEnumerable<ServiceDescriptorWrapper> GetTypeServiceDescriptors(this Type type, ServiceLifetime defaultLifetime) =>
        from attribute in type.GetCustomAttributes<ServiceAttribute>() 
        select new ServiceDescriptorWrapper(type.GetTypeServiceDescriptor(attribute, defaultLifetime), attribute.Order);

    private static ServiceDescriptor GetTypeServiceDescriptor(
        this Type type,
        ServiceAttribute attribute,
        ServiceLifetime defaultLifetime) {
        var lifetime = attribute.Lifetime ?? defaultLifetime;

        if (attribute.FactoryMethod != null) {
            var methodInfo = type.GetMethod(attribute.FactoryMethod);
            ValidateFactoryMethodExists(type, attribute, methodInfo);
            
            return type.GetFactoryServiceDescriptor(methodInfo, (attribute as ImplementsServiceAttribute)?.ServiceType, attribute.ServiceKey, lifetime);
        }
        ValidateServiceNotAbstract(type);
        
        var serviceType = (attribute as ImplementsServiceAttribute)?.ServiceType ?? type;
        ValidateServiceType(type, serviceType);

        return ServiceDescriptor.DescribeKeyed(serviceType, attribute.ServiceKey, type, lifetime);
    }

    private static void ValidateFactoryMethodExists(Type type, ServiceAttribute attribute, [NotNull]MethodInfo? methodInfo) {
        if (methodInfo == null) {
            throw new FactoryMethodNotFoundException($"Type {type.Name} does not contain a public static method named {attribute.FactoryMethod}.");
        }
    }

    private static void ValidateServiceType(Type type, Type serviceType) {
        if (!type.IsAssignableTo(serviceType)) {
            throw new InvalidServiceTypeException($"Type {type.Name} must be assignable to Service Type {serviceType.Name}.");
        }
    }

    private static void ValidateServiceNotAbstract(Type type) {
        if (type.IsAbstract) {
            throw new InvalidServiceTypeException($"Implementation type {type.Name} must not be an abstract class.");
        }
    }

    private static void ValidateFactoryMethodIsStatic(MethodInfo method) {
        if (!method.IsStatic) {
            throw new FactoryMethodNotStaticException("Factory Method must be static");
        }
    }
}
