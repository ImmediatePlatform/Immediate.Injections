using Immediate.Injections.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Immediate.Injections.FunctionalTests;

public abstract class OrderedWorkerBase : BackgroundService
{
	protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
}

[RegisterHostedService(Order = int.MaxValue, Tags = ["order"])]
public sealed class LastOrderedWorker : OrderedWorkerBase;

[RegisterHostedService(Tags = ["order"])]
public sealed class UnorderedWorker : OrderedWorkerBase;

[RegisterHostedService(Order = 0, Tags = ["order"])]
public sealed class MiddleOrderedWorker : OrderedWorkerBase;

[RegisterHostedService(Order = int.MinValue, Tags = ["order"])]
public sealed class FirstOrderedWorker : OrderedWorkerBase;

public sealed class HostedServiceOrderingTests
{
	[Fact]
	public void Services_AreRegisteredInOrderWithUnorderedServicesFirst()
	{
		using var provider = ServiceProviderFactory.Create(tags: "order");
		Assert.Equal(
			[typeof(UnorderedWorker), typeof(FirstOrderedWorker), typeof(MiddleOrderedWorker), typeof(LastOrderedWorker)],
			provider.GetServices<IHostedService>().Select(service => service.GetType())
		);
	}
}
