using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Immediate.Injections.Generators;

public sealed partial class ImmediateInjectionsGenerator
{
	private sealed record HostedService(string Implementation, string? Tags);

	private static void ProcessRegisterHostedServices(
		IncrementalGeneratorInitializationContext context,
		IncrementalValueProvider<AssemblyDefaults> assemblyDefaults
	)
	{
		var services = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				"Immediate.Injections.Shared.RegisterHostedServiceAttribute",
				(node, _) => node is ClassDeclarationSyntax or RecordDeclarationSyntax,
				(context, token) =>
				{
					token.ThrowIfCancellationRequested();
					if (context.TargetSymbol is not INamedTypeSymbol { IsValidHostedService: true } target)
						return null;

					return new HostedService(
						target.ToDisplayString(DisplayNameFormatters.FullyQualifiedWithNullableFormat),
						context.Attributes[0].NamedArguments.GetArgumentValue("Tags")?.GetStringArray()
					);
				}
			)
			.WhereNotNull()
			.Collect()
			.WithTrackingName("RegisterHostedServices");

		context.RegisterSourceOutput(services.Combine(assemblyDefaults), (context, value) =>
		{
			if (value.Left.Length == 0)
				return;

			var source = GetTemplate("RegisterHostedServices").Render(new
			{
				value.Right.RootNamespace,
				ServicesByTag = value.Left.GroupBy(s => s.Tags, StringComparer.Ordinal),
			});
			context.CancellationToken.ThrowIfCancellationRequested();
			context.AddSource("II.RegisterHostedServices.g.cs", source);
		});
	}
}
