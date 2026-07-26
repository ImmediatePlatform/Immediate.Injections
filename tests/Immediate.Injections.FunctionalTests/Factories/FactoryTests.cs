using Immediate.Injections.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Immediate.Injections.FunctionalTests.Factories;

[RegisterSingleton]
public sealed class FactoryDependency;

public interface IFactoryService
{
	FactoryDependency Dependency { get; }
}

[RegisterTransient<IFactoryService>(Factory = nameof(Create))]
public sealed class FactoryService(FactoryDependency dependency) : IFactoryService
{
	public FactoryDependency Dependency { get; } = dependency;

	public static FactoryService Create(IServiceProvider provider) =>
		new(provider.GetRequiredService<FactoryDependency>());
}

public interface IKeyedFactoryService
{
	object? ServiceKey { get; }
}

[RegisterSingleton<IKeyedFactoryService>(ServiceKey = Key, Factory = nameof(Create))]
public sealed class KeyedFactoryService(object? serviceKey) : IKeyedFactoryService
{
	public const string Key = "factory-key";

	public object? ServiceKey { get; } = serviceKey;

	public static KeyedFactoryService Create(IServiceProvider _, object? serviceKey) => new(serviceKey);
}

public sealed class FactoryTests
{
	[Fact]
	public void FactoryShouldResolveDependenciesFromServiceProvider()
	{
		using var provider = ServiceProviderFactory.Create();

		var service = provider.GetRequiredService<IFactoryService>();
		var dependency = provider.GetRequiredService<FactoryDependency>();

		Assert.Same(dependency, service.Dependency);
	}

	[Fact]
	public void KeyedFactoryShouldReceiveRequestedServiceKey()
	{
		using var provider = ServiceProviderFactory.Create();

		var service = provider.GetRequiredKeyedService<IKeyedFactoryService>(KeyedFactoryService.Key);

		Assert.Equal(KeyedFactoryService.Key, service.ServiceKey);
	}
}
