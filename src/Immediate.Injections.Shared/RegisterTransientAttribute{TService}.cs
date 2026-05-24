namespace Immediate.Injections.Shared;

/// <summary>
///		Attribute to indicate the target class should be registered for dependency injection
///		as a transient implementation for the specified service.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class RegisterTransientAttribute<TService> : Attribute
{
	/// <inheritdoc cref="RegisterSingletonAttribute.ServiceKey" />
	public object? ServiceKey { get; init; }

	/// <inheritdoc cref="RegisterSingletonAttribute.Factory" />
	public string? Factory { get; init; }

	/// <inheritdoc cref="RegisterSingletonAttribute.DuplicateStrategy" />
	public DuplicateStrategy DuplicateStrategy { get; init; }

	/// <inheritdoc cref="RegisterSingletonAttribute.Tags" />
	public string[]? Tags { get; init; }

	/// <inheritdoc cref="RegisterSingletonAttribute{TService}.UseProxy" />
	public bool UseProxy { get; init; }
}
