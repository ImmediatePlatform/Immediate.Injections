using Immediate.Injections.Analyzers;

namespace Immediate.Injections.Tests.AnalyzerTests;

public sealed partial class RegisterTypeAnalyzerTests
{
	[Fact]
	public async Task TService_TImplementation_SimpleApplication() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			public interface IService<T>;
			
			[RegisterSingleton<IService<string>, Class<string>>]
			public class Class<T> : IService<T>;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task TService_TImplementation_Factory() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using System;

			using Immediate.Injections.Shared;

			public interface IService<T>;
			
			[RegisterSingleton<IService<string>, Class<string>>(Factory = nameof(Factory))]
			public class Class<T> : IService<T>
			{
				public static Class<T> Factory(IServiceProvider provider) => new();
			};
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task TService_SimpleApplication1() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			public interface IService;
			
			[RegisterSingleton<IService>]
			public class Class : IService;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task TService_SimpleApplication2() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			public interface IService<T>;
			
			[RegisterSingleton<IService<string>>]
			public class Class<T> : IService<T>;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task TService_Factory() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using System;

			using Immediate.Injections.Shared;

			public interface IService<T>;
			
			[RegisterSingleton<IService<string>>(Factory = nameof(Factory))]
			public class Class<T> : IService<T>
			{
				public static Class<T> Factory(IServiceProvider provider) => new();
			};
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task SimpleApplication1() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			public interface IService<T>;
			
			[RegisterSingleton(ServiceType = typeof(IService<>))]
			public class Class<T> : IService<T>;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task SimpleApplication2() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			public interface IService<T>;
			
			[RegisterSingleton(ServiceType = typeof(IService<string>))]
			public class Class<T> : IService<T>;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task SimpleApplication3() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			public interface IService;
			
			[RegisterSingleton(ServiceType = typeof(IService))]
			public class Class : IService;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task Factory() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using System;

			using Immediate.Injections.Shared;

			public interface IService;
			
			[RegisterSingleton(Factory = nameof(Factory))]
			public class Class : IService
			{
				public static Class Factory(IServiceProvider provider) => new();
			};
			"""
		).RunAsync(TestContext.Current.CancellationToken);
}
