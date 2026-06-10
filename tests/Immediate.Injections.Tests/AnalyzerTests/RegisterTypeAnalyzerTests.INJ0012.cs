using Immediate.Injections.Analyzers;

namespace Immediate.Injections.Tests.AnalyzerTests;

public sealed partial class RegisterTypeAnalyzerTests
{
	[Fact]
	public async Task FactoryMethodWithOpenGenericServiceTypeTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using System;

			using Immediate.Injections.Shared;
			
			public interface IService<T>;

			[{|INJ0012:{|INJ0002:RegisterSingleton(ServiceType = typeof(IService<>), Factory = nameof(Factory))|}|}]
			public class Class<T> : IService<T>
			{
				public static Class<T> Factory(IServiceProvider provider) => new();
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);
}
