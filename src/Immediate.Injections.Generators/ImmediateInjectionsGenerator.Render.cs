using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Scriban;

namespace Immediate.Injections.Generators;

public sealed partial class ImmediateInjectionsGenerator
{
	private static readonly Template ServiceCollectionExtensionsTemplate = GetTemplate("ServiceCollectionExtensions");
	private static readonly Template RegisterServicesTemplate = GetTemplate("RegisterServices");
	private static readonly Template RegisterClassesTemplate = GetTemplate("RegisterClasses");

	private static void RenderServiceCollectionExtensions(
		IncrementalGeneratorInitializationContext context,
		IncrementalValueProvider<AssemblyDefaults> assemblyDefaults
	)
	{
		context.RegisterSourceOutput(
			assemblyDefaults,
			(context, assemblyDefaults) =>
			{
				var source = ServiceCollectionExtensionsTemplate
					.Render(new
					{
						assemblyDefaults.AssemblyName,
						assemblyDefaults.LanguageVersion,
						assemblyDefaults.RootNamespace,

						Version = ThisAssembly.InformationalVersion,
					});

				context.CancellationToken.ThrowIfCancellationRequested();
				context.AddSource("II.ServiceCollectionExtensions.g.cs", source);
			}
		);
	}

	private static void RenderRegisterServicesMethods(
		IncrementalGeneratorInitializationContext context,
		IncrementalValueProvider<AssemblyDefaults> assemblyDefaults,
		IncrementalValueProvider<ImmutableArray<RegisterServicesMethod>> methods
	)
	{
		context.RegisterSourceOutput(
			methods.Combine(assemblyDefaults),
			(context, x) =>
			{
				var (methods, assemblyDefaults) = x;

				if (methods.Length == 0)
					return;

				var source = RegisterServicesTemplate
					.Render(new
					{
						assemblyDefaults.AssemblyName,
						assemblyDefaults.RootNamespace,

						Methods = methods,
					});

				context.CancellationToken.ThrowIfCancellationRequested();
				context.AddSource("II.RegisterServicesMethods.g.cs", source);
			}
		);
	}

	private static void RenderRegisterClasses(
		IncrementalGeneratorInitializationContext context,
		IncrementalValueProvider<AssemblyDefaults> assemblyDefaults,
		IncrementalValueProvider<ImmutableArray<RegisterClass>> classes,
		string lifetime,
		int arity
	)
	{
		context.RegisterSourceOutput(
			classes.Combine(assemblyDefaults),
			(context, x) =>
			{
				var (classes, assemblyDefaults) = x;

				if (classes.Length == 0)
					return;

				var source = RegisterClassesTemplate
					.Render(new
					{
						assemblyDefaults.AssemblyName,
						assemblyDefaults.RootNamespace,

						Arity = arity,
						Lifetime = lifetime,

						ClassesByTag = classes
							.Select(c => new
							{
								c.Implementation,
								c.ServiceType,
								c.ServiceKey,
								c.Tags,

								ServiceMethod = (c.DuplicateStrategy ?? assemblyDefaults.DuplicateStrategy ?? "Append") switch
								{
									"Append" => "Add",
									"Replace" => "Replace",
									"Skip" => "TryAdd",
									_ => "",
								},

								DescriptorType = (lifetime, c.ServiceKey) switch
								{
									("Scoped", { }) => "KeyedScoped",
									("Scoped", _) => "Scoped",
									("Singleton", { }) => "KeyedSingleton",
									("Singleton", _) => "Singleton",
									("Transient", { }) => "KeyedTransient",
									("Transient", _) => "Transient",
									_ => "",
								},
							})
							.GroupBy(c => c.Tags, StringComparer.Ordinal),
					});

				context.CancellationToken.ThrowIfCancellationRequested();
				context.AddSource(
					FormattableString.Invariant($"II.Register{lifetime}`{arity}.g.cs"),
					source
				);
			}
		);
	}

	private static Template GetTemplate(string name)
	{
		using var stream = Assembly
			.GetExecutingAssembly()
			.GetManifestResourceStream(
				$"Immediate.Injections.Generators.Templates.{name}.sbntxt"
			);

		Debug.Assert(stream is { });

		using var reader = new StreamReader(stream);
		return Template.Parse(reader.ReadToEnd());
	}
}
