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

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create([
			AttributeIsInvalid,
			ServiceTypeAndRegistrationStrategyIncompatible,
			ServiceTypeIncompatibleWithTarget,
			TargetClassIsNotGeneric,
			TImplementationIsNotSameAsTarget,
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

		var arguments = attribute.NamedArguments;

		if (arguments.GetArgumentValue("ServiceType")?.ArgumentType is { } serviceTypeSymbol)
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

			if (!ImplementsService(containerSymbol, serviceTypeSymbol))
				valid = false;
		}
		else
		{
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

		if (!ImplementsService(containerSymbol, serviceTypeSymbol))
			valid = false;

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

		if (!ImplementsService(implementationTypeSymbol, serviceTypeSymbol))
			valid = false;

		return valid;
	}

	private bool ImplementsService(
		INamedTypeSymbol implementationTypeSymbol,
		INamedTypeSymbol serviceTypeSymbol
	)
	{
		if (Core(implementationTypeSymbol, serviceTypeSymbol))
			return true;

		context.ReportDiagnostic(
			Diagnostic.Create(
				RegisterTypeAnalyzer.ServiceTypeIncompatibleWithTarget,
				location,
				implementationTypeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat),
				serviceTypeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)
			)
		);

		return false;

		bool Core(
			INamedTypeSymbol implementationTypeSymbol,
			INamedTypeSymbol serviceTypeSymbol
		)
		{
			// covers non-generic and bound generics
			if (!implementationTypeSymbol.TypeArguments.Any(a => a is ITypeParameterSymbol))
				return Implements(implementationTypeSymbol, serviceTypeSymbol);

			if (implementationTypeSymbol.Arity != serviceTypeSymbol.Arity)
				return false;

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

			return false;
		}

		bool Implements(
			INamedTypeSymbol implementationTypeSymbol,
			INamedTypeSymbol serviceTypeSymbol
		)
		{
			return context.Compilation.ClassifyConversion(implementationTypeSymbol, serviceTypeSymbol) is
			{ IsIdentity: true } or { IsImplicit: true, IsReference: true };
		}
	}
}
