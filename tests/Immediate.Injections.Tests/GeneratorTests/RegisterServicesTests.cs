using Microsoft.CodeAnalysis.CSharp;

namespace Immediate.Injections.Tests.GeneratorTests;

public sealed class RegisterServicesTests
{
	[Fact]
	public async Task ValidRegisterServicesMethodWhenLangVersionIs12()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public class Class
			{
				[RegisterServices]
				public static void CallMe(IServiceCollection services)
				{
				}
			}
			""",
			languageVersion: LanguageVersion.CSharp12
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.RegisterServicesMethods.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task ValidRegisterServicesMethodIsCalled1()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public class Class
			{
				[RegisterServices]
				public static void CallMe(IServiceCollection services)
				{
				}
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.RegisterServicesMethods.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}

	[Fact]
	public async Task ValidRegisterServicesMethodIsCalled2()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using System;

			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public class Class
			{
				[RegisterServices]
				public static void CallMe(IServiceCollection services, ReadOnlySpan<string> tags)
				{
				}
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.RegisterServicesMethods.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}
}
