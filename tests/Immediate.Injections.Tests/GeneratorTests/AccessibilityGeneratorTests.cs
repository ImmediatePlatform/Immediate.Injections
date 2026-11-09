using System.Text.RegularExpressions;
using static Immediate.Injections.Tests.Utility;

namespace Immediate.Injections.Tests.GeneratorTests;

public sealed partial class AccessibilityGeneratorTests
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
	public void AccessibleRegistrations_AreEmitted(string source)
	{
		var result = GeneratorTestHelper.RunGenerator(source);

		Assert.Collection(
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/')),
			s => Assert.Equal("Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs", s),
			s => Assert.Matches(GeneratedServiceSourcePath(), s),
			s => Assert.Equal("Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs", s)
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
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System;
			using Immediate.Injections.Shared;

			public interface IService{{typeParameters}};
			public class Container
			{
				private interface IHidden{{typeParameters}};

				[RegisterScoped(RegistrationStrategy = RegistrationStrategy.{{strategy}})]
				public sealed class Service{{typeParameters}} : IService{{typeParameters}}, IHidden{{typeParameters}}, IDisposable
				{
					public void Dispose() { }
				}
			}
			"""
		);

		Assert.Equal(
			[
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.ServiceCollectionExtensions.g.cs",
				"Immediate.Injections.Generators/Immediate.Injections.Generators.ImmediateInjectionsGenerator/II.RegisterScoped`0.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreCommonFile(result).UseParameters(strategy, generic);
	}

	[GeneratedRegex(
		@"^Immediate\.Injections\.Generators/Immediate\.Injections\.Generators\.ImmediateInjectionsGenerator/II\.Register(Options|HostedServices|ServicesMethods|Scoped`\d)\.g\.cs$",
		RegexOptions.ExplicitCapture
	)]
	private static partial Regex GeneratedServiceSourcePath();
}
