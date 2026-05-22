namespace Immediate.Injections.Shared;

/// <summary>
///		Service registration duplicate service strategies
/// </summary>
public enum DuplicateStrategy
{
	/// <summary>
	///		Appends a new registration for existing services.
	/// </summary>
	/// <remarks>
	///		Generated code will use <c>services.Add()</c>
	/// </remarks>
	Append = 0,

	/// <summary>
	///		Skips registrations for services that already exist.
	/// </summary>
	/// <remarks>
	///		Generated code will use <c>services.TryAdd()</c>
	/// </remarks>
	Skip = 1,

	/// <summary>
	///		Replaces the first existing service registration.
	/// </summary>
	/// <remarks>
	///		Generated code will use <c>services.Replace()</c>
	/// </remarks>
	Replace = 2,
}
