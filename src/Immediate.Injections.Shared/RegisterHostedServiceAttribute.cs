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
	/// An optional list of tags used to filter the generated registration at runtime.
	/// </summary>
	public string[]? Tags { get; init; }
}
