namespace Immediate.Injections.Shared;

/// <summary>
///		Attribute to indicate the target class should be registered for dependency injection
///		as a singleton implementation for the specified service.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class RegisterSingletonAttribute<TService> : Attribute
{
	/// <inheritdoc cref="RegisterSingletonAttribute.ServiceKey" />
	public object? ServiceKey { get; init; }

	/// <inheritdoc cref="RegisterSingletonAttribute.Factory" />
	public string? Factory { get; init; }

	/// <inheritdoc cref="RegisterSingletonAttribute.DuplicateStrategy" />
	public DuplicateStrategy DuplicateStrategy { get; init; }

	/// <inheritdoc cref="RegisterSingletonAttribute.Tags" />
	public string[]? Tags { get; init; }

	/// <summary>
	///	    When <see langword="true" />, generate proxy method for the registered instance of the target class.
	/// </summary>
	/// <remarks>
	///	    This proxy registration does not generate a separate registration for the target class. This property is
	///	    meant to be used in conjunction with other applications that actually register the implementation.
	/// </remarks>
	public bool UseProxyFactory { get; init; }
}
