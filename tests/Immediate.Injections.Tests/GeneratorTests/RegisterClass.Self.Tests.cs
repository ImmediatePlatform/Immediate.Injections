using static Immediate.Injections.Tests.Utility;

namespace Immediate.Injections.Tests.GeneratorTests;

public sealed class RegisterClass_Self_Tests
{
	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task IsRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService;

			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.Self)]
			public class Service : IService
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
	public async Task AssemblyDefault_Registered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			[assembly: RegistrationDefaults(RegistrationStrategy = RegistrationStrategy.Self)]

			public interface IService;

			[Register{{lifetime}}]
			public class Service : IService
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
	public async Task AssemblyDefault_IgnoredWithServiceType_Registered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			[assembly: RegistrationDefaults(RegistrationStrategy = RegistrationStrategy.Self)]

			public interface IService;

			[Register{{lifetime}}(ServiceType = typeof(IService))]
			public class Service : IService
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
	public async Task ServiceType_IsNotRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService;
			
			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.Self, ServiceType = typeof(Service))]
			public class Service : IService
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
	public async Task InvalidFactory_IsNotRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService;

			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.Self, Factory = "Test")]
			public class Service : IService
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
	public async Task UseProxy_IsNotRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System;

			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService;

			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.Self, UseProxyFactory = true)]
			public sealed class Service : IService
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
	public async Task ValidFactory_Registered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System;

			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService;

			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.Self, Factory = "BuildService")]
			public sealed class Service : IService
			{
				public static Service BuildService(IServiceProvider sp)
				{
					return new();
				}
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
	public async Task ValidKeyedFactory_Registered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System;

			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService;

			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.Self, ServiceKey = "Key", Factory = "BuildService")]
			public sealed class Service : IService
			{
				public static Service BuildService(IServiceProvider sp, object key)
				{
					return new();
				}
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
	public async Task ValidKey_Registered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService;

			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.Self, ServiceKey = "Key")]
			public sealed class Service : IService
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
			
			public interface IService;

			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.Self, Tags = ["abc", "def"])]
			public sealed class Service : IService
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
			
			public interface IService;

			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.Self, DuplicateStrategy = DuplicateStrategy.Append)]
			public sealed class Service : IService
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
			
			public interface IService;

			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.Self, DuplicateStrategy = DuplicateStrategy.Replace)]
			public sealed class Service : IService
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
			
			public interface IService;

			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.Self, DuplicateStrategy = DuplicateStrategy.Skip)]
			public sealed class Service : IService
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
			
			[assembly: RegistrationDefaults(RegistrationStrategy = RegistrationStrategy.Self, DuplicateStrategy = DuplicateStrategy.Append)]

			public interface IService;

			[Register{{lifetime}}]
			public sealed class Service : IService
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
			
			[assembly: RegistrationDefaults(RegistrationStrategy = RegistrationStrategy.Self, DuplicateStrategy = DuplicateStrategy.Replace)]

			public interface IService;

			[Register{{lifetime}}]
			public sealed class Service : IService
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

			public interface IService;

			[Register{{lifetime}}(RegistrationStrategy = RegistrationStrategy.Self)]
			public sealed class Service : IService
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
