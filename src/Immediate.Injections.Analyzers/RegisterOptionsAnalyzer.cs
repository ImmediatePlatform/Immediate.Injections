using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Injections.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RegisterOptionsAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor OptionsConfigurationExtensionsIsMissing =
		new(
			id: DiagnosticIds.INJ0013OptionsConfigurationExtensionsIsMissing,
			title: "`Microsoft.Extensions.Options.ConfigurationExtensions` must be referenced",
			messageFormat: "Type '{0}' has attribute `[{1}]` but `Microsoft.Extensions.Options.ConfigurationExtensions` is not referenced",
			category: "ImmediateInjections",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Registered options are bound to configuration; add a package reference to `Microsoft.Extensions.Options.ConfigurationExtensions`.",
			customTags: [WellKnownDiagnosticTags.NotConfigurable]
		);

	public static readonly DiagnosticDescriptor OptionsDataAnnotationsIsMissing =
		new(
			id: DiagnosticIds.INJ0014OptionsDataAnnotationsIsMissing,
			title: "`Microsoft.Extensions.Options.DataAnnotations` must be referenced",
			messageFormat: "Type '{0}' has attribute `[{1}]` with `ValidateDataAnnotations` but `Microsoft.Extensions.Options.DataAnnotations` is not referenced",
			category: "ImmediateInjections",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Validating data annotations requires a package reference to `Microsoft.Extensions.Options.DataAnnotations`.",
			customTags: [WellKnownDiagnosticTags.NotConfigurable]
		);

	public static readonly DiagnosticDescriptor ValidatorIsInvalid =
		new(
			id: DiagnosticIds.INJ0015ValidatorIsInvalid,
			title: "The validator must implement `IValidateOptions<TOptions>` for the options type",
			messageFormat: "Validator `{0}` does not implement `IValidateOptions<{1}>`",
			category: "ImmediateInjections",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "The validator type of `[RegisterOptions<TValidator>]` must implement `IValidateOptions<TOptions>` where `TOptions` is the options type.",
			customTags: [WellKnownDiagnosticTags.NotConfigurable]
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create([
			OptionsConfigurationExtensionsIsMissing,
			OptionsDataAnnotationsIsMissing,
			ValidatorIsInvalid,
		]);

	public override void Initialize(AnalysisContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
	}

	private static void AnalyzeSymbol(SymbolAnalysisContext context)
	{
		var token = context.CancellationToken;
		token.ThrowIfCancellationRequested();

		foreach (var attribute in context.Symbol.GetAttributes())
		{
			if (!attribute.AttributeClass.IsRegisterOptionsAttribute)
				continue;

			var location = attribute.ApplicationSyntaxReference?.GetSyntax(token).GetLocation();

			if (!context.Compilation.HasOptionsConfigurationExtensions)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						OptionsConfigurationExtensionsIsMissing,
						location,
						context.Symbol.Name,
						attribute.AttributeClass?.Name
					)
				);
			}

			if (attribute.NamedArguments.GetArgumentValue("ValidateDataAnnotations")?.Value is true
				&& !context.Compilation.HasOptionsDataAnnotations)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						OptionsDataAnnotationsIsMissing,
						location,
						context.Symbol.Name,
						attribute.AttributeClass?.Name
					)
				);
			}

			if (attribute.AttributeClass is { TypeArguments: [{ TypeKind: not TypeKind.Error } validatorSymbol] }
				&& !validatorSymbol.AllInterfaces.Any(i =>
					i.IsValidateOptions
					&& SymbolEqualityComparer.Default.Equals(i.TypeArguments[0], context.Symbol)
				))
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						ValidatorIsInvalid,
						location,
						validatorSymbol.Name,
						context.Symbol.Name
					)
				);
			}
		}
	}
}
