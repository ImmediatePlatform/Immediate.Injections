using Microsoft.CodeAnalysis;

namespace Immediate.Injections.Generators;

internal static class DisplayNameFormatters
{
	public static readonly SymbolDisplayFormat MethodFullyQualifiedWithType =
		SymbolDisplayFormat.FullyQualifiedFormat
			.WithMemberOptions(SymbolDisplayMemberOptions.IncludeContainingType);

}
