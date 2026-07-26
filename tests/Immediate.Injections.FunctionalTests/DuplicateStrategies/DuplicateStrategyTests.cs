using Immediate.Injections.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Immediate.Injections.FunctionalTests.DuplicateStrategies;

public interface IAppendService
{
	string Source { get; }
}

public sealed class ExistingAppendService : IAppendService
{
	public string Source => nameof(ExistingAppendService);
}

[RegisterSingleton<IAppendService>(DuplicateStrategy = DuplicateStrategy.Append)]
public sealed class AppendedService : IAppendService
{
	public string Source => nameof(AppendedService);
}

public interface ISkipService
{
	string Source { get; }
}

public sealed class ExistingSkipService : ISkipService
{
	public string Source => nameof(ExistingSkipService);
}

[RegisterSingleton<ISkipService>(DuplicateStrategy = DuplicateStrategy.Skip)]
public sealed class SkippedService : ISkipService
{
	public string Source => nameof(SkippedService);
}

public interface IReplaceService
{
	string Source { get; }
}

public sealed class ExistingReplaceService : IReplaceService
{
	public string Source => nameof(ExistingReplaceService);
}

[RegisterSingleton<IReplaceService>(DuplicateStrategy = DuplicateStrategy.Replace)]
public sealed class ReplacementService : IReplaceService
{
	public string Source => nameof(ReplacementService);
}

public sealed class DuplicateStrategyTests
{
	[Fact]
	public void AppendShouldPreserveExistingRegistrationAndAddGeneratedRegistration()
	{
		using var provider = ServiceProviderFactory.Create(
			services => services.AddSingleton<IAppendService, ExistingAppendService>()
		);

		Assert.Collection(
			provider.GetServices<IAppendService>(),
			service => Assert.IsType<ExistingAppendService>(service),
			service => Assert.IsType<AppendedService>(service)
		);
	}

	[Fact]
	public void SkipShouldPreserveExistingRegistrationOnly()
	{
		using var provider = ServiceProviderFactory.Create(
			services => services.AddSingleton<ISkipService, ExistingSkipService>()
		);

		Assert.Collection(
			provider.GetServices<ISkipService>(),
			service => Assert.IsType<ExistingSkipService>(service)
		);
	}

	[Fact]
	public void ReplaceShouldRemoveExistingRegistrationAndAddGeneratedRegistration()
	{
		using var provider = ServiceProviderFactory.Create(
			services => services.AddSingleton<IReplaceService, ExistingReplaceService>()
		);

		Assert.Collection(
			provider.GetServices<IReplaceService>(),
			service => Assert.IsType<ReplacementService>(service)
		);
	}
}
