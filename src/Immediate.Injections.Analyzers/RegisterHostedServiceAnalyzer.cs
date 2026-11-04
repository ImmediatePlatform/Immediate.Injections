using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Injections.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RegisterHostedServiceAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor InvalidHostedService = new(
		id: "INJ0018",
		title: "Hosted services must be concrete non-generic implementations of IHostedService",
		messageFormat: "Type '{0}' must be a concrete non-generic implementation of IHostedService",
		category: "ImmediateInjections",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: "RegisterHostedService requires a concrete non-generic implementation of Microsoft.Extensions.Hosting.IHostedService.",
		customTags: [WellKnownDiagnosticTags.NotConfigurable]
	);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create([InvalidHostedService]);

	public override void Initialize(AnalysisContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();
		context.RegisterSymbolAction(context =>
		{
			var type = (INamedTypeSymbol)context.Symbol;
			foreach (var attribute in type.GetAttributes())
			{
				if (attribute.AttributeClass is not
					{ Name: "RegisterHostedServiceAttribute", ContainingNamespace.IsImmediateInjectionsShared: true }
					|| type.IsValidHostedService)
					continue;

				var location = attribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation();
				context.ReportDiagnostic(Diagnostic.Create(InvalidHostedService, location, type.Name));
			}
		}, SymbolKind.NamedType);
	}
}
