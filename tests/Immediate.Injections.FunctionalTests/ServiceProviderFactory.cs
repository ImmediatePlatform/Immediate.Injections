using Microsoft.Extensions.DependencyInjection;

namespace Immediate.Injections.FunctionalTests;

internal static class ServiceProviderFactory
{
	public static ServiceProvider Create(
		Action<IServiceCollection>? configure = null,
		params string[] tags)
	{
		var services = new ServiceCollection();
		configure?.Invoke(services);

		_ = services.AddImmediateInjectionsFunctionalTestsServices(tags);

		return services.BuildServiceProvider(
			new ServiceProviderOptions
			{
				ValidateOnBuild = true,
				ValidateScopes = true,
			}
		);
	}
}
