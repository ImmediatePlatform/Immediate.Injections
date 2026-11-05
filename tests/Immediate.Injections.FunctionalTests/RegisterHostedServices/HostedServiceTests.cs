using Immediate.Injections.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Immediate.Injections.FunctionalTests.RegisterHostedServices;

[RegisterHostedService(Tags = ["hosted"])]
public sealed class Worker : IHostedService
{
	public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

[RegisterHostedService(Tags = ["hosted"])]
public sealed class OtherWorker : BackgroundService
{
	protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
}

public sealed class HostedServiceTests
{
	[Fact]
	public async Task HostedServices_AreSingletonsAndCanStartAndStop()
	{
		await using var provider = ServiceProviderFactory.Create(tags: "hosted");

		var first = provider.GetServices<IHostedService>().ToArray();
		var second = provider.GetServices<IHostedService>().ToArray();

		Assert.Collection(
			first,
			service => Assert.IsType<Worker>(service),
			service => Assert.IsType<OtherWorker>(service)
		);

		Assert.Equal(first, second);

		foreach (var service in first)
		{
			await service.StartAsync(TestContext.Current.CancellationToken);
			await service.StopAsync(TestContext.Current.CancellationToken);
		}
	}

	[Fact]
	public void RepeatedRegistration_DoesNotDuplicateHostedServices()
	{
		var services = new ServiceCollection();
		_ = services.AddImmediateInjectionsFunctionalTestsServices("hosted");
		_ = services.AddImmediateInjectionsFunctionalTestsServices("hosted");
		Assert.Equal(2, services.Count(s => s.ServiceType == typeof(IHostedService)));
	}

	[Fact]
	public void OtherTags_ExcludeHostedServices()
	{
		using var provider = ServiceProviderFactory.Create(tags: "other");
		Assert.Empty(provider.GetServices<IHostedService>());
	}
}
