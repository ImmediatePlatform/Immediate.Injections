using Immediate.Injections.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Immediate.Injections.FunctionalTests.OpenGenerics;

public interface IRepository<T>
{
	Type ElementType { get; }
}

[RegisterTransient(ServiceType = typeof(IRepository<>))]
public sealed class Repository<T> : IRepository<T>
{
	public Type ElementType => typeof(T);
}

public sealed class OpenGenericTests
{
	[Fact]
	public void OpenGenericRegistrationShouldResolveClosedImplementations()
	{
		using var provider = ServiceProviderFactory.Create();

		_ = Assert.IsType<Repository<string>>(provider.GetRequiredService<IRepository<string>>());
		_ = Assert.IsType<Repository<int>>(provider.GetRequiredService<IRepository<int>>());
	}
}
