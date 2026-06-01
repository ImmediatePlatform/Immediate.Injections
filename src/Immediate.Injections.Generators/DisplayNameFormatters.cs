using Microsoft.CodeAnalysis;

namespace Immediate.Injections.Generators;

internal static class DisplayNameFormatters
{
	public static readonly SymbolDisplayFormat MethodFullyQualifiedWithType =
		SymbolDisplayFormat.FullyQualifiedFormat
			.WithMemberOptions(SymbolDisplayMemberOptions.IncludeContainingType);

	public static readonly SymbolDisplayFormat FullyQualifiedWithNullableFormat =
		SymbolDisplayFormat.FullyQualifiedFormat
			.WithMiscellaneousOptions(
				SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions
				| SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
			);

}
