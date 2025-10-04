[![NuGet Version](https://img.shields.io/nuget/v/Benjft.Util.DependencyInjection.svg)](https://www.nuget.org/packages/Benjft.Util.DependencyInjection/)
[![License](https://img.shields.io/github/license/benjft/Benjft.Util.svg)](LICENSE)
[![CI](https://github.com/Benjft/Benjft.Util.DependencyInjection/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/Benjft/Benjft.Util.DependencyInjection/actions/workflows/build-and-test.yml)
[![Coverage Status](https://coveralls.io/repos/github/benjft/Benjft.Util.DependencyInjection/badge.svg)](https://coveralls.io/github/benjft/Benjft.Util.DependencyInjection)

# Benjft.Util.DependencyInjection

Attribute-based registration for Microsoft.Extensions.DependencyInjection with support for:

- Class annotations (self-registrations and interface/base mappings)
- Static factory methods
- Transient/Scoped/Singleton lifetimes
- Keyed services (ServiceKey)
- Deterministic registration ordering (Order)
- Assembly scanning across your load context or specific assemblies

Target framework: net9.0

## Installation

Add a reference to the library in your application:
- Project reference: reference the Benjft.Util.DependencyInjection project.
- NuGet: `dotnet add package Benjft.Util.DependencyInjection` (if/when published).

## Usage

### 1) Annotate your types

Self-registration (the type registers as its own service type):

```csharp
using Benjft.Util.DependencyInjection.Attributes;
using Microsoft.Extensions.DependencyInjection;

[SingletonService]
public class CacheProvider { }

// Equivalent with explicit lifetime
[Service(ServiceLifetime.Scoped)]
public class RequestTracker { }
```

Map to an interface or base type:

```csharp
public interface IFoo { }

// Non-generic form
[ImplementsService(typeof(IFoo), ServiceLifetime.Transient)]
public class Foo : IFoo { }

// Generic helper attributes
[ImplementsScopedService<IFoo>]
public class FooScoped : IFoo { }

[ImplementsSingletonService<IFoo>]
public class FooSingleton : IFoo { }
```

Keyed registrations (DI supports multiple registrations differentiated by a key):

```csharp
[SingletonService(ServiceKey = "k1")]
public class SpecialCache : CacheProvider { }
```

Ordering (lower Order values are registered first):

```csharp
[ImplementsService(typeof(IFoo), ServiceKey = "first", Order = 0)]
public class FooFirst : IFoo { }

[ImplementsService(typeof(IFoo), ServiceKey = "second", Order = 10)]
public class FooSecond : IFoo { }
```

### 2) Use static factory methods (optional)

When you want DI to call a static factory:

- For unkeyed registrations, the method must be assignable to `Func<IServiceProvider, object>`
- For keyed registrations, the method must be assignable to `Func<IServiceProvider, object?, object>`

Use the ServiceFactoryAttribute family to declare factories and configure lifetime, service type, key, and order.

```csharp
public interface IBar { }
public class Bar : IBar { }

public static class BarFactory
{
    // Registers IBar as Transient using a factory (non-keyed)
    [TransientServiceFactory(typeof(IBar))]
    public static object CreateBar(IServiceProvider sp) => new Bar();

    // Registers IBar as Scoped using a keyed factory
    [ScopedServiceFactory(typeof(IBar), ServiceKey = "k2")]
    public static object CreateKeyedBar(IServiceProvider sp, object? key) => new Bar();
}
```

Alternatively, you can place a factory method on the implementation type and reference it from a ServiceAttribute using `FactoryMethod`:

```csharp
public class Baz
{
    public static Baz Make(IServiceProvider sp) => new Baz();
}

[Service(FactoryMethod = nameof(Baz.Make), Lifetime = ServiceLifetime.Scoped)]
public class Baz { }
```

If you also want to register against an interface/base type from a type-level attribute with a factory method, use ImplementsServiceAttribute:

```csharp
public interface IBaz { }

public class Baz2 : IBaz
{
    public static IBaz Build(IServiceProvider sp) => new Baz2();
}

[ImplementsService(typeof(IBaz), Lifetime = ServiceLifetime.Singleton, FactoryMethod = nameof(Baz2.Build))]
public class Baz2 : IBaz { /* ... */ }
```

### 3) Scan and register

Choose one of the extension methods to register based on your scenario:

```csharp
using Benjft.Util.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// A) Scan all assemblies visible to the current AssemblyLoadContext (default lifetime = Transient)
services.AddServicesFromAttributes();

// B) Scan a specific assembly
services.AddServicesFromAttributes(typeof(SomeTypeFromTargetAssembly).Assembly);

// C) Scan a set of assemblies
services.AddServicesFromAttributes(new[]
{
    typeof(Foo).Assembly,
    typeof(Bar).Assembly
});

// D) Scan everything currently loaded in the AppDomain and their references
services.AddServicesFromAttributesInDomain();
```

You can override the default lifetime used when an attribute does not specify one:

```csharp
services.AddServicesFromAttributes(defaultLifetime: ServiceLifetime.Scoped);
```

### Resolving keyed services

In .NET 8/9 DI, you can resolve keyed services via the built-in APIs such as `GetRequiredKeyedService<T>(key)` (or equivalent methods/extensions available in your version).

Example (if your environment provides the extension):

```csharp
var provider = services.BuildServiceProvider();
var fooK1 = provider.GetRequiredKeyedService<IFoo>("k1");
```

If your DI version doesn’t expose keyed retrieval extensions, you can still ensure keyed registrations were added by inspecting the ServiceCollection descriptors (as demonstrated in tests).

## API reference summary

Attributes for types:
- ServiceAttribute (and TransientServiceAttribute, ScopedServiceAttribute, SingletonServiceAttribute)
  - Lifetime (nullable): overrides the defaultLifetime passed into AddServicesFromAttributes
  - FactoryMethod (string): name of a public static factory method on the declaring type
  - ServiceKey (object?): registers as a keyed service
  - Order (int): lower values register first
- ImplementsServiceAttribute (and generic + lifetime-specific variants)
  - ServiceType: interface/base type to register against
  - Inherits all properties from ServiceAttribute (Lifetime, FactoryMethod, ServiceKey, Order)

Attributes for static factory methods:
- ServiceFactoryAttribute (and Transient/Scoped/Singleton variants; generic convenience types exist)
  - ServiceTypeOverride (Type?): register against a specified service type instead of the method’s return type
  - Lifetime (nullable): overrides the defaultLifetime
  - ServiceKey (object?): keyed registration
  - Order (int)

Extension methods on IServiceCollection:
- AddServicesFromAttributesInDomain(ServiceLifetime defaultLifetime = Transient)
- AddServicesFromAttributes(ServiceLifetime defaultLifetime = Transient, AssemblyLoadContext? alc = null)
- AddServicesFromAttributes(Assembly assembly, ServiceLifetime defaultLifetime = Transient)
- AddServicesFromAttributes(IEnumerable<Assembly> assemblies, ServiceLifetime defaultLifetime = Transient)
- GetServicesFromAttributes for Type/IEnumerable<Type> (if you need the descriptors without adding them)

## Error handling

The scanner throws descriptive exceptions derived from `DependencyInjectionAttributeException` when it encounters invalid annotations:

- InvalidServiceTypeException
  - Thrown when the implementation type is not assignable to the specified service type, or the implementation is abstract.
- InvalidFactoryMethodException (base for factory errors)
  - FactoryMethodNotFoundException — named factory method not found on the type
  - FactoryMethodNotStaticException — the factory method is not static
  - FactoryMethodHasWrongSignatureException — factory method signature does not match required delegate

Required signatures:
- Unkeyed factory: `Func<IServiceProvider, object>`
- Keyed factory: `Func<IServiceProvider, object?, object>`

## Assembly scanning notes

- `AddServicesFromAttributes()` discovers assemblies via the current contextual AssemblyLoadContext (or its default) and then includes their referenced assemblies.
- `AddServicesFromAttributesInDomain()` starts from all assemblies loaded in the current AppDomain and includes their references.
- For precise control and best performance, prefer scanning specific assemblies (single assembly or a curated list).

## Examples from tests

The test suite includes examples for:
- Self-registration via attributes
- Interface-based registration
- Keyed services (asserting descriptor presence)
- Factory methods and their signatures
- Failure cases using the InvalidOnly fixtures

To run tests:

```powershell
# From the repository root
 dotnet test Benjft.Util.DependencyInjection.sln
```

## License

See the LICENSE file at the repository root.
