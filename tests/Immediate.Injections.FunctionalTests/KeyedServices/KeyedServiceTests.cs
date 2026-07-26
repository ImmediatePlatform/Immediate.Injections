using Immediate.Injections.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Immediate.Injections.FunctionalTests.KeyedServices;

public interface IKeyedService
{
	string Value { get; }
}

[RegisterSingleton<IKeyedService>(ServiceKey = Key)]
public sealed class KeyedService : IKeyedService
{
	public const string Key = "keyed-service";

	public string Value => nameof(KeyedService);
}

public sealed class KeyedServiceTests
{
	[Fact]
	public void KeyedRegistrationShouldResolveOnlyForMatchingKey()
	{
		using var provider = ServiceProviderFactory.Create();

		_ = Assert.IsType<KeyedService>(provider.GetRequiredKeyedService<IKeyedService>(KeyedService.Key));
		Assert.Null(provider.GetKeyedService<IKeyedService>("other-key"));
		Assert.Null(provider.GetService<IKeyedService>());
	}
}
