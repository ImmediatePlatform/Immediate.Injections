using Immediate.Injections.Analyzers;

namespace Immediate.Injections.Tests.AnalyzerTests;

public sealed partial class RegisterTypeAnalyzerTests
{
	[Fact]
	public async Task TService_TImplementation_IncompatibleNonGenericTypeParametersTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			public interface IService;
			
			[{|INJ0005:{|INJ0004:{|INJ0002:RegisterSingleton<IService, Class>|}|}|}]
			public class Class;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task TService_TImplementation_IncompatibleGenericTypeParametersTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			public interface IService<T>;
			
			[{|INJ0004:{|INJ0002:RegisterSingleton<IService<string>, Class<string>>|}|}]
			public class Class<T>;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task TService_IncompatibleNonGenericTypeParameterTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			public interface IService;
			
			[{|INJ0004:{|INJ0002:RegisterSingleton<IService>|}|}]
			public class Class;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task TService_IncompatibleGenericTypeParameterTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			public interface IService<T>;
			
			[{|INJ0004:{|INJ0002:RegisterSingleton<IService<string>>|}|}]
			public class Class<T>;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task IncompatibleUnboundGenericServiceTypeTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			public interface IService<T>;
			
			[{|INJ0004:{|INJ0002:RegisterSingleton(ServiceType = typeof(IService<>))|}|}]
			public class Class<T>;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task IncompatibleBoundGenericServiceTypeTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			public interface IService<T>;
			
			[{|INJ0004:{|INJ0002:RegisterSingleton(ServiceType = typeof(IService<string>))|}|}]
			public class Class<T>;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task IncompatibleNonGenericServiceTypeTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			public interface IService;
			
			[{|INJ0004:{|INJ0002:RegisterSingleton(ServiceType = typeof(IService))|}|}]
			public class Class;
			"""
		).RunAsync(TestContext.Current.CancellationToken);
}
