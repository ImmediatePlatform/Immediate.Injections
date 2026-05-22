using Microsoft.CodeAnalysis.CSharp;

namespace Immediate.Injections.Generators;

public sealed partial class ImmediateInjectionsGenerator
{
	private sealed record AssemblyRegistrationDefaults
	{
		public required string? DuplicateStrategy { get; init; }
		public required string? RegistrationStrategy { get; init; }
	}

	private sealed record AssemblyDefaults
	{
		public required string AssemblyName { get; init; }
		public required LanguageVersion LanguageVersion { get; init; }
		public required string RootNamespace { get; init; }

		public required string DuplicateStrategy { get; init; }
		public required string RegistrationStrategy { get; init; }
	}
}
