using Immediate.Injections.Analyzers;

namespace Immediate.Injections.Tests.AnalyzerTests;

public sealed partial class RegisterTypeAnalyzerTests
{
	[Fact]
	public async Task ApplyingTServiceTImplementationForNonGenericTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			
			[{|INJ0005:RegisterSingleton<Class, Class>|}]
			public class Class;
			"""
		).RunAsync(TestContext.Current.CancellationToken);
}
