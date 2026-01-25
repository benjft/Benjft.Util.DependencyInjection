using System.Reflection;
using Benjft.Util.DependencyInjection.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Benjft.Util.DependencyInjection.ServiceLocators;

internal interface IServiceLocator {
    /// <summary>
    /// Scans the assembly for service registrations marked with the <see cref="ServiceAttribute"/> attribute.
    /// </summary>
    /// <param name="assembly">
    /// The assembly to find services in.
    /// </param>
    /// <returns>
    /// An enumerable of service descriptors with their registration order.
    /// </returns>
    IEnumerable<(int registrationOrder, ServiceDescriptor serviceDescriptor)> FindServicesInAssembly(Assembly assembly);
}
