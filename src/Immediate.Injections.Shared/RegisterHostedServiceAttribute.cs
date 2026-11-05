namespace Immediate.Injections.Shared;

/// <summary>
/// Registers the target class as a singleton hosted service.
/// </summary>
/// <remarks>
/// The target must implement <c>Microsoft.Extensions.Hosting.IHostedService</c> and must not be abstract or generic.
/// Like <c>AddHostedService</c>, repeated registrations of the same implementation are ignored.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RegisterHostedServiceAttribute : Attribute
{
	/// <summary>
	/// The optional registration order, applied after services without an explicit order.
	/// </summary>
	/// <remarks>
	/// If an order is not provided, it is assumed that it does not matter whether this service
	/// is initialized before or after any other hosted service. Explicit orders must be unique
	/// within the assembly and are applied in ascending order.
	/// </remarks>
	public int Order { get; init; }

	/// <summary>
	/// An optional list of tags used to filter the generated registration at runtime.
	/// </summary>
	public string[]? Tags { get; init; }
}
