using Immediate.Injections.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Immediate.Injections.FunctionalTests.Tags;

public interface IAlphaService
{
	string Value { get; }
}

[RegisterSingleton<IAlphaService>(Tags = ["alpha"])]
public sealed class AlphaService : IAlphaService
{
	public string Value => nameof(AlphaService);
}

public interface IBetaService
{
	string Value { get; }
}

[RegisterSingleton<IBetaService>(Tags = ["beta"])]
public sealed class BetaService : IBetaService
{
	public string Value => nameof(BetaService);
}

public interface IUntaggedService
{
	string Value { get; }
}

[RegisterSingleton<IUntaggedService>]
public sealed class UntaggedService : IUntaggedService
{
	public string Value => nameof(UntaggedService);
}

public sealed class TagTests
{
	[Fact]
	public void NoTagsShouldRegisterAllServices()
	{
		using var provider = ServiceProviderFactory.Create();

		Assert.NotNull(provider.GetService<IAlphaService>());
		Assert.NotNull(provider.GetService<IBetaService>());
		Assert.NotNull(provider.GetService<IUntaggedService>());
	}

	[Fact]
	public void TagsShouldRegisterMatchingAndUntaggedServicesOnly()
	{
		using var provider = ServiceProviderFactory.Create(configure: null, tags: ["alpha"]);

		Assert.NotNull(provider.GetService<IAlphaService>());
		Assert.Null(provider.GetService<IBetaService>());
		Assert.NotNull(provider.GetService<IUntaggedService>());
	}

	[Fact]
	public void UnknownTagShouldRegisterUntaggedServicesOnly()
	{
		using var provider = ServiceProviderFactory.Create(configure: null, tags: ["unknown"]);

		Assert.Null(provider.GetService<IAlphaService>());
		Assert.Null(provider.GetService<IBetaService>());
		Assert.NotNull(provider.GetService<IUntaggedService>());
	}
}
