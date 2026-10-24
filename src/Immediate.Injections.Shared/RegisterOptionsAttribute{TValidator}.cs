using Microsoft.Extensions.Options;

namespace Immediate.Injections.Shared;

/// <typeparam name="TValidator">
///		An <see cref="IValidateOptions{TOptions}"/> implementation.
/// </typeparam>
/// <param name="name">
///		The configuration section name. Defaults to the class name without the options suffix.
/// </param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RegisterOptionsAttribute<TValidator>(string? name = null) : Attribute where TValidator : class
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
