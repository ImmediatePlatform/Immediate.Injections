using Immediate.Injections.Analyzers;

namespace Immediate.Injections.Tests.AnalyzerTests;

public sealed partial class RegisterTypeAnalyzerTests
{
	[Fact]
	public async Task UsingBothServiceTypeAndRegistrationStrategyTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			
			[{|INJ0003:{|INJ0002:RegisterSingleton(ServiceType = typeof(Class), RegistrationStrategy = RegistrationStrategy.Self)|}|}]
			public class Class;
			"""
		).RunAsync(TestContext.Current.CancellationToken);
}
