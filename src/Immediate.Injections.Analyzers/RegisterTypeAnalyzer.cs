using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Injections.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RegisterTypeAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor AttributeIsInvalid =
		new(
			id: DiagnosticIds.INJ0002AttributeIsInvalid,
			title: "Attribute application is invalid",
			messageFormat: "Class '{0}' has attribute `[{1}]` and will not be registered",
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
			messageFormat: "Class '{0}' has attribute `[{1}]` applied with incompatible `ServiceType` and `RegistrationStrategy` parameters",
			category: "ImmediateInjections",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "`ServiceType` and `RegistrationStrategy` are incompatible parameters; providing both is an invalid scenario.",
			customTags: [WellKnownDiagnosticTags.NotConfigurable]
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create([
			AttributeIsInvalid,
			ServiceTypeAndRegistrationStrategyIncompatible,
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

			var diagnostics = attribute switch
			{
				{ AttributeClass.IsRegisterType0Attribute: true, NamedArguments: var arguments } =>
					AnalyzeRegisterType0Attribute(context, containerSymbol, arguments),

				{ AttributeClass.IsRegisterType1Attribute: true, NamedArguments: var arguments } =>
					AnalyzeRegisterType1Attribute(context, containerSymbol, arguments),

				{ AttributeClass.IsRegisterType2Attribute: true, NamedArguments: var arguments } =>
					AnalyzeRegisterType2Attribute(context, containerSymbol, arguments),

				_ => [],
			};

			if (diagnostics is [])
				continue;

			foreach (var diagnostic in diagnostics)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						diagnostic,
						location,
						containerSymbol.Name,
						attribute.AttributeClass?.Name
					)
				);
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					AttributeIsInvalid,
					location,
					attribute.AttributeClass?.Name
				)
			);
		}
	}

	private static List<DiagnosticDescriptor> AnalyzeRegisterType0Attribute(
		SymbolAnalysisContext context,
		INamedTypeSymbol containerSymbol,
		ImmutableArray<KeyValuePair<string, TypedConstant>> arguments
	)
	{
		var diagnostics = new List<DiagnosticDescriptor>();

		if (arguments.GetArgumentValue("ServiceType") is { }
			&& arguments.GetArgumentValue("RegistrationStrategy") is { })
		{
			diagnostics.Add(ServiceTypeAndRegistrationStrategyIncompatible);
		}

		return diagnostics;
	}

	private static List<DiagnosticDescriptor> AnalyzeRegisterType1Attribute(
		SymbolAnalysisContext context,
		INamedTypeSymbol containerSymbol,
		ImmutableArray<KeyValuePair<string, TypedConstant>> arguments
	)
	{
		var diagnostics = new List<DiagnosticDescriptor>();

		return diagnostics;
	}

	private static List<DiagnosticDescriptor> AnalyzeRegisterType2Attribute(
		SymbolAnalysisContext context,
		INamedTypeSymbol containerSymbol,
		ImmutableArray<KeyValuePair<string, TypedConstant>> arguments
	)
	{
		var diagnostics = new List<DiagnosticDescriptor>();

		return diagnostics;
	}
}
