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
		Assert.Equal("II.ServiceCollectionExtensions.g.cs", Assert.Single(result.Results[0].GeneratedSources).HintName);
	}

	[Theory]
	[MemberData(nameof(AccessibilityTestCases.AcceptedLifetimes), MemberType = typeof(AccessibilityTestCases))]
	[MemberData(nameof(AccessibilityTestCases.AcceptedOptions), MemberType = typeof(AccessibilityTestCases))]
	[MemberData(nameof(AccessibilityTestCases.AcceptedHostedServices), MemberType = typeof(AccessibilityTestCases))]
	[MemberData(nameof(AccessibilityTestCases.AcceptedMethods), MemberType = typeof(AccessibilityTestCases))]
	public void AccessibleRegistrations_AreEmitted(string source)
	{
		var result = GeneratorTestHelper.RunGenerator(source);
		_ = Assert.Single(result.Results[0].GeneratedSources, s => !s.HintName.Equals("II.ServiceCollectionExtensions.g.cs", StringComparison.Ordinal));
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
			public sealed class Service<T> : IService<T>;
			"""
		);
		_ = await VerifyIgnoreCommonFile(result);
	}
}
