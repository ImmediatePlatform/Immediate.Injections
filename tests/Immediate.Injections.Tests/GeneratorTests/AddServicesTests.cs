namespace Immediate.Injections.Tests.GeneratorTests;

public sealed class AddServicesTests
{
	[Theory]
	[MemberData(nameof(Frameworks))]
	public async Task ValidAddServicesMethod(string framework)
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result)
			.UseParameters(framework);
	}

	public static TheoryData<string> Frameworks =>
		[Utility.ReferenceAssemblies.TargetFramework];
}
