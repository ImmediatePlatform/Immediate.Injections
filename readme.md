# Immediate.Injections

[![NuGet](https://img.shields.io/nuget/v/Immediate.Injections.svg?style=plastic)](https://www.nuget.org/packages/Immediate.Injections/)
[![GitHub release](https://img.shields.io/github/release/ImmediatePlatform/Immediate.Injections.svg)](https://GitHub.com/ImmediatePlatform/Immediate.Injections/releases/)
[![GitHub license](https://img.shields.io/github/license/ImmediatePlatform/Immediate.Injections.svg)](https://github.com/ImmediatePlatform/Immediate.Injections/blob/main/license.txt) 
[![GitHub issues](https://img.shields.io/github/issues/ImmediatePlatform/Immediate.Injections.svg)](https://GitHub.com/ImmediatePlatform/Immediate.Injections/issues/) 
[![GitHub issues-closed](https://img.shields.io/github/issues-closed/ImmediatePlatform/Immediate.Injections.svg)](https://GitHub.com/ImmediatePlatform/Immediate.Injections/issues?q=is%3Aissue+is%3Aclosed) 
[![GitHub Actions](https://github.com/ImmediatePlatform/Immediate.Injections/actions/workflows/build.yml/badge.svg)](https://github.com/ImmediatePlatform/Immediate.Injections/actions)
---

## Installation

```
dotnet add package Immediate.Injections
```

## Basic Usage

### Registering a service

Apply one of the three lifetime attributes to a class or record:

```cs
[RegisterSingleton]
public class MyService { }

[RegisterScoped]
public class MyScopedService { }

[RegisterTransient]
public class MyTransientService { }
```

By default (when no `RegistrationStrategy` is set at the attribute or assembly level), a class is registered as itself.

### Calling the generated extension method

The generator produces a `RegistrationServiceCollectionExtensions` class with an extension method named `Add{AssemblyName}Services`:

```cs
builder.Services.AddMyProjectServices();
```

The assembly name is derived from the assembly's `AssemblyName` property, with dots and spaces stripped. It can be overridden with `ImmediateAssemblyIdentifierAttribute`.

Tags can be passed to selectively register only tagged services:

```cs
builder.Services.AddMyProjectServices("tag-a", "tag-b");
```

***

## Attribute Reference

All three lifetime attributes — `RegisterSingleton`, `RegisterScoped`, `RegisterTransient` — share the same properties.

### `ServiceType`

Registers the class as the specified service type. The class must be assignable to it.

```cs
[RegisterSingleton(ServiceType = typeof(IMyService))]
public class MyService : IMyService { }
```

Alternatively, on .NET versions that support generic attributes:

```cs
[RegisterSingleton<IMyService>]
public class MyService : IMyService { }
```

### `RegistrationStrategy`

Controls which service types are generated. Mutually exclusive with `ServiceType`.

| Value | Effect |
|---|---|
| `None` (default) | Registers as `ServiceType` if provided, otherwise as the class itself |
| `Self` | Registers as the concrete class |
| `ImplementedInterfaces` | Registers as each interface the class implements |
| `SelfAndImplementedInterfaces` | Registers as the concrete class and each interface |

```csharp
[RegisterSingleton(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)]
public class MyService : IMyService { }
```

### `DuplicateStrategy`

Controls what happens when a registration for the same service type already exists.

| Value | Generated call |
|---|---|
| `Append` (default) | `services.Add(...)` |
| `Skip` | `services.TryAdd(...)` |
| `Replace` | `services.Replace(...)` |

### `ServiceKey`

Registers the service as a keyed service.

```csharp
[RegisterSingleton(ServiceKey = "my-key")]
public class MyService { }
```

### `Factory`

Name of a static factory method on the class to use as `ImplementationFactory`. The method must be `static`, return the class type, and accept `(IServiceProvider)` for non-keyed or `(IServiceProvider, object)` for keyed registrations. Cannot be combined with `UseProxyFactory` or used on open generic types.

```csharp
[RegisterSingleton(Factory = nameof(Create))]
public class MyService
{
    public static MyService Create(IServiceProvider sp) => new MyService();
}
```

### `UseProxyFactory`

When `true`, the registration uses `ServiceProviderServiceExtensions.GetRequiredService<T>` (or the keyed equivalent) as the factory. This produces a proxy registration — it does not register the implementation itself, but resolves it from the container.

Cannot be combined with `Factory`. Cannot be used with `RegistrationStrategy = Self` or on open generics.

```csharp
[RegisterSingleton(ServiceType = typeof(IMyService), UseProxyFactory = true)]
public class MyService : IMyService { }
```

### `Tags`

Assigns string tags to the registration. When `AddXxxServices` is called with tag arguments, only registrations that share at least one tag (or registrations with no tags) are included.

```csharp
[RegisterSingleton(Tags = ["worker", "background"])]
public class BackgroundWorker { }
```

***

## Global Configuration

`ServiceKey` and `DuplicationStrategy` can be configured globally with `[RegistrationDefaults]` attribute applied to the assembly.

```cs
[assembly: RegistrationDefaults(
	RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces,
	DuplicateStrategy = DuplicateStrategy.Replace
)]
```

Per-attribute values take precedence over assembly defaults.

***

## `[RegisterServices]`

Apply this attribute to a `static void` method to have it called as part of `AddXxxServices`. The method must accept `IServiceCollection` as its first parameter, and optionally `ReadOnlySpan<string>` as its second parameter to receive the tags passed to `AddXxxServices`.

```csharp
public static class ManualRegistrations
{
    [RegisterServices]
    public static void Register(IServiceCollection services) { ... }

    // or, to receive tags:
    [RegisterServices]
    public static void RegisterWithTags(IServiceCollection services, ReadOnlySpan<string> tags) { ... }
}
```

***

## Migration

### From Injectio

* `Tags = "foo,bar"` becomes `Tags = ["foo", "bar"]`
* `RegistrationStrategy.SelfWithInterfaces` becomes `RegistrationStrategy.SelfAndImplementedInterfaces`
* `Duplicate = DuplicateStrategy.Replace` becomes `DuplicateStrategy = DuplicateStrategy.Replace`
* Assembly name override moves from MSBuild property to `[ImmediateAssemblyIdentifier]` attribute

### From AutoRegisterInject

* Add `[assembly: RegistrationDefaults(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)]` to get the default ARI behaviour
* `[TryRegister*]` attributes become `DuplicateStrategy = DuplicateStrategy.Skip`
