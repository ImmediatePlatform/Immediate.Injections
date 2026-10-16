using System.Diagnostics.CodeAnalysis;

namespace Immediate.Injections.Shared;

/// <param name="name">
///		The configuration section name.
///		<remarks>
///			Defaults to the class name without the options suffix.
///		</remarks>
/// </param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
[SuppressMessage("Performance", "CA1813:Avoid unsealed attributes", Justification = "RegisterOptionsAttribute<TValidator>")]
public class RegisterOptionsAttribute(string? name = null) : Attribute
{
	/// <summary>
	///		The configuration section name.
	/// </summary>
	public string? Name { get; } = name;

	/// <summary>
	///		Whether non-public properties are bound.
	/// </summary>
	public bool BindNonPublicProperties { get; init; }

	/// <summary>
	///		Whether data annotations are validated.
	/// </summary>
	public bool ValidateDataAnnotations { get; init; }

	/// <summary>
	///		Whether validation runs on host start.
	/// </summary>
	public bool ValidateOnStart { get; init; }
}
