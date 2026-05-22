namespace Immediate.Injections.Shared;

/// <summary>
///		Provides assembly-wide registration defaults
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class RegistrationDefaultsAttribute : Attribute
{
	/// <summary>
	///		Sets the default <see cref="Shared.DuplicateStrategy"/> for the assembly.
	/// </summary>
	/// <remarks>
	///		Default value is <see cref="DuplicateStrategy.Append"/>.
	/// </remarks>
	public DuplicateStrategy DuplicateStrategy { get; init; }

	/// <summary>
	///		Sets the default <see cref="Shared.RegistrationStrategy"/> for the assembly.
	/// </summary>
	/// <remarks>
	///		Default value is <see cref="RegistrationStrategy.Self"/>.
	/// </remarks>
	public RegistrationStrategy RegistrationStrategy { get; init; }
}
