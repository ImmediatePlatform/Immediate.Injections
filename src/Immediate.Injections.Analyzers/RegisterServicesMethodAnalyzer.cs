using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Injections.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RegisterServicesMethodAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor RegisterServicesMethodIsInvalid =
		new(
			id: DiagnosticIds.INJ0001RegisterServicesMethodIsInvalid,
			title: "RegisterServices method is invalid",
			messageFormat: "Method '{0}' must be a static void method receiving an `IServiceCollection` parameter and optionally a `ReadOnlySpan<string>` parameter",
			category: "ImmediateInjections",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "RegisterServices methods must follow a precise format to be called from the generated code.",
			customTags: [WellKnownDiagnosticTags.NotConfigurable]
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create([RegisterServicesMethodIsInvalid]);

	public override void Initialize(AnalysisContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
	}

	private static void AnalyzeMethod(SymbolAnalysisContext context)
	{
		var token = context.CancellationToken;
		token.ThrowIfCancellationRequested();

		var methodSymbol = (IMethodSymbol)context.Symbol;

		if (!methodSymbol.GetAttributes().Any(a => a.AttributeClass.IsRegisterServicesAttribute))
			return;

		if (methodSymbol.IsValidRegisterServicesMethod)
			return;

		var location = methodSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax(token) switch
		{
			MethodDeclarationSyntax { Identifier: { } identifier } => identifier.GetLocation(),
			_ => methodSymbol.Locations[0],
		};

		context.ReportDiagnostic(
			Diagnostic.Create(
				RegisterServicesMethodIsInvalid,
				location,
				methodSymbol.Name
			)
		);
	}
}
