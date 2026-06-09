using static Immediate.Injections.Tests.Utility;

namespace Immediate.Injections.Tests.GeneratorTests;

public sealed class RegisterClass_TService_TImplementation_Tests
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

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task Record_IsRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}<IService<string>, Service<string>>]
			public record Service<T> : IService<T>
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

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task TService_TService_Registered(string lifetime)
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

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task NonGeneric_IsRegistered(string lifetime)
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
				$"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.Register{lifetime}`2.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task DifferentTarget_IsNotRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService;

			public class Dummy : IService;

			[Register{{lifetime}}<IService, Dummy>]
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

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task DifferentTargetGeneric_IsNotRegistered(string lifetime)
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

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task IncompatibleCast_IsNotRegistered(string lifetime)
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

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task InvalidFactoryWithUseProxy_IsNotRegistered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System;

			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}<IService<string>, Service<string>>(Factory = "BuildService", UseProxyFactory = true)]
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
	public async Task UseProxy_Registered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System;

			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}<IService<string>, Service<string>>(UseProxyFactory = true)]
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

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task KeyedUseProxy_Registered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System;

			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;

			[Register{{lifetime}}<IService<string>, Service<string>>(ServiceKey = "Key", UseProxyFactory = true)]
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

			[Register{{lifetime}}<IService<string>, Service<string>>]
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

			[Register{{lifetime}}<IService<string>, Service<string>>]
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

			[Register{{lifetime}}<IService<string>, Service<string>>]
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

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task AssemblyUseProxyFactory_Registered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			[assembly: RegistrationDefaults(UseProxyFactory = true)]

			public interface IService<T>;

			[Register{{lifetime}}<IService<string>, Service<string>>]
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

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task AssemblyUseProxyFactoryWithOverride_Registered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			[assembly: RegistrationDefaults(UseProxyFactory = true)]

			public interface IService<T>;

			[Register{{lifetime}}<IService<string>, Service<string>>(UseProxyFactory = false)]
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

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Theory]
	[MemberData(nameof(Lifetimes), MemberType = typeof(Utility))]
	public async Task AssemblyUseProxyFactory_Factory_Registered(string lifetime)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System;

			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			[assembly: RegistrationDefaults(UseProxyFactory = true)]

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

		_ = await VerifyIgnoreCommonFile(result)
			.UseParameters(lifetime);
	}

	[Fact]
	public async Task MultipleAttributes_Registered()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService1<T>;
			public interface IService2<T>;

			[RegisterScoped<IService1<int>, Service<int>>]
			[RegisterScoped<IService1<string>, Service<string>>]
			[RegisterSingleton<IService2<int>, Service<int>>]
			[RegisterSingleton<IService2<string>, Service<string>>]
			public class Service<T> : IService1<T>, IService2<T>
			{
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.RegisterScoped`2.g.cs",
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.RegisterSingleton`2.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result);
	}

}
