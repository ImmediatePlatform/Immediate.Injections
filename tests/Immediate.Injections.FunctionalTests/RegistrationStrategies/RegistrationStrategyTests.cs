using Immediate.Injections.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Immediate.Injections.FunctionalTests.RegistrationStrategies;

public interface IFirstService
{
	string FirstValue { get; }
}

public interface ISecondService
{
	string SecondValue { get; }
}

[RegisterSingleton(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)]
public sealed class InterfacesOnlyService : IFirstService, ISecondService
{
	public string FirstValue => nameof(FirstValue);

	public string SecondValue => nameof(SecondValue);
}

public interface ISharedService
{
	Guid Id { get; }
}

[RegisterSingleton(
	RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces,
	UseProxyFactory = true
)]
public sealed class SharedService : ISharedService
{
	public Guid Id { get; } = Guid.NewGuid();
}

public interface IExplicitService
{
	string Value { get; }
}

[RegisterTransient<IExplicitService>]
public sealed class ExplicitService : IExplicitService
{
	public string Value => nameof(ExplicitService);
}

public sealed class RegistrationStrategyTests
{
	[Fact]
	public void ImplementedInterfacesShouldRegisterEachInterfaceButNotSelf()
	{
		using var provider = ServiceProviderFactory.Create();

		_ = Assert.IsType<InterfacesOnlyService>(provider.GetRequiredService<IFirstService>());
		_ = Assert.IsType<InterfacesOnlyService>(provider.GetRequiredService<ISecondService>());
		Assert.Null(provider.GetService<InterfacesOnlyService>());
	}

	[Fact]
	public void SelfAndImplementedInterfacesWithProxyShouldShareSingletonInstance()
	{
		using var provider = ServiceProviderFactory.Create();

		var implementation = provider.GetRequiredService<SharedService>();
		var service = provider.GetRequiredService<ISharedService>();

		Assert.Same(implementation, service);
	}

	[Fact]
	public void ExplicitServiceTypeShouldResolveImplementation()
	{
		using var provider = ServiceProviderFactory.Create();

		_ = Assert.IsType<ExplicitService>(provider.GetRequiredService<IExplicitService>());
		Assert.Null(provider.GetService<ExplicitService>());
	}
}
