namespace Immediate.Injections.Analyzers;

internal static class DiagnosticIds
{
	public const string INJ0001RegisterServicesMethodIsInvalid = "INJ0001";
	public const string INJ0002AttributeIsInvalid = "INJ0002";
	public const string INJ0003ServiceTypeAndRegistrationStrategyIncompatible = "INJ0003";
	public const string INJ0004ServiceTypeIncompatibleWithTarget = "INJ0004";
	public const string INJ0005TargetClassIsNotGeneric = "INJ0005";
	public const string INJ0006TImplementationIsNotSameAsTarget = "INJ0006";
	public const string INJ0007CannotUseProxyFactoryOnSelf = "INJ0007";
	public const string INJ0008CannotUseProxyFactoryForOpenGeneric = "INJ0008";
}
