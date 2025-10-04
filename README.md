# Benjft.Util.DependencyInjection

A .NET library that simplifies dependency injection registration through attributes. This library allows you to register services with the Microsoft Dependency Injection container using simple attribute declarations on your classes, eliminating the need for manual registration code.

## Installation

```bash
dotnet add package Benjft.Util.DependencyInjection
```

## Features

- Register services using attributes directly on implementation classes
- Support for Transient, Scoped, and Singleton lifetimes
- Define service implementations through attributes
- Register services with a specific factory method
- Support for keyed services
- Order-based registration priority

## Basic Usage

### Registering Services

In your application startup code (e.g., Program.cs or Startup.cs):

```csharp
using Benjft.Util.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services with attribute-based registration
builder.Services.AddServicesFromAttributes();
```

### Marking Classes as Services

You can mark any class as a service using the attributes:

```csharp
// Register as a transient service
[TransientService]
public class MyTransientService
{
    // Implementation
}

// Register as a scoped service
[ScopedService]
public class MyScopedService
{
    // Implementation
}

// Register as a singleton service
[SingletonService]
public class MySingletonService
{
    // Implementation
}

// Using the base attribute with specified lifetime
[Service(ServiceLifetime.Transient)]
public class MyCustomService
{
    // Implementation
}
```

### Implementing Interfaces

Register a class as an implementation of an interface:

```csharp
public interface IMyService
{
    void DoSomething();
}

[ImplementsService(typeof(IMyService))]
public class MyServiceImplementation : IMyService
{
    public void DoSomething()
    {
        // Implementation
    }
}
```

### Using Factory Methods

Specify a factory method to create the service:

```csharp
[Service(FactoryMethod = nameof(Create))]
public class MyFactoryService
{
    private MyFactoryService() { }

    public static MyFactoryService Create(IServiceProvider serviceProvider)
    {
        // Create and configure the service instance
        return new MyFactoryService();
    }
}
```

### Keyed Services

Register services with a key:

```csharp
[Service(ServiceKey = "primary")]
public class PrimaryService : IMyService
{
    // Implementation
}

[Service(ServiceKey = "secondary")]
public class SecondaryService : IMyService
{
    // Implementation
}
```

## Advanced Usage

### Specifying Registration Order

You can control the order in which services are registered:

```csharp
[Service(Order = 1)]
public class HighPriorityService
{
    // Implementation
}

[Service(Order = 10)]
public class LowPriorityService
{
    // Implementation
}
```

### Scanning Specific Assemblies

You can specify which assemblies to scan for service attributes:

```csharp
// Scan a specific assembly
builder.Services.AddServicesFromAttributes(typeof(MyType).Assembly);

// Scan multiple assemblies
builder.Services.AddServicesFromAttributes(new[] { assembly1, assembly2 });
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.
