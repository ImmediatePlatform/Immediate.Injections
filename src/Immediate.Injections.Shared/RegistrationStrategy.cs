namespace Immediate.Injections.Shared;

/// <summary>
///		Service registration generation strategy
/// </summary>
public enum RegistrationStrategy
{
	/// <summary>
	///		Registers the attributed concrete type as itself
	/// </summary>
	Self = 0,

	/// <summary>
	///		Registers the attributed concrete type as all of its implemented interfaces.
	/// </summary>
	ImplementedInterfaces = 1,

	/// <summary>
	///		Registers the attributed concrete type as all of its implemented interfaces and itself
	/// </summary>
	SelfWithImplementedInterfaces = 2,

	/// <summary>
	///		Registers the attributed concrete type as all of its implemented interfaces and itself.
	///		For the interfaces a proxy-factory resolves the service from its type-name, so only one 
	///		instance is created per lifetime.
	/// </summary>
	/// <remarks>
	///		For open-generic registrations, this behaves like <see cref="SelfWithImplementedInterfaces"/>.
	///	</remarks>
	SelfWithProxyFactory = 3,
}
