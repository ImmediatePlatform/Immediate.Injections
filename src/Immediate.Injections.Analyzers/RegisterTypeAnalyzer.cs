using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Injections.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RegisterTypeAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor AttributeIsInvalid =
		new(
			id: DiagnosticIds.INJ0002AttributeIsInvalid,
			title: "Attribute application is invalid",
			messageFormat: "Type '{0}' has attribute `[{1}]` and will not be registered",
			category: "ImmediateInjections",
			defaultSeverity: DiagnosticSeverity.Hidden,
			isEnabledByDefault: true,
			description: "Invalid applications of the `RegisterXxx` attribute will not be transformed into DI registrations.",
			customTags: [WellKnownDiagnosticTags.NotConfigurable, WellKnownDiagnosticTags.Unnecessary]
		);

	public static readonly DiagnosticDescriptor ServiceTypeAndRegistrationStrategyIncompatible =
		new(
			id: DiagnosticIds.INJ0003ServiceTypeAndRegistrationStrategyIncompatible,
			title: "`ServiceType` and `RegistrationStrategy` are incompatible parameters",
			messageFormat: "Type `{0}` has attribute `[{1}]` applied with incompatible `ServiceType` and `RegistrationStrategy` parameters",
			category: "ImmediateInjections",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "`ServiceType` and `RegistrationStrategy` are incompatible parameters; providing both is an invalid scenario.",
			customTags: [WellKnownDiagnosticTags.NotConfigurable]
		);

	public static readonly DiagnosticDescriptor ServiceTypeIncompatibleWithTarget =
		new(
			id: DiagnosticIds.INJ0004ServiceTypeIncompatibleWithTarget,
			title: "Target class does not implement Service Type",
			messageFormat: "Type `{0}` does not implement Service Type `{1}`",
			category: "ImmediateInjections",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "The implementation type must implement or inherit the service type in order to be registered correctly.",
			customTags: [WellKnownDiagnosticTags.NotConfigurable]
		);

	public static readonly DiagnosticDescriptor TargetClassIsNotGeneric =
		new(
			id: DiagnosticIds.INJ0005TargetClassIsNotGeneric,
			title: "RegisterXxx<,> requires a generic target",
			messageFormat: "Type `{0}` is not generic",
			category: "ImmediateInjections",
			defaultSeverity: DiagnosticSeverity.Info,
			isEnabledByDefault: true,
			description: "Specifying the `TImplementation` is redundant when the target type is not generic."
		);

	public static readonly DiagnosticDescriptor TImplementationIsNotSameAsTarget =
		new(
			id: DiagnosticIds.INJ0006TImplementationIsNotSameAsTarget,
			title: "`TImplementation` is not the target class",
			messageFormat: "`TImplementation` type `{0}` is not the same as the target type `{1}`",
			category: "ImmediateInjections",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "When providing an implementation type, it must be of the same type as the target class.",
			customTags: [WellKnownDiagnosticTags.NotConfigurable]
		);

	public static readonly DiagnosticDescriptor CannotUseProxyFactoryOnSelf =
		new(
			id: DiagnosticIds.INJ0007CannotUseProxyFactoryOnSelf,
			title: "`UseProxyFactory` cannot be `true` when registering target as self",
			messageFormat: "Cannot register type `{0}` as itself when using a proxy",
			category: "ImmediateInjections",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description:
				"`UseProxyFactory` is used to register a proxy method which will return the instance of the target class. "
				+ "Proxying a type to itself will produce an infinite loop.",
			customTags: [WellKnownDiagnosticTags.NotConfigurable]
		);

	public static readonly DiagnosticDescriptor CannotUseProxyFactoryForOpenGeneric =
		new(
			id: DiagnosticIds.INJ0008CannotUseProxyFactoryForOpenGeneric,
			title: "`UseProxyFactory` cannot be true when registering an open generic",
			messageFormat: "Cannot register type `{0}` using a proxy for an open generic",
			category: "ImmediateInjections",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description:
				"`UseProxyFactory` is used to register a proxy method which will return the instance of the target class. "
				+ "The container for MSDI does not support creating a proxy for an open generic.",
			customTags: [WellKnownDiagnosticTags.NotConfigurable]
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create([
			AttributeIsInvalid,
			ServiceTypeAndRegistrationStrategyIncompatible,
			ServiceTypeIncompatibleWithTarget,
			TargetClassIsNotGeneric,
			TImplementationIsNotSameAsTarget,
			CannotUseProxyFactoryOnSelf,
			CannotUseProxyFactoryForOpenGeneric,
		]);

	public override void Initialize(AnalysisContext context)
	{
		if (context == null)
			throw new ArgumentNullException(nameof(context));

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
	}

	private void AnalyzeSymbol(SymbolAnalysisContext context)
	{
		var token = context.CancellationToken;
		token.ThrowIfCancellationRequested();

		var containerSymbol = (INamedTypeSymbol)context.Symbol;

		foreach (var attribute in context.Symbol.GetAttributes())
		{
			var location = attribute.ApplicationSyntaxReference?.GetSyntax(token).GetLocation();
			var analyzer = new TypeAnalyzer(context, containerSymbol, attribute, location);

			var isValid = attribute switch
			{
				{ AttributeClass.IsRegisterType0Attribute: true } =>
					analyzer.AnalyzeRegisterType0Attribute(),

				{ AttributeClass.IsRegisterType1Attribute: true } =>
					analyzer.AnalyzeRegisterType1Attribute(),

				{ AttributeClass.IsRegisterType2Attribute: true } =>
					analyzer.AnalyzeRegisterType2Attribute(),

				_ => true,
			};

			if (isValid)
				continue;

			context.ReportDiagnostic(
				Diagnostic.Create(
					AttributeIsInvalid,
					location,
					containerSymbol.Name,
					attribute.AttributeClass?.Name
				)
			);
		}
	}
}

file sealed class TypeAnalyzer(
	SymbolAnalysisContext context,
	INamedTypeSymbol containerSymbol,
	AttributeData attribute,
	Location? location
)
{
	public bool AnalyzeRegisterType0Attribute()
	{
		var valid = true;

		var assemblyAttributes = context.Compilation.Assembly.GetAttributes();
		var defaultsAttribute = assemblyAttributes.FirstOrDefault(a => a.AttributeClass.IsRegistrationDefaultsAttribute);
		var defaultRegistration = defaultsAttribute?.NamedArguments.GetEnumArgumentValue("RegistrationStrategy") ?? "None";

		var arguments = attribute.NamedArguments;
		var serviceType = arguments.GetArgumentValue("ServiceType")?.ArgumentType;
		var useProxy = arguments.GetArgumentValue("UseProxyFactory")?.Value is true;
		var registrationStrategy = arguments.GetEnumArgumentValue("RegistrationStrategy") ?? (serviceType is { } ? "None" : defaultRegistration);

		if (serviceType is { })
		{
			if (arguments.GetArgumentValue("RegistrationStrategy") is { })
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						RegisterTypeAnalyzer.ServiceTypeAndRegistrationStrategyIncompatible,
						location,
						containerSymbol.Name,
						attribute.AttributeClass?.Name
					)
				);

				valid = false;
			}

			var conversionType = ImplementsService(containerSymbol, serviceType);
			if (conversionType is ConversionType.Invalid)
				valid = false;

			if (useProxy)
			{
				if (conversionType is ConversionType.Identity)
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							RegisterTypeAnalyzer.CannotUseProxyFactoryOnSelf,
							location,
							containerSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
						)
					);

					valid = false;
				}

				if (serviceType.IsUnboundGenericType)
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							RegisterTypeAnalyzer.CannotUseProxyFactoryForOpenGeneric,
							location,
							containerSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
						)
					);

					valid = false;
				}
			}
		}

		switch (registrationStrategy)
		{
			case null:
			case "None":
			{
				if (serviceType is null)
					goto case "Self";

				break;
			}

			case "Self":
			{
				if (useProxy)
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							RegisterTypeAnalyzer.CannotUseProxyFactoryOnSelf,
							location,
							containerSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
						)
					);

					valid = false;
				}

				break;
			}

			case "ImplementedInterfaces":
			{
				break;
			}

			case "SelfAndImplementedInterfaces":
			{
				break;
			}

			default:
				valid = false;
				break;
		}

		return valid;
	}

	public bool AnalyzeRegisterType1Attribute()
	{
		var valid = true;

		if (attribute.AttributeClass is not
			{
				TypeArguments:
				[
					INamedTypeSymbol serviceTypeSymbol,
				],
			})
		{
			return valid;
		}

		var conversionType = ImplementsService(containerSymbol, serviceTypeSymbol);
		if (conversionType is ConversionType.Invalid)
			valid = false;

		var useProxy = attribute.NamedArguments.GetArgumentValue("UseProxyFactory")?.Value is true;
		if (conversionType is ConversionType.Identity && useProxy)
		{
			context.ReportDiagnostic(
				Diagnostic.Create(
					RegisterTypeAnalyzer.CannotUseProxyFactoryOnSelf,
					location,
					containerSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
				)
			);

			valid = false;
		}

		return valid;
	}

	public bool AnalyzeRegisterType2Attribute()
	{
		var valid = true;

		if (!containerSymbol.IsGenericType)
		{
			context.ReportDiagnostic(
				Diagnostic.Create(
					RegisterTypeAnalyzer.TargetClassIsNotGeneric,
					location,
					containerSymbol.Name,
					attribute.AttributeClass?.Name
				)
			);
		}

		if (attribute.AttributeClass is not
			{
				TypeArguments:
				[
					INamedTypeSymbol serviceTypeSymbol,
					INamedTypeSymbol implementationTypeSymbol,
				],
			})
		{
			return valid;
		}

		if (!SymbolEqualityComparer.Default.Equals(
				implementationTypeSymbol.OriginalDefinition,
				containerSymbol
			))
		{
			context.ReportDiagnostic(
				Diagnostic.Create(
					RegisterTypeAnalyzer.TImplementationIsNotSameAsTarget,
					location,
					implementationTypeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat),
					containerSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)
				)
			);

			valid = false;
		}

		var conversionType = ImplementsService(containerSymbol, serviceTypeSymbol);
		if (conversionType is ConversionType.Invalid)
			valid = false;

		var useProxy = attribute.NamedArguments.GetArgumentValue("UseProxyFactory")?.Value is true;
		if (conversionType is ConversionType.Identity && useProxy)
		{
			context.ReportDiagnostic(
				Diagnostic.Create(
					RegisterTypeAnalyzer.CannotUseProxyFactoryOnSelf,
					location,
					containerSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
				)
			);

			valid = false;
		}

		return valid;
	}

	private ConversionType ImplementsService(
		INamedTypeSymbol implementationTypeSymbol,
		INamedTypeSymbol serviceTypeSymbol
	)
	{
		var conversion = Core(implementationTypeSymbol, serviceTypeSymbol);

		if (conversion is ConversionType.Invalid)
		{
			context.ReportDiagnostic(
				Diagnostic.Create(
					RegisterTypeAnalyzer.ServiceTypeIncompatibleWithTarget,
					location,
					implementationTypeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat),
					serviceTypeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)
				)
			);
		}

		return conversion;

		ConversionType Core(
			INamedTypeSymbol implementationTypeSymbol,
			INamedTypeSymbol serviceTypeSymbol
		)
		{
			// covers non-generic and bound generics
			if (!implementationTypeSymbol.TypeArguments.Any(a => a is ITypeParameterSymbol))
				return Implements(implementationTypeSymbol, serviceTypeSymbol);

			if (implementationTypeSymbol.Arity != serviceTypeSymbol.Arity)
				return ConversionType.Invalid;

			if (serviceTypeSymbol.IsUnboundGenericType)
			{
				return Implements(
					implementationTypeSymbol,
					serviceTypeSymbol.ConstructedFrom.Construct(
						implementationTypeSymbol.TypeArguments,
						implementationTypeSymbol.TypeArgumentNullableAnnotations
					)
				);
			}

			if (!serviceTypeSymbol.TypeArguments.Any(a => a is ITypeParameterSymbol))
			{
				return Implements(
					implementationTypeSymbol.Construct(
						serviceTypeSymbol.TypeArguments,
						serviceTypeSymbol.TypeArgumentNullableAnnotations
					),
					serviceTypeSymbol
				);
			}

			return ConversionType.Invalid;
		}

		ConversionType Implements(
			INamedTypeSymbol implementationTypeSymbol,
			INamedTypeSymbol serviceTypeSymbol
		)
		{
			return context.Compilation.ClassifyConversion(implementationTypeSymbol, serviceTypeSymbol) switch
			{
				{ IsIdentity: true } => ConversionType.Identity,
				{ IsImplicit: true, IsReference: true } => ConversionType.Implicit,
				_ => ConversionType.Invalid,
			};
		}
	}

	private enum ConversionType
	{
		Invalid = 0,
		Identity = 1,
		Implicit = 2,
	}
}
