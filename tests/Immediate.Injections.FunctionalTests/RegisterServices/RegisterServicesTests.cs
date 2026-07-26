using Immediate.Injections.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Immediate.Injections.FunctionalTests.RegisterServices;

public sealed class ManuallyRegisteredService;

public sealed record ReceivedTags(IReadOnlyList<string> Values);

public static class ManualRegistrations
{
	[RegisterServices]
	public static void Register(IServiceCollection services)
	{
		_ = services.AddSingleton<ManuallyRegisteredService>();
	}

	[RegisterServices]
	public static void RegisterWithTags(IServiceCollection services, ReadOnlySpan<string> tags)
	{
		_ = services.AddSingleton(new ReceivedTags(tags.ToArray()));
	}
}

public sealed class RegisterServicesTests
{
	[Fact]
	public void RegisterServicesMethodShouldAddManualRegistration()
	{
		using var provider = ServiceProviderFactory.Create();

		Assert.NotNull(provider.GetService<ManuallyRegisteredService>());
	}

	[Fact]
	public void RegisterServicesMethodShouldReceiveTags()
	{
		using var provider = ServiceProviderFactory.Create(configure: null, tags: ["alpha", "beta"]);

		Assert.Equal(["alpha", "beta"], provider.GetRequiredService<ReceivedTags>().Values);
	}
}
