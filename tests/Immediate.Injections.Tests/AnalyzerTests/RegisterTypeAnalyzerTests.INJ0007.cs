using Immediate.Injections.Analyzers;

namespace Immediate.Injections.Tests.AnalyzerTests;

public sealed partial class RegisterTypeAnalyzerTests
{
	[Fact]
	public async Task TService_TImplementation_SelfUseProxyTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			
			[{|INJ0007:{|INJ0005:{|INJ0002:RegisterSingleton<Class, Class>(UseProxyFactory = true)|}|}|}]
			public class Class;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task TService_SelfUseProxyTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			
			[{|INJ0007:{|INJ0002:RegisterSingleton<Class>(UseProxyFactory = true)|}|}]
			public class Class;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task ServiceTypeSelfUseProxyTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			
			[{|INJ0007:{|INJ0002:RegisterSingleton(ServiceType = typeof(Class), UseProxyFactory = true)|}|}]
			public class Class;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task RegistrationStrategySelfUseProxyTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			
			[{|INJ0007:{|INJ0002:RegisterSingleton(RegistrationStrategy = RegistrationStrategy.Self, UseProxyFactory = true)|}|}]
			public class Class;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task AssemblyRegistrationStrategySelfUseProxyTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;

			[assembly: RegistrationDefaults(RegistrationStrategy = RegistrationStrategy.Self)]
			
			[{|INJ0007:{|INJ0002:RegisterSingleton(UseProxyFactory = true)|}|}]
			public class Class;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task SelfUseProxyTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			
			[{|INJ0007:{|INJ0002:RegisterSingleton(UseProxyFactory = true)|}|}]
			public class Class;
			"""
		).RunAsync(TestContext.Current.CancellationToken);
}
