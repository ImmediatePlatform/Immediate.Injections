namespace Immediate.Injections.Shared;

/// <summary>
///		Attribute to indicate the method should be called to register services
///	</summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class RegisterServicesAttribute : Attribute;
