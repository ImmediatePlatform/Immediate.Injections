namespace Immediate.Injections.Tests.GeneratorTests;

public sealed class ImmediateAssemblyIdentifierTests
{
	[Fact]
	public async Task ImmediateAssemblyIdentifierOverridesAssemblyName()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using System;

			using Immediate.Handlers.Shared;
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			[assembly: ImmediateAssemblyIdentifier("Custom")]

			namespace Dummy;

			public class Class
			{
				[RegisterServices]
				public static void CallMe(IServiceCollection services)
				{
				}
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.RegisterServicesMethods.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}
}
