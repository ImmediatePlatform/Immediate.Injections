using Immediate.Injections.Analyzers;

namespace Immediate.Injections.Tests.AnalyzerTests;

public sealed partial class RegisterTypeAnalyzerTests
{
	[Fact]
	public async Task TService_TImplementation_FactoryMethodWithUseProxyTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using System;

			using Immediate.Injections.Shared;
			
			[{|INJ0011:{|INJ0007:{|INJ0005:{|INJ0002:RegisterSingleton<Class, Class>(Factory = nameof(Factory), UseProxyFactory = true)|}|}|}|}]
			public class Class
			{
				public static Class Factory(IServiceProvider provider) => new();
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task TService_FactoryMethodWithUseProxyTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using System;
			
			using Immediate.Injections.Shared;
			
			[{|INJ0011:{|INJ0007:{|INJ0002:RegisterSingleton<Class>(Factory = nameof(Factory), UseProxyFactory = true)|}|}|}]
			public class Class
			{
				public static Class Factory(IServiceProvider provider) => new();
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task FactoryMethodWithUseProxyTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using System;
			
			using Immediate.Injections.Shared;
			
			[{|INJ0011:{|INJ0007:{|INJ0002:RegisterSingleton(Factory = nameof(Factory), UseProxyFactory = true)|}|}|}]
			public class Class
			{
				public static Class Factory(IServiceProvider provider) => new();
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task FactoryMethodWithUseProxyForSelfAndImplementedInterfacesDoesNotTrigger1() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using System;
			
			using Immediate.Injections.Shared;
			
			[RegisterSingleton(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces, Factory = nameof(Factory), UseProxyFactory = true)]
			public class Class
			{
				public static Class Factory(IServiceProvider provider) => new();
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task FactoryMethodWithUseProxyForSelfAndImplementedInterfacesDoesNotTrigger2() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using System;
			
			using Immediate.Injections.Shared;

			[assembly: RegistrationDefaults(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces)]
			
			[RegisterSingleton(Factory = nameof(Factory), UseProxyFactory = true)]
			public class Class
			{
				public static Class Factory(IServiceProvider provider) => new();
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task FactoryMethodWithUseProxyForSelfAndImplementedInterfacesDoesNotTrigger3() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using System;
			
			using Immediate.Injections.Shared;

			[assembly: RegistrationDefaults(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces, UseProxyFactory = true)]
			
			[RegisterSingleton(Factory = nameof(Factory))]
			public class Class
			{
				public static Class Factory(IServiceProvider provider) => new();
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);
}
