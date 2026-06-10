using Immediate.Injections.Analyzers;

namespace Immediate.Injections.Tests.AnalyzerTests;

public sealed partial class RegisterTypeAnalyzerTests
{
	[Fact]
	public async Task TService_TImplementation_FactoryMethodInvalidSignatureTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using System;

			using Immediate.Injections.Shared;
			
			[{|INJ0010:{|INJ0005:{|INJ0002:RegisterSingleton<Class, Class>(Factory = nameof(Factory))|}|}|}]
			public class Class
			{
				public static void Factory(IServiceProvider provider) { }
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task TService_FactoryMethodInvalidSignatureTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using System;
			
			using Immediate.Injections.Shared;
			
			[{|INJ0010:{|INJ0002:RegisterSingleton<Class>(Factory = nameof(Factory))|}|}]
			public class Class
			{
				public static Class Factory() => new();
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task FactoryMethodInvalidSignatureTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using System;
			
			using Immediate.Injections.Shared;
			
			[{|INJ0010:{|INJ0002:RegisterSingleton(Factory = nameof(Factory))|}|}]
			public class Class
			{
				public static Class Factory(IServiceProvider provider, int nonKey) => new();
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);
}
