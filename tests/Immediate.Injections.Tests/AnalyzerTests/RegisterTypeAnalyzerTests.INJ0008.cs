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
	public async Task UseProxyFactoryWithInferredOpenGenericSelfStrategyTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			[{|INJ0007:{|INJ0008:{|INJ0002:RegisterSingleton(RegistrationStrategy = RegistrationStrategy.Self, UseProxyFactory = true)|}|}|}]
			public class Class<T>;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task UseProxyFactoryWithInferredOpenGenericImplementedInterfacesStrategyTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			public interface IService<T>;

			[{|INJ0008:{|INJ0002:RegisterSingleton(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces, UseProxyFactory = true)|}|}]
			public class Class<T> : IService<T>;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task UseProxyFactoryWithInferredOpenGenericSelfAndImplementedInterfacesStrategyTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			public interface IService<T>;

			[{|INJ0008:{|INJ0002:RegisterSingleton(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces, UseProxyFactory = true)|}|}]
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

	[Fact]
	public async Task UseProxyFactoryWithAssemblyDefaultSelfInferredOpenGenericTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			[assembly: RegistrationDefaults(RegistrationStrategy = RegistrationStrategy.Self)]

			[{|INJ0007:{|INJ0008:{|INJ0002:RegisterSingleton(UseProxyFactory = true)|}|}|}]
			public class Class<T>;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task UseProxyFactoryWithAssemblyDefaultSelfAndImplementedInterfacesInferredOpenGenericTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			[assembly: RegistrationDefaults(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces)]

			public interface IService<T>;

			[{|INJ0008:{|INJ0002:RegisterSingleton(UseProxyFactory = true)|}|}]
			public class Class<T> : IService<T>;
			"""
		).RunAsync(TestContext.Current.CancellationToken);
}
