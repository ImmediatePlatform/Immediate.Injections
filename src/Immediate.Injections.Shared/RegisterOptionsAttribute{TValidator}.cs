namespace Immediate.Injections.Shared;

/// <typeparam name="TValidator">
///		An <see cref="IValidateOptions{TOptions}"/> implementation.
/// </typeparam>
/// <param name="name">
///		The configuration section name.
///		<remarks>
///			Defaults to the class name without the options suffix.
///		</remarks>
/// </param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RegisterOptionsAttribute<TValidator>(string? name = null) : RegisterOptionsAttribute(name) where TValidator : class;
