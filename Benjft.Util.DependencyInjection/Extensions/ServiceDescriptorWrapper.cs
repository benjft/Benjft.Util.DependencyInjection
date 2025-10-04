using System.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace Benjft.Util.DependencyInjection;

    /// <summary>
    /// Wraps a ServiceDescriptor with ordering information for deterministic service registration.
    /// </summary>
    /// <param name="serviceDescriptor">The service descriptor to wrap.</param>
    /// <param name="order">The registration order for this service.</param>
internal class ServiceDescriptorWrapper(ServiceDescriptor serviceDescriptor, int order)
    : IComparable<ServiceDescriptorWrapper>, IComparable {
    
    /// <summary>
    /// Deconstructs the wrapper into its component parts.
    /// </summary>
    /// <param name="serviceDescriptor">The service descriptor.</param>
    /// <param name="order">The registration order.</param>
    public void Deconstruct(out ServiceDescriptor serviceDescriptor, out int order) {
        serviceDescriptor = ServiceDescriptor;
        order = Order;
    }

    /// <summary>
    /// Gets the wrapped service descriptor.
    /// </summary>
    public ServiceDescriptor ServiceDescriptor { get; init; } = serviceDescriptor;

    /// <summary>
    /// Gets the registration order for this service.
    /// Services with lower order values are registered first.
    /// </summary>
    public int Order { get; init; } = order;

            /// <summary>
            /// Compares this wrapper to another wrapper for ordering purposes.
            /// </summary>
            /// <param name="other">The wrapper to compare with.</param>
            /// <returns>A value indicating the relative order of the wrappers.</returns>
    public int CompareTo(ServiceDescriptorWrapper? other) {
        if (ReferenceEquals(this, other)) {
            return 0;
        }
        if (other is null) {
            return 1;
        }

        var result = Order.CompareTo(other.Order);
        if (result != 0) {
            return result;
        }

        result = string.Compare(
            ServiceDescriptor.ServiceType.FullName,
            other.ServiceDescriptor.ServiceType.FullName,
            StringComparison.Ordinal);
        if (result != 0) {
            return result;
        }

        var isKeyed = ServiceDescriptor.IsKeyedService;
        var otherIsKeyed = other.ServiceDescriptor.IsKeyedService;
        return (isKeyed, otherIsKeyed) switch {
            (false, true) => -1,
            (true, false) => 1,
            (true, true)  => CompareServiceKeys(other),
            _             => 0,
        };
    }

    private int CompareServiceKeys(ServiceDescriptorWrapper other) {
        try {
            return Comparer.Default.Compare(ServiceDescriptor.ServiceKey, other.ServiceDescriptor.ServiceKey);
        } catch (ArgumentException) { }
        return 0;
    }

    public int CompareTo(object? obj) {
        if (obj is ServiceDescriptorWrapper other) {
            return CompareTo(other);
        }
        throw new ArgumentException("Object is not a ServiceDescriptorWrapper", nameof(obj));
    }
}
