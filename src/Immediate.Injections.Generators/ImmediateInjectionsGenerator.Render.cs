using System.Reflection;
using Microsoft.CodeAnalysis;
using Scriban;

namespace Immediate.Injections.Generators;

public sealed partial class ImmediateInjectionsGenerator
{
	private static readonly Template ServiceCollectionExtensionsTemplate = GetTemplate("ServiceCollectionExtensions");

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
						assemblyDefaults.RootNamespace,

						Version = ThisAssembly.InformationalVersion,
					});

				context.CancellationToken.ThrowIfCancellationRequested();
				context.AddSource("II.ServiceCollectionExtensions.g.cs", source);
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

		using var reader = new StreamReader(stream);
		return Template.Parse(reader.ReadToEnd());
	}
}
