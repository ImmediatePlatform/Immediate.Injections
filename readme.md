# Immediate.Injections

[![NuGet](https://img.shields.io/nuget/v/Immediate.Injections.svg?style=plastic)](https://www.nuget.org/packages/Immediate.Injections/)
[![GitHub release](https://img.shields.io/github/release/ImmediatePlatform/Immediate.Injections.svg)](https://GitHub.com/ImmediatePlatform/Immediate.Injections/releases/)
[![GitHub license](https://img.shields.io/github/license/ImmediatePlatform/Immediate.Injections.svg)](https://github.com/ImmediatePlatform/Immediate.Injections/blob/main/license.txt) 
[![GitHub issues](https://img.shields.io/github/issues/ImmediatePlatform/Immediate.Injections.svg)](https://GitHub.com/ImmediatePlatform/Immediate.Injections/issues/) 
[![GitHub issues-closed](https://img.shields.io/github/issues-closed/ImmediatePlatform/Immediate.Injections.svg)](https://GitHub.com/ImmediatePlatform/Immediate.Injections/issues?q=is%3Aissue+is%3Aclosed) 
[![GitHub Actions](https://github.com/ImmediatePlatform/Immediate.Injections/actions/workflows/build.yml/badge.svg)](https://github.com/ImmediatePlatform/Immediate.Injections/actions)
[![Coverage Status](https://coveralls.io/repos/github/ImmediatePlatform/Immediate.Injections/badge.svg)](https://coveralls.io/github/ImmediatePlatform/Immediate.Injections)
---

Immediate.Injections is a support package for `Microsoft.Extensions.DependencyInjection` which allows declaring
registrations through attributes. This package supports multiple registration strategies, duplicate-handling strategies,
tagging, and much more.

## Installation

```
dotnet add package Immediate.Injections
```

## Basic Usage

### Declaring a registration

Apply one of the three lifetime attributes to a class or record:

```cs
[RegisterSingleton]
public class MyService;

[RegisterScoped]
public class MyScopedService;

[RegisterTransient]
public class MyTransientService<T>;
```

By default (when no `RegistrationStrategy` is set at the attribute or assembly level), a class is registered as itself.
Generic classes will be registered using open-generics.

### Adding declared registrations to the `IServiceCollection` collection

In your `Program.cs`, add a call to `services.AddXxxServices()`, where Xxx is the application identifier. By default,
this is the short form of the assembly name. For example:

* For a project named `Web`, it will be `services.AddWebHandlers()`
* For a project named `Application.Web`, it will be `services.AddApplicationWebHandlers()`

However, this name can be overridden using `[assembly: ImmediateAssemblyIdentifierAttribute("SomeIdentifier")]`.

### Tags

Tags can be passed to selectively register only tagged services:

```cs
[RegisterSingleton(Tags = ["tag-a"])]
public class MyService;

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

Alternatively, a concrete registration may be declared using generic attributes.

```cs
[RegisterSingleton<IMyService>]
public class MyService : IMyService { }

[RegisterScoped<IService<string>>]
public class MyService<T> : IMyService<string>
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

Name of a static factory method on the class to use as `ImplementationFactory`. The method must be `static`, return the
class type, and accept `(IServiceProvider)` for non-keyed or `(IServiceProvider, object)` for keyed registrations.
Cannot be combined with `UseProxyFactory` or used on open generic types. Factories cannot be used with generic target
classes.

```csharp
[RegisterSingleton(Factory = nameof(Create))]
public class MyService
{
    public static MyService Create(IServiceProvider sp) => new MyService();
}
```

### `UseProxyFactory`

When `true`, the registration uses `ServiceProviderServiceExtensions.GetRequiredService<T>` (or the keyed equivalent) as
the factory. This produces a proxy registration — it does not register the implementation itself, but resolves it from
the container.

`UseProxyFactory = true` be combined with the following:
* A provided `Factory`,
* `RegistrationStrategy = Self`, or
* Generic target classes

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

`ServiceKey` and `DuplicationStrategy` can be configured globally with `[RegistrationDefaults]` attribute applied to the
assembly.

```cs
[assembly: RegistrationDefaults(
	RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces,
	DuplicateStrategy = DuplicateStrategy.Replace,
	UseProxyFactory = true
)]
```

Per-attribute values take precedence over assembly defaults.

***

## `[RegisterServices]`

Apply this attribute to a `static void` method to have it called as part of `AddXxxServices`. The method must accept
`IServiceCollection` as its first parameter, and optionally `ReadOnlySpan<string>` as its second parameter to receive
the tags passed to `AddXxxServices`.

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

> [!NOTE]
> Immediate.Injections uses a different default registration strategy than Injectio. For a clean migration, set
> `[assembly: RegistrationDefaults(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces, UseProxyFactory = true)]`

* The `Tags` parameter receives a `string[]` instead of a comma-separated `string`. Example: `Tags = "foo,bar"` becomes `Tags = ["foo", "bar"]`
* The `RegistrationStrategy.SelfWithInterfaces` enum value changes to `RegistrationStrategy.SelfAndImplementedInterfaces`
* The `Registration` parameter changes to `RegistrationStrategy`
* The `Duplicate` parameter becomes `DuplicateStrategy`
* Assembly name override moves from MSBuild property to `[ImmediateAssemblyIdentifier]` attribute

### From AutoRegisterInject

> [!NOTE]
> Immediate.Injections uses a different default registration strategy than Injectio. For a clean migration, set
> `[assembly: RegistrationDefaults(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)]`.

* The `[TryRegisterXxx]` attributes are removed; but the behavior is implemented using a parameter. Example: `DuplicateStrategy = DuplicateStrategy.Skip`
