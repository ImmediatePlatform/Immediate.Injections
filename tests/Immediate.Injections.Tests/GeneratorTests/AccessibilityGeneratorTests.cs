using static Immediate.Injections.Tests.Utility;

namespace Immediate.Injections.Tests.GeneratorTests;

public sealed class AccessibilityGeneratorTests
{
	[Theory]
	[MemberData(nameof(AccessibilityTestCases.RejectedLifetimes), MemberType = typeof(AccessibilityTestCases))]
	[MemberData(nameof(AccessibilityTestCases.RejectedOptions), MemberType = typeof(AccessibilityTestCases))]
	[MemberData(nameof(AccessibilityTestCases.RejectedHostedServices), MemberType = typeof(AccessibilityTestCases))]
	[MemberData(nameof(AccessibilityTestCases.RejectedMethods), MemberType = typeof(AccessibilityTestCases))]
	public void InaccessibleRegistrations_AreNotEmitted(string source)
	{
		var result = GeneratorTestHelper.RunGenerator(AccessibilityTestCases.WithoutMarkup(source));
		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);
	}

	[Theory]
	[MemberData(nameof(AccessibilityTestCases.AcceptedLifetimes), MemberType = typeof(AccessibilityTestCases))]
	[MemberData(nameof(AccessibilityTestCases.AcceptedOptions), MemberType = typeof(AccessibilityTestCases))]
	[MemberData(nameof(AccessibilityTestCases.AcceptedHostedServices), MemberType = typeof(AccessibilityTestCases))]
	[MemberData(nameof(AccessibilityTestCases.AcceptedMethods), MemberType = typeof(AccessibilityTestCases))]
	public void AccessibleRegistrations_AreEmitted(string source, string[] expectedHints)
	{
		ArgumentNullException.ThrowIfNull(expectedHints);
		var result = GeneratorTestHelper.RunGenerator(source);
		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				..expectedHints.Select(hint => $"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/{hint}"),
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);
	}

	[Theory]
	[InlineData("ImplementedInterfaces", false)]
	[InlineData("SelfAndImplementedInterfaces", false)]
	[InlineData("ImplementedInterfaces", true)]
	[InlineData("SelfAndImplementedInterfaces", true)]
	public async Task ImplicitInterfaces_OnlyAccessibleNonDisposableInterfacesAreRegistered(string strategy, bool generic)
	{
		var typeParameters = generic ? "<T>" : "";
		var source = $$"""
			using System;
			using Immediate.Injections.Shared;

			public interface IService{{typeParameters}};
			public class Container
			{
				private interface IHidden{{typeParameters}};

				[RegisterScoped(RegistrationStrategy = RegistrationStrategy.{{strategy}})]
				[RegisterSingleton(RegistrationStrategy = RegistrationStrategy.{{strategy}})]
				[RegisterTransient(RegistrationStrategy = RegistrationStrategy.{{strategy}})]
				public sealed class Service{{typeParameters}} : IService{{typeParameters}}, IHidden{{typeParameters}}, IDisposable
				{
					public void Dispose() { }
				}
			}
			""";
		var result = GeneratorTestHelper.RunGenerator(source);
		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.RegisterScoped`0.g.cs",
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.RegisterSingleton`0.g.cs",
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.RegisterTransient`0.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);
		_ = await VerifyIgnoreCommonFile(result).UseParameters(strategy, generic);
	}

	[Fact]
	public async Task MultipleClosedGenericRegistrations_AreIndependent()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Injections.Shared;

			public interface IService<T>;
			[RegisterScoped<IService<int>>]
			[RegisterScoped<IService<string>>]
			[RegisterSingleton<IService<int>>]
			[RegisterSingleton<IService<string>>]
			[RegisterTransient<IService<int>>]
			[RegisterTransient<IService<string>>]
			public sealed class Service<T> : IService<T>;
			"""
		);
		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.RegisterScoped`1.g.cs",
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.RegisterSingleton`1.g.cs",
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.RegisterTransient`1.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);
		_ = await VerifyIgnoreCommonFile(result);
	}
}
