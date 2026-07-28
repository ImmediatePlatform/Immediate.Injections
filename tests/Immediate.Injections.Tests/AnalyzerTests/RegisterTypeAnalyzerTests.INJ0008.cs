using Immediate.Injections.Analyzers;

namespace Immediate.Injections.Tests.AnalyzerTests;

public sealed partial class RegisterTypeAnalyzerTests
{
	[Fact]
	public async Task UseProxyFactoryWithOpenGenericServiceTypeTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			
			public interface IService<T>;

			[{|INJ0008:{|INJ0002:RegisterSingleton(ServiceType = typeof(IService<>), UseProxyFactory = true)|}|}]
			public class Class<T> : IService<T>;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task UseProxyFactoryWithInferredOpenGenericStrategyTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			public interface IService<T>;

			[{|INJ0008:{|INJ0002:RegisterSingleton(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces, UseProxyFactory = true)|}|}]
			public class Class<T> : IService<T>;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task UseProxyFactoryWithAssemblyDefaultInferredOpenGenericTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			[assembly: RegistrationDefaults(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)]

			public interface IService<T>;

			[{|INJ0008:{|INJ0002:RegisterSingleton(UseProxyFactory = true)|}|}]
			public class Class<T> : IService<T>;
			"""
		).RunAsync(TestContext.Current.CancellationToken);
}
