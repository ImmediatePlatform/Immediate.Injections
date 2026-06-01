using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Immediate.Injections.Generators;

[Generator]
public sealed partial class ImmediateInjectionsGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var assemblyDefaults = GetAssemblyDefaults(context);
		RenderServiceCollectionExtensions(context, assemblyDefaults);

		ProcessRegisterServicesMethods(context, assemblyDefaults);

		foreach (var lifetime in new[] { "Scoped", "Singleton", "Transient" })
		{
			ProcessRegisterClass0(context, assemblyDefaults, lifetime);
			ProcessRegisterClass1(context, assemblyDefaults, lifetime);
			ProcessRegisterClass2(context, assemblyDefaults, lifetime);
		}
	}

	private static IncrementalValueProvider<AssemblyDefaults> GetAssemblyDefaults(IncrementalGeneratorInitializationContext context)
	{
		var assemblyName = context.CompilationProvider
			.Select((cp, _) => new
			{
				AssemblyName = cp.GetAssemblyIdentifier(),
				LanguageVersion = (cp.SyntaxTrees.FirstOrDefault()?.Options as CSharpParseOptions)?.LanguageVersion ?? LanguageVersion.CSharp12,
			})
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
				AssemblyName = x.Left.Left.AssemblyName,
				LanguageVersion = x.Left.Left.LanguageVersion,
				RootNamespace = x.Left.Right,

				DuplicateStrategy = x.Right?.DuplicateStrategy ?? "Append",
			})
			.WithTrackingName("AssemblyDefaults");

		return assemblyDefaults;
	}

	private static void ProcessRegisterServicesMethods(IncrementalGeneratorInitializationContext context, IncrementalValueProvider<AssemblyDefaults> assemblyDefaults)
	{
		var methods = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				"Immediate.Injections.Shared.RegisterServicesAttribute",
				(node, _) => node is MethodDeclarationSyntax,
				TransformRegisterServicesMethod
			)
			.WhereNotNull()
			.Collect()
			.WithTrackingName("RegisterServicesMethods");

		RenderRegisterServicesMethods(context, assemblyDefaults, methods);
	}

	private static void ProcessRegisterClass0(
		IncrementalGeneratorInitializationContext context,
		IncrementalValueProvider<AssemblyDefaults> assemblyDefaults,
		string lifetime
	)
	{
		var attributeQualifiedName = $"Immediate.Injections.Shared.Register{lifetime}Attribute";

		var classes = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				attributeQualifiedName,
				(node, _) => node is ClassDeclarationSyntax,
				TransformRegisterClass0
			)
			.SelectMany((x, _) => x)
			.Collect()
			.WithTrackingName($"Register{lifetime}");

		RenderRegisterClasses(context, assemblyDefaults, classes, lifetime, 0);
	}

	private static void ProcessRegisterClass1(
		IncrementalGeneratorInitializationContext context,
		IncrementalValueProvider<AssemblyDefaults> assemblyDefaults,
		string lifetime
	)
	{
		var attributeQualifiedName = $"Immediate.Injections.Shared.Register{lifetime}Attribute`1";

		var classes = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				attributeQualifiedName,
				(node, _) => node is ClassDeclarationSyntax,
				TransformRegisterClass1
			)
			.SelectMany((x, _) => x)
			.Collect()
			.WithTrackingName($"Register{lifetime}`1");

		RenderRegisterClasses(context, assemblyDefaults, classes, lifetime, 1);
	}

	private static void ProcessRegisterClass2(
		IncrementalGeneratorInitializationContext context,
		IncrementalValueProvider<AssemblyDefaults> assemblyDefaults,
		string lifetime
	)
	{
		var attributeQualifiedName = $"Immediate.Injections.Shared.Register{lifetime}Attribute`2";

		var classes = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				attributeQualifiedName,
				(node, _) => node is ClassDeclarationSyntax,
				TransformRegisterClass2
			)
			.SelectMany((x, _) => x)
			.Collect()
			.WithTrackingName($"Register{lifetime}`2");

		RenderRegisterClasses(context, assemblyDefaults, classes, lifetime, 2);
	}
}

file static class Extensions
{
	public static string GetAssemblyIdentifier(this Compilation compilation)
	{
		if (compilation.Assembly.GetAttributes()
				.FirstOrDefault(a => a.AttributeClass.IsImmediateAssemblyIdentifierAttribute)
				is { ConstructorArguments: [{ Value: string { Length: >= 1 } identifier }] }
			&& identifier[0] != '@'
			&& SyntaxFacts.IsValidIdentifier(identifier))
		{
			return identifier;
		}

		return compilation.AssemblyName!
			.Replace(".", string.Empty)
			.Replace(" ", string.Empty)
			.Trim();
	}
}
