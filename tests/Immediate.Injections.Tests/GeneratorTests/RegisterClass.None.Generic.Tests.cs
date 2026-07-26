using static Immediate.Injections.Tests.Utility;

namespace Immediate.Injections.Tests.GeneratorTests;

public sealed class RegisterClass_None_Generic_Tests
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
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task ServiceType_IsRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
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
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task ServiceType_Concrete_IsRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;
			
			[Register{{lifetime}}(ServiceType = typeof(IService<string>))]
			public class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`0.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task ServiceType_IncompatibleCast_IsNotRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}(ServiceType = typeof(IService<>))]
			public class Service<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task ServiceType_NonGeneric_IsNotRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService;

			[Register{{lifetime}}(ServiceType = typeof(IService))]
			public class Service<T> : IService
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task ServiceType_Concrete_IncompatibleCast_IsNotRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}(ServiceType = typeof(IService<string>))]
			public class Service<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task ServiceType_IncompatibleArity_IsNotRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}(ServiceType = typeof(IService<>))]
			public class Service<T1, T2> : IService<T1>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
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

			[Register{{lifetime}}(Factory = "BuildService")]
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
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task UseProxyWithServiceType_IsNotRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System;

			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}(ServiceType = typeof(IService<>), UseProxyFactory = true)]
			public sealed class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
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

			[Register{{lifetime}}(ServiceKey = "Key")]
			public sealed class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`0.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
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

			[Register{{lifetime}}(Tags = ["abc", "def"])]
			public sealed class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`0.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
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

			[Register{{lifetime}}(DuplicateStrategy = DuplicateStrategy.Append)]
			public sealed class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`0.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
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

			[Register{{lifetime}}(DuplicateStrategy = DuplicateStrategy.Replace)]
			public sealed class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`0.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
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

			[Register{{lifetime}}(DuplicateStrategy = DuplicateStrategy.Skip)]
			public sealed class Service<T> : IService<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`0.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
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
			
			[assembly: RegistrationDefaults(DuplicateStrategy = DuplicateStrategy.Append)]

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
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
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
			
			[assembly: RegistrationDefaults(DuplicateStrategy = DuplicateStrategy.Replace)]

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
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
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
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

}
