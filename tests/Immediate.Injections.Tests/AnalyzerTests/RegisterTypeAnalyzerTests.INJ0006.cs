using Immediate.Injections.Analyzers;

namespace Immediate.Injections.Tests.AnalyzerTests;

public sealed partial class RegisterTypeAnalyzerTests
{
	[Fact]
	public async Task NonGenericTImplementationOfIncorrectTypeTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService;
			
			public class Dummy : IService;
			
			[{|INJ0006:{|INJ0005:{|INJ0002:RegisterSingleton<IService, Dummy>|}|}|}]
			public class Service : IService
			{
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task GenericTImplementationOfIncorrectTypeTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public interface IService<T>;
			
			public class Dummy<T> : IService<T>;
			
			[{|INJ0006:{|INJ0002:RegisterSingleton<IService<string>, Dummy<string>>|}|}]
			public class Service<T> : IService<T>
			{
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);
}
