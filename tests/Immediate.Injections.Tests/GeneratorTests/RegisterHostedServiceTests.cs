using static Immediate.Injections.Tests.Utility;

namespace Immediate.Injections.Tests.GeneratorTests;

public sealed class RegisterHostedServiceTests
{
	[Fact]
	public void UnsupportedTarget_IsNotRegistered()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Injections.Shared;

			[RegisterHostedService]
			public sealed class NotHosted;
			"""
		);
		Assert.DoesNotContain(result.GeneratedTrees, tree => tree.FilePath.EndsWith("II.RegisterHostedServices.g.cs", StringComparison.Ordinal));
	}

	[Fact]
	public async Task HostedServices_AreRegistered()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.Hosting;
			using System.Threading;
			using System.Threading.Tasks;

			[RegisterHostedService(Tags = ["background"])]
			public sealed class Worker : IHostedService
			{
				public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
				public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
			}
			"""
		);

		_ = await VerifyIgnoreCommonFile(result);
	}
}
