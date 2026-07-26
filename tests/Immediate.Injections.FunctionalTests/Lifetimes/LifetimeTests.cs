using Immediate.Injections.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Immediate.Injections.FunctionalTests.Lifetimes;

[RegisterSingleton]
public sealed class SingletonService;

[RegisterScoped]
public sealed class ScopedService;

[RegisterTransient]
public sealed class TransientService;

public sealed class LifetimeTests
{
	[Fact]
	public void SingletonShouldReturnSameInstanceAcrossScopes()
	{
		using var provider = ServiceProviderFactory.Create();
		using var scope = provider.CreateScope();

		var first = provider.GetRequiredService<SingletonService>();
		var second = scope.ServiceProvider.GetRequiredService<SingletonService>();

		Assert.Same(first, second);
	}

	[Fact]
	public void ScopedShouldReturnSameInstanceWithinScopeAndDifferentInstancesAcrossScopes()
	{
		using var provider = ServiceProviderFactory.Create();
		using var firstScope = provider.CreateScope();
		using var secondScope = provider.CreateScope();

		var first = firstScope.ServiceProvider.GetRequiredService<ScopedService>();
		var firstAgain = firstScope.ServiceProvider.GetRequiredService<ScopedService>();
		var second = secondScope.ServiceProvider.GetRequiredService<ScopedService>();

		Assert.Same(first, firstAgain);
		Assert.NotSame(first, second);
	}

	[Fact]
	public void TransientShouldReturnNewInstanceForEveryResolution()
	{
		using var provider = ServiceProviderFactory.Create();

		var first = provider.GetRequiredService<TransientService>();
		var second = provider.GetRequiredService<TransientService>();

		Assert.NotSame(first, second);
	}
}
