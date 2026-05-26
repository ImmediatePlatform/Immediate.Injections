using Microsoft.Extensions.DependencyInjection;

namespace Immediate.Injections.Shared;

/// <summary>
///	    Attribute to indicate the target class should be registered for dependency injection as a singleton
///	    implementation for services as described by <see cref="RegistrationStrategy"/>.
/// </summary>
/// <remarks>
///	    If the target class is generic, then it will be registered as an open generic.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class RegisterSingletonAttribute : Attribute
{
	/// <summary>
	///		The <see cref="ServiceDescriptor.ServiceType"/> of the generated registration.
	/// </summary>
	public Type? ServiceType { get; init; }

	/// <summary>
	///		The <see cref="ServiceDescriptor.ServiceKey"/> of the generated registration.
	/// </summary>
	public object? ServiceKey { get; init; }

	/// <summary>
	///	    Name of the <see cref="ServiceDescriptor.ImplementationFactory"/> method of the generated registration.
	/// </summary>
	/// <remarks>
	///	    If <see cref="ServiceDescriptor.ServiceKey"/> is provided, then the factory will be registered as <see
	///	    cref="ServiceDescriptor.KeyedImplementationFactory"/>.
	/// </remarks>
	public string? Factory { get; init; }

	/// <summary>
	///	    The <see cref="Shared.DuplicateStrategy"/> to configure how the service registration will be added to the
	///	    collection.
	/// </summary>
	/// <remarks>
	///		<list type="bullet">
	///			<item>If provided, this value is used.</item>
	///			<item>If not provided, the assembly default set by <see cref="RegistrationDefaultsAttribute.DuplicateStrategy"/> is used.</item>
	///			<item>Otherwise, default is <see cref="DuplicateStrategy.Append"/>.</item>
	///		</list>
	/// </remarks>
	public DuplicateStrategy DuplicateStrategy { get; init; }

	/// <summary>
	///	    The <see cref="Shared.RegistrationStrategy"/> to configure which services will be registered for the target
	///	    class.
	/// </summary>
	/// <remarks>
	///		<list type="bullet">
	///			<item>If provided, this value is used.</item>
	///			<item>If not provided, the assembly default set by <see cref="RegistrationDefaultsAttribute.RegistrationStrategy"/> is used.</item>
	///			<item>Otherwise, default is <see cref="RegistrationStrategy.Self"/>.</item>
	///		</list>
	/// </remarks>
	public RegistrationStrategy RegistrationStrategy { get; init; }

	/// <summary>
	///	    When <see langword="true" />, generate proxy method for the registered instance of the target class.
	/// </summary>
	/// <remarks>
	/// <para>
	///	    This proxy registration does not generate a separate registration for the target class. This property is
	///	    meant to be used in conjunction with other applications that actually register the implementation.
	/// </para>
	/// </remarks>
	public bool UseProxyFactory { get; init; }

	/// <summary>
	///		An optional list of tags which can be used to filter the generated registrations at runtime.
	/// </summary>
	public string[]? Tags { get; init; }
}
