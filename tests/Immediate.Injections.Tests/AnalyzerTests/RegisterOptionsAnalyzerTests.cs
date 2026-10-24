using Immediate.Injections.Analyzers;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Immediate.Injections.Tests.AnalyzerTests;

public sealed class RegisterOptionsAnalyzerTests
{
	[Fact]
	public async Task OptionsWithReferencesDoesNotTrigger() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterOptionsAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			[RegisterOptions(ValidateDataAnnotations = true, ValidateOnStart = true)]
			public sealed class PositionOptions
			{
				public string Title { get; set; } = string.Empty;
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task MissingConfigurationExtensionsTriggers() =>
		await CreateAnalyzerTest(
			"""
			using Immediate.Injections.Shared;

			[{|INJ0013:RegisterOptions|}]
			public sealed class PositionOptions
			{
				public string Title { get; set; } = string.Empty;
			}
			""",
			"Microsoft.Extensions.Options.ConfigurationExtensions"
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task MissingDataAnnotationsTriggers() =>
		await CreateAnalyzerTest(
			"""
			using Immediate.Injections.Shared;

			[{|INJ0014:RegisterOptions(ValidateDataAnnotations = true)|}]
			public sealed class PositionOptions
			{
				public string Title { get; set; } = string.Empty;
			}
			""",
			"Microsoft.Extensions.Options.DataAnnotations"
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task MissingDataAnnotationsWithoutValidateDataAnnotationsDoesNotTrigger() =>
		await CreateAnalyzerTest(
			"""
			using Immediate.Injections.Shared;

			[RegisterOptions]
			public sealed class PositionOptions
			{
				public string Title { get; set; } = string.Empty;
			}
			""",
			"Microsoft.Extensions.Options.DataAnnotations"
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task ValidatorForOtherOptionsTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterOptionsAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.Options;

			[{|INJ0015:RegisterOptions<OtherOptionsValidator>|}]
			public sealed class PositionOptions
			{
				public string Title { get; set; } = string.Empty;
			}

			public sealed class OtherOptions;

			public sealed class OtherOptionsValidator : IValidateOptions<OtherOptions>
			{
				public ValidateOptionsResult Validate(string? name, OtherOptions options) =>
					ValidateOptionsResult.Success;
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task AbstractOptionsTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterOptionsAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			[{|INJ0016:RegisterOptions|}]
			public abstract class PositionOptions
			{
				public string Title { get; set; } = string.Empty;
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task AbstractValidatorTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterOptionsAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.Options;

			[{|INJ0017:RegisterOptions<PositionOptionsValidator>|}]
			public sealed class PositionOptions
			{
				public string Title { get; set; } = string.Empty;
			}

			public abstract class PositionOptionsValidator : IValidateOptions<PositionOptions>
			{
				public abstract ValidateOptionsResult Validate(string? name, PositionOptions options);
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	private static CSharpAnalyzerTest<RegisterOptionsAnalyzer, DefaultVerifier> CreateAnalyzerTest(
		string inputSource,
		string removedAssembly
	)
	{
		var test = AnalyzerTestHelpers.CreateAnalyzerTest<RegisterOptionsAnalyzer>(inputSource);

		_ = test.TestState.AdditionalReferences.RemoveAll(r =>
			string.Equals(Path.GetFileNameWithoutExtension(r.Display), removedAssembly, StringComparison.Ordinal)
		);

		return test;
	}
}
