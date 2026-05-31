using static Immediate.Injections.Tests.Utility;

namespace Immediate.Injections.Tests.GeneratorTests;

public sealed class RegisterClass_SelfAndImplementedInterfaces_Generic_Tests
{
	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task IsRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces)]
			public class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`0.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task AssemblyDefault_Registered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			[assembly: RegistrationDefaults(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces)]

			public interface IService<T>;

			[Register{{lifetime}}]
			public class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`0.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task AssemblyDefault_IgnoredWithServiceType_Registered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			[assembly: RegistrationDefaults(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces)]

			public interface IService<T>;

			[Register{{lifetime}}(ServiceType = typeof(IService<>))]
			public class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`0.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task ServiceType_IsNotRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;
			
			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces, ServiceType = typeof(Service<>))]
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

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task UseProxy_IsNotRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System;

			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces, UseProxyFactory = true)]
			public sealed class Service<T> : IService<T>
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

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task Factory_IsNotRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System;

			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces, Factory = "BuildService")]
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
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task ValidKey_Registered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces, ServiceKey = "Key")]
			public sealed class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`0.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task ValidTags_Registered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces, Tags = ["abc", "def"])]
			public sealed class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`0.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task Append_Registered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces, DuplicateStrategy = DuplicateStrategy.Append)]
			public sealed class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`0.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task Replace_Registered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces, DuplicateStrategy = DuplicateStrategy.Replace)]
			public sealed class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`0.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task Skip_Registered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces, DuplicateStrategy = DuplicateStrategy.Skip)]
			public sealed class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`0.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task AssemblyAppend_Registered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			[assembly: RegistrationDefaults(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces, DuplicateStrategy = DuplicateStrategy.Append)]

			public interface IService<T>;

			[Register{{lifetime}}]
			public sealed class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`0.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task AssemblyReplace_Registered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			[assembly: RegistrationDefaults(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces, DuplicateStrategy = DuplicateStrategy.Replace)]

			public interface IService<T>;

			[Register{{lifetime}}]
			public sealed class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`0.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task AssemblySkip_Registered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			[assembly: RegistrationDefaults(DuplicateStrategy = DuplicateStrategy.Skip)]

			public interface IService<T>;

			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces)]
			public sealed class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`0.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

}
