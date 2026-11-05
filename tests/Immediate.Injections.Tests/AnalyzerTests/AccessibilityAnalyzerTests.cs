using Immediate.Injections.Analyzers;

namespace Immediate.Injections.Tests.AnalyzerTests;

public sealed class AccessibilityAnalyzerTests
{
	[Theory]
	[MemberData(nameof(AccessibilityTestCases.RejectedLifetimes), MemberType = typeof(AccessibilityTestCases))]
	public async Task InaccessibleLifetimes_AreDiagnosed(string source) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(source)
			.RunAsync(TestContext.Current.CancellationToken);

	[Theory]
	[MemberData(nameof(AccessibilityTestCases.AcceptedLifetimes), MemberType = typeof(AccessibilityTestCases))]
	public async Task AccessibleLifetimes_AreAccepted(string source, string[] _) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterTypeAnalyzer>(source)
			.RunAsync(TestContext.Current.CancellationToken);

	[Theory]
	[MemberData(nameof(AccessibilityTestCases.RejectedOptions), MemberType = typeof(AccessibilityTestCases))]
	public async Task InaccessibleOptions_AreDiagnosed(string source) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterOptionsAnalyzer>(source)
			.RunAsync(TestContext.Current.CancellationToken);

	[Theory]
	[MemberData(nameof(AccessibilityTestCases.AcceptedOptions), MemberType = typeof(AccessibilityTestCases))]
	public async Task AccessibleOptions_AreAccepted(string source, string[] _) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterOptionsAnalyzer>(source)
			.RunAsync(TestContext.Current.CancellationToken);

	[Theory]
	[MemberData(nameof(AccessibilityTestCases.RejectedHostedServices), MemberType = typeof(AccessibilityTestCases))]
	public async Task InaccessibleHostedServices_AreDiagnosed(string source) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterHostedServiceAnalyzer>(source)
			.RunAsync(TestContext.Current.CancellationToken);

	[Theory]
	[MemberData(nameof(AccessibilityTestCases.AcceptedHostedServices), MemberType = typeof(AccessibilityTestCases))]
	public async Task AccessibleHostedServices_AreAccepted(string source, string[] _) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterHostedServiceAnalyzer>(source)
			.RunAsync(TestContext.Current.CancellationToken);

	[Theory]
	[MemberData(nameof(AccessibilityTestCases.RejectedMethods), MemberType = typeof(AccessibilityTestCases))]
	public async Task InaccessibleMethods_AreDiagnosed(string source) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterServicesMethodAnalyzer>(source)
			.RunAsync(TestContext.Current.CancellationToken);

	[Theory]
	[MemberData(nameof(AccessibilityTestCases.AcceptedMethods), MemberType = typeof(AccessibilityTestCases))]
	public async Task AccessibleMethods_AreAccepted(string source, string[] _) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterServicesMethodAnalyzer>(source)
			.RunAsync(TestContext.Current.CancellationToken);

}
