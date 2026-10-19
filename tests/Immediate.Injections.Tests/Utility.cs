using System.Runtime.CompilerServices;
using Immediate.Handlers.Shared;
using Immediate.Injections.Shared;
using Immediate.Validations.Shared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Immediate.Injections.Tests;

public static class Utility
{
#if NET8_0
	public static ReferenceAssemblies ReferenceAssemblies => ReferenceAssemblies.Net.Net80;
	public static IEnumerable<MetadataReference> NetCoreAssemblies => Basic.Reference.Assemblies.Net80.References.All;
#elif NET9_0
	public static ReferenceAssemblies ReferenceAssemblies => ReferenceAssemblies.Net.Net90;
	public static IEnumerable<MetadataReference> NetCoreAssemblies => Basic.Reference.Assemblies.Net90.References.All;
#elif NET10_0
	public static ReferenceAssemblies ReferenceAssemblies => ReferenceAssemblies.Net.Net100;
	public static IEnumerable<MetadataReference> NetCoreAssemblies => Basic.Reference.Assemblies.Net100.References.All;
#elif NET11_0
	public static ReferenceAssemblies ReferenceAssemblies { get; } = new ReferenceAssemblies(
		"net11.0",
		new PackageIdentity(
			"Microsoft.NETCore.App.Ref",
			"11.0.0-rc.1.26425.128"
		),
		Path.Combine("ref", "net11.0")
	);
	public static IEnumerable<MetadataReference> NetCoreAssemblies => Basic.Reference.Assemblies.Net110.References.All;
#else
#error .net version not yet implemented
#endif

	public static IEnumerable<MetadataReference> GetAdditionalReferences() =>
	[
		MetadataReference.CreateFromFile(typeof(RegisterServicesAttribute).Assembly.Location),
		MetadataReference.CreateFromFile(typeof(HandlerAttribute).Assembly.Location),
		MetadataReference.CreateFromFile(typeof(ValidateAttribute).Assembly.Location),
		MetadataReference.CreateFromFile(typeof(ServiceCollection).Assembly.Location),
		MetadataReference.CreateFromFile(typeof(IServiceCollection).Assembly.Location),
		MetadataReference.CreateFromFile(typeof(Microsoft.Extensions.Options.IOptions<>).Assembly.Location),
		MetadataReference.CreateFromFile(typeof(OptionsBuilderConfigurationExtensions).Assembly.Location),
		MetadataReference.CreateFromFile(typeof(OptionsBuilderDataAnnotationsExtensions).Assembly.Location),
		MetadataReference.CreateFromFile(typeof(Microsoft.Extensions.Configuration.BinderOptions).Assembly.Location),
		MetadataReference.CreateFromFile(typeof(Microsoft.Extensions.Configuration.IConfiguration).Assembly.Location),
	];

	public static TheoryData<string> Lifetimes { get; } = ["Scoped", "Singleton", "Transient"];

	public static SettingsTask VerifyIgnoreCommonFile(GeneratorDriverRunResult result, [CallerFilePath] string sourceFile = "") =>
		Verify(result, sourceFile: sourceFile)
			.IgnoreGeneratedResult(gsr => Path.GetFileName(gsr.HintName) is "II.ServiceCollectionExtensions.g.cs")
			.IgnoreGeneratedResult(gsr => Path.GetFileName(gsr.HintName) is "IH.ServiceCollectionExtensions.g.cs")
			.IgnoreGeneratedResult(gsr => Path.GetFileName(gsr.HintName).StartsWith("IV.", StringComparison.Ordinal));
}
