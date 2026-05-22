using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Immediate.Injections.Generators;

[Generator]
public sealed partial class ImmediateInjectionsGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var assemblyDefaults = GetAssemblyDefaults(context);

		RenderServiceCollectionExtensions(context, assemblyDefaults);
	}

	private static IncrementalValueProvider<AssemblyDefaults> GetAssemblyDefaults(IncrementalGeneratorInitializationContext context)
	{
		var assemblyName = context.CompilationProvider
			.Select((cp, _) => cp.AssemblyName!
				.Replace(".", string.Empty)
				.Replace(" ", string.Empty)
				.Trim()
			)
			.WithTrackingName("AssemblyName");

		var @namespace = context
			.AnalyzerConfigOptionsProvider
			.Select(
				(c, _) => c.GlobalOptions
					.TryGetValue("build_property.rootnamespace", out var ns)
						? ns : ""
			)
			.WithTrackingName("RootNamespace");

		var assemblyRegistrationDefaults = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				"Immediate.Injections.Shared.RegistrationDefaultsAttribute",
				(node, _) => node is CompilationUnitSyntax,
				TransformAssemblyDefaults
			)
			.Collect()
			.Select((x, _) => x.FirstOrDefault())
			.WithTrackingName("AssemblyRegistrationDefaults");

		var assemblyDefaults = assemblyName
			.Combine(@namespace)
			.Combine(assemblyRegistrationDefaults)
			.Select((x, _) => new AssemblyDefaults
			{
				AssemblyName = x.Left.Left,
				RootNamespace = x.Left.Right,
				DuplicateStrategy = x.Right?.DuplicateStrategy ?? "Append",
				RegistrationStrategy = x.Right?.RegistrationStrategy ?? "Self",
			})
			.WithTrackingName("AssemblyDefaults");

		return assemblyDefaults;
	}
}
