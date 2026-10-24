using static Immediate.Injections.Tests.Utility;

namespace Immediate.Injections.Tests.GeneratorTests;

public sealed class RegisterOptionsTests
{
	[Fact]
	public async Task WithoutValidator_IsRegistered()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Injections.Shared;

			[RegisterOptions(ValidateOnStart = true)]
			public sealed class PositionOptions
			{
				public string Title { get; set; } = string.Empty;
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.RegisterOptions.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result);
	}

	[Fact]
	public async Task WithValidator_IsRegistered()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.Options;

			[RegisterOptions<PositionOptionsValidator>("Something:Position")]
			public sealed class PositionOptions
			{
				public int Age { get; set; }
			}

			public sealed class PositionOptionsValidator : IValidateOptions<PositionOptions>
			{
				public ValidateOptionsResult Validate(string name, PositionOptions options) =>
					ValidateOptionsResult.Success;
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.RegisterOptions.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result);
	}

	[Fact]
	public async Task WithImmediateValidations_IsRegistered()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Injections.Shared;
			using Immediate.Validations.Shared;

			[Validate]
			[RegisterOptions]
			public sealed partial class PositionOptions : IValidationTarget<PositionOptions>
			{
				[NotEmpty]
				public string Title { get; set; } = string.Empty;
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.RegisterOptions.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
				"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...PositionOptions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result);
	}

	[Fact]
	public async Task WithImmediateValidations_ValidateOnStart_IsRegistered()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Injections.Shared;
			using Immediate.Validations.Shared;

			[Validate]
			[RegisterOptions("Something:Position", ValidateOnStart = true)]
			public sealed partial record PositionOptions : IValidationTarget<PositionOptions>
			{
				[GreaterThan(0)]
				public int Age { get; set; }
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.RegisterOptions.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
				"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...PositionOptions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result);
	}

	[Fact]
	public async Task WithImmediateValidationsAndValidator_IsRegistered()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Injections.Shared;
			using Immediate.Validations.Shared;
			using Microsoft.Extensions.Options;

			[Validate]
			[RegisterOptions<PositionOptionsValidator>(ValidateOnStart = true)]
			public sealed partial class PositionOptions : IValidationTarget<PositionOptions>
			{
				[NotEmpty]
				public string Title { get; set; } = string.Empty;
			}

			public sealed class PositionOptionsValidator : IValidateOptions<PositionOptions>
			{
				public ValidateOptionsResult Validate(string name, PositionOptions options) =>
					ValidateOptionsResult.Success;
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.RegisterOptions.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
				"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV...PositionOptions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result);
	}

	[Fact]
	public async Task WithAndWithoutImmediateValidations_AreRegistered()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Injections.Shared;
			using Immediate.Validations.Shared;

			namespace Some.Namespace;

			[Validate]
			[RegisterOptions]
			public sealed partial class PositionOptions : IValidationTarget<PositionOptions>
			{
				[NotEmpty]
				public string Title { get; set; } = string.Empty;
			}

			[RegisterOptions]
			public sealed class OtherOptions
			{
				public string Title { get; set; } = string.Empty;
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.RegisterOptions.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
				"Immediate.Validations.Generators/Immediate.Validations.Generators.ImmediateValidationsGenerator/IV.Some.Namespace..PositionOptions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result);
	}
}
