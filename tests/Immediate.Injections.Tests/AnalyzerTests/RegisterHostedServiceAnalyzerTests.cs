using Immediate.Injections.Analyzers;

namespace Immediate.Injections.Tests.AnalyzerTests;

public sealed class RegisterHostedServiceAnalyzerTests
{
	[Fact]
	public async Task ValidHostedService_DoesNotReportDiagnostic() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterHostedServiceAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.Hosting;
			using System.Threading;
			using System.Threading.Tasks;

			[RegisterHostedService]
			public sealed class Worker : IHostedService
			{
				public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
				public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Theory]
	[InlineData("public sealed class Worker")]
	[InlineData("public abstract class Worker")]
	[InlineData("public sealed class Worker<T>")]
	public async Task UnsupportedTarget_ReportsDiagnostic(string declaration) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterHostedServiceAnalyzer>(
			$$"""
			using Immediate.Injections.Shared;

			[{|INJ0018:RegisterHostedService|}]
			{{declaration}};
			"""
		).RunAsync(TestContext.Current.CancellationToken);
}
