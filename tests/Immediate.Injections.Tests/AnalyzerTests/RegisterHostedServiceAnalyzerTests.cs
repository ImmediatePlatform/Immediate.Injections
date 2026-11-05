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
	[Theory]
	[InlineData(0)]
	[InlineData(-10)]
	[InlineData(int.MaxValue)]
	public async Task DuplicateOrdersAcrossNamespacesAndTags_ReportDiagnostics(int order) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterHostedServiceAnalyzer>(
			$$"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.Hosting;
			using System.Threading;
			using System.Threading.Tasks;

			namespace First
			{
				[{|INJ0019:RegisterHostedService(Order = {{order}}, Tags = ["first"])|}]
				public sealed class Worker : BackgroundService
				{
					protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
				}
			}
			namespace Second
			{
				public static class Container
				{
					[{|INJ0019:RegisterHostedService(Order = {{order}}, Tags = ["second"])|}]
					public sealed class Worker : BackgroundService
					{
						protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
					}
				}
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task UnspecifiedOrdersAndDistinctExplicitOrders_DoNotReportDiagnostics() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterHostedServiceAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.Hosting;
			using System.Threading;
			using System.Threading.Tasks;

			public abstract class WorkerBase : BackgroundService
			{
				protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
			}
			[RegisterHostedService]
			public sealed class First : WorkerBase;
			[RegisterHostedService]
			public sealed class Second : WorkerBase;
			[RegisterHostedService(Order = 0)]
			public sealed class Third : WorkerBase;
			[RegisterHostedService(Order = -1)]
			public sealed class Fourth : WorkerBase;
			[RegisterHostedService(Order = int.MaxValue)]
			public sealed class Fifth : WorkerBase;
			"""
		).RunAsync(TestContext.Current.CancellationToken);

}
