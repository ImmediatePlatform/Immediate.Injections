using Immediate.Injections.Analyzers;

namespace Immediate.Injections.Tests.AnalyzerTests;

public sealed partial class RegisterTypeAnalyzerTests
{
	[Fact]
	public async Task TService_TImplementation_NonExistentFactoryMethodTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			
			[{|INJ0009:{|INJ0005:{|INJ0002:RegisterSingleton<Class, Class>(Factory = "NonExistent")|}|}|}]
			public class Class;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task TService_NonExistentFactoryMethodTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			
			[{|INJ0009:{|INJ0002:RegisterSingleton<Class>(Factory = "NonExistent")|}|}]
			public class Class;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task NonExistentFactoryMethodTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			
			[{|INJ0009:{|INJ0002:RegisterSingleton(Factory = "NonExistent")|}|}]
			public class Class;
			"""
		).RunAsync(TestContext.Current.CancellationToken);
}
