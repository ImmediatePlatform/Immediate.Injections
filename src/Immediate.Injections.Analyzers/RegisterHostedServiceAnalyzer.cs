using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Injections.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RegisterHostedServiceAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor InvalidHostedService = new(
		id: DiagnosticIds.INJ0018InvalidHostedService,
		title: "RegisterHostedService target is invalid",
		messageFormat: "Type '{0}' has attribute `[RegisterHostedService]` but does not implement `IHostedService` or cannot be registered due to accessibility",
		category: "ImmediateInjections",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: "RegisterHostedService requires a concrete, non-generic, accessible implementation of Microsoft.Extensions.Hosting.IHostedService.",
		customTags: [WellKnownDiagnosticTags.NotConfigurable]
	);

	public static readonly DiagnosticDescriptor DuplicateHostedServiceOrder = new(
		id: DiagnosticIds.INJ0019DuplicateHostedServiceOrder,
		title: "Hosted service orders must be unique within an assembly",
		messageFormat: "Hosted service order '{0}' is used by more than one registration in this assembly",
		category: "ImmediateInjections",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: "Explicit RegisterHostedService orders must be unique across the assembly, regardless of tags.",
		customTags: [WellKnownDiagnosticTags.NotConfigurable, WellKnownDiagnosticTags.CompilationEnd]
	);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create([InvalidHostedService, DuplicateHostedServiceOrder]);

	public override void Initialize(AnalysisContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();
		context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
		context.RegisterCompilationAction(AnalyzeCompilation);
	}

	private static void AnalyzeSymbol(SymbolAnalysisContext context)
	{
		var token = context.CancellationToken;
		token.ThrowIfCancellationRequested();

		var type = (INamedTypeSymbol)context.Symbol;
		foreach (var attribute in type.GetAttributes())
		{
			if (!attribute.AttributeClass.IsRegisterHostedServiceAttribute
				|| type.IsValidHostedService)
			{
				continue;
			}

			var location = attribute.ApplicationSyntaxReference?.GetSyntax(token).GetLocation();
			context.ReportDiagnostic(Diagnostic.Create(InvalidHostedService, location, type.Name));
		}
	}
	private static void AnalyzeCompilation(CompilationAnalysisContext context)
	{
		var registrations = GetTypes(context.Compilation.Assembly.GlobalNamespace, context.CancellationToken)
			.SelectMany(type => type.GetAttributes())
			.Where(attribute => attribute.AttributeClass.IsRegisterHostedServiceAttribute)
			.Select(attribute => new
			{
				Attribute = attribute,
				Order = attribute.NamedArguments.GetArgumentValue("Order")?.Value as int?,
			})
			.Where(registration => registration.Order.HasValue)
			.GroupBy(registration => registration.Order);

		foreach (var group in registrations)
		{
			context.CancellationToken.ThrowIfCancellationRequested();

			if (!group.Skip(1).Any())
				continue;

			foreach (var registration in group)
			{
				var location = registration.Attribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation();
				context.ReportDiagnostic(Diagnostic.Create(DuplicateHostedServiceOrder, location, group.Key));
			}
		}
	}

	private static IEnumerable<INamedTypeSymbol> GetTypes(INamespaceOrTypeSymbol container, CancellationToken token)
	{
		foreach (var member in container.GetMembers())
		{
			token.ThrowIfCancellationRequested();
			if (member is INamedTypeSymbol type)
				yield return type;

			if (member is INamespaceOrTypeSymbol nested)
			{
				foreach (var nestedType in GetTypes(nested, token))
					yield return nestedType;
			}
		}
	}

}
