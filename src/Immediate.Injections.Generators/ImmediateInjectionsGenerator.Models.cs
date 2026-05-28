using Microsoft.CodeAnalysis.CSharp;

namespace Immediate.Injections.Generators;

public sealed partial class ImmediateInjectionsGenerator
{
	private sealed record AssemblyRegistrationDefaults
	{
		public required string? DuplicateStrategy { get; init; }
	}

	private sealed record AssemblyDefaults
	{
		public required string AssemblyName { get; init; }
		public required LanguageVersion LanguageVersion { get; init; }
		public required string RootNamespace { get; init; }

		public required string DuplicateStrategy { get; init; }
	}

	private sealed record RegisterServicesMethod
	{
		public required string FullName { get; init; }
		public required bool ReceivesTags { get; init; }
	}

	private sealed record RegisterClass
	{
		public required string ServiceType { get; init; }
		public required string Implementation { get; init; }
		public required string? Tags { get; init; }
		public required string? ServiceKey { get; init; }
		public required string? DuplicateStrategy { get; init; }
	}
}
