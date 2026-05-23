namespace Immediate.Injections.Tests.GeneratorTests;

public sealed class RegisterClass_TService_TImplementation_Tests
{
	[Theory]
	[InlineData("Scoped")]
	[InlineData("Singleton")]
	[InlineData("Transient")]
	public async Task ValidRegisterXxx_TService_TImplementation_IsRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}<IService<string>, Service<string>>]
			public class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`2.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[InlineData("Scoped")]
	[InlineData("Singleton")]
	[InlineData("Transient")]
	public async Task ValidRegisterXxx_TService_TService_IsRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			[Register{{lifetime}}<Service<string>, Service<string>>]
			public class Service<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`2.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[InlineData("Scoped")]
	[InlineData("Singleton")]
	[InlineData("Transient")]
	public async Task NonGenericRegisterXxx_TService_TImplementation_IsNotRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService;

			[Register{{lifetime}}<IService, Service>]
			public class Service : IService
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[InlineData("Scoped")]
	[InlineData("Singleton")]
	[InlineData("Transient")]
	public async Task DifferentTargetRegisterXxx_TService_TImplementation_IsNotRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			public class Dummy<T> : IService<T>;

			[Register{{lifetime}}<IService<string>, Dummy<string>>]
			public class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[InlineData("Scoped")]
	[InlineData("Singleton")]
	[InlineData("Transient")]
	public async Task IncompatibleCastRegisterXxx_TService_TImplementation_IsNotRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}<IService<string>, Service<string>>]
			public class Service<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[InlineData("Scoped")]
	[InlineData("Singleton")]
	[InlineData("Transient")]
	public async Task RegisterXxx_TService_TImplementation_InvalidFactory(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}<IService<string>, Service<string>>(Factory = "Test")]
			public class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[InlineData("Scoped")]
	[InlineData("Singleton")]
	[InlineData("Transient")]
	public async Task RegisterXxx_TService_TImplementation_ValidFactory(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System;

			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}<IService<string>, Service<string>>(Factory = "BuildService")]
			public sealed class Service<T> : IService<T>
			{
				public static Service<T> BuildService(IServiceProvider sp)
				{
					return new();
				}
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`2.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[InlineData("Scoped")]
	[InlineData("Singleton")]
	[InlineData("Transient")]
	public async Task RegisterXxx_TService_TImplementation_ValidKeyedFactory(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System;

			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}<IService<string>, Service<string>>(ServiceKey = "Key", Factory = "BuildService")]
			public sealed class Service<T> : IService<T>
			{
				public static Service<T> BuildService(IServiceProvider sp, object key)
				{
					return new();
				}
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`2.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[InlineData("Scoped")]
	[InlineData("Singleton")]
	[InlineData("Transient")]
	public async Task RegisterXxx_TService_TImplementation_ValidKey(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}<IService<string>, Service<string>>(ServiceKey = "Key")]
			public sealed class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`2.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[InlineData("Scoped")]
	[InlineData("Singleton")]
	[InlineData("Transient")]
	public async Task RegisterXxx_TService_TImplementation_ValidTags(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}<IService<string>, Service<string>>(Tags = ["abc", "def"])]
			public sealed class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`2.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[InlineData("Scoped")]
	[InlineData("Singleton")]
	[InlineData("Transient")]
	public async Task RegisterXxx_TService_TImplementation_Append(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}<IService<string>, Service<string>>(DuplicateStrategy = DuplicateStrategy.Append)]
			public sealed class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`2.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[InlineData("Scoped")]
	[InlineData("Singleton")]
	[InlineData("Transient")]
	public async Task RegisterXxx_TService_TImplementation_Replace(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}<IService<string>, Service<string>>(DuplicateStrategy = DuplicateStrategy.Replace)]
			public sealed class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`2.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[InlineData("Scoped")]
	[InlineData("Singleton")]
	[InlineData("Transient")]
	public async Task RegisterXxx_TService_TImplementation_Skip(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}<IService<string>, Service<string>>(DuplicateStrategy = DuplicateStrategy.Skip)]
			public sealed class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`2.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result)
			.UseParameters(lifetime);
	}
}
