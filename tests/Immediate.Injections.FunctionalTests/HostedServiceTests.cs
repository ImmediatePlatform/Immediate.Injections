using Immediate.Injections.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Immediate.Injections.FunctionalTests;

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
		using var provider = ServiceProviderFactory.Create(tags: "hosted");
		var first = provider.GetServices<IHostedService>().ToArray();
		var second = provider.GetServices<IHostedService>().ToArray();
		Assert.Equal(2, first.Length);
		Assert.Contains(first, service => service is Worker);
		Assert.Contains(first, service => service is OtherWorker);
		Assert.Same(first[0], second[0]);
		Assert.Same(first[1], second[1]);
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
