namespace Immediate.Injections.Shared;

/// <summary>
///		Attribute to indicate the target class should be registered for dependency injection
///		as a singleton implementation for services as described by <see cref="RegistrationStrategy"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class RegisterScopedAttribute : Attribute
{
	/// <inheritdoc cref="RegisterSingletonAttribute.ServiceKey" />
	public object? ServiceKey { get; init; }

	/// <inheritdoc cref="RegisterSingletonAttribute.Factory" />
	public string? Factory { get; init; }

	/// <inheritdoc cref="RegisterSingletonAttribute.DuplicateStrategy" />
	public DuplicateStrategy DuplicateStrategy { get; init; }

	/// <inheritdoc cref="RegisterSingletonAttribute.RegistrationStrategy" />
	public RegistrationStrategy RegistrationStrategy { get; init; }

	/// <inheritdoc cref="RegisterSingletonAttribute.Tags" />
	public string[]? Tags { get; init; }
}
