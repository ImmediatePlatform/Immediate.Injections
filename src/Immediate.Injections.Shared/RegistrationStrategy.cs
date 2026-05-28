namespace Immediate.Injections.Shared;

/// <summary>
///		Service registration generation strategy
/// </summary>
public enum RegistrationStrategy
{
	/// <summary>
	///	    Registers the attributed concrete type as the service provided in <c>ServiceType</c>; or the targeted class
	///	    otherwise.
	/// </summary>
	/// <remarks>
	///		Should not be manually specified.
	/// </remarks>
	None = 0,

	/// <summary>
	///	    Registers the attributed concrete type as itself.
	/// </summary>
	Self = 1,

	/// <summary>
	///		Registers the attributed concrete type as all of its implemented interfaces.
	/// </summary>
	ImplementedInterfaces = 2,

	/// <summary>
	///		Registers the attributed concrete type as all of its implemented interfaces and itself
	/// </summary>
	SelfAndImplementedInterfaces = 3,
}
