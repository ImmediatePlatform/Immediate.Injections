using Microsoft.CodeAnalysis;

namespace Immediate.Injections;

internal static class RegistrationAccessibility
{
	public static bool IsTypeAccessable(ITypeSymbol? typeSymbol)
	{
		return typeSymbol switch
		{
			IArrayTypeSymbol { ElementType: var elementType } => IsTypeAccessable(elementType),

			INamedTypeSymbol namedTypeSymbol => IsTypeAccessable(namedTypeSymbol),

			null => false,

			_ => true,
		};
	}

	public static bool IsTypeAccessable(INamedTypeSymbol namedTypeSymbol)
	{
		if (namedTypeSymbol.IsFileLocal)
			return false;

		if (namedTypeSymbol.DeclaredAccessibility is not (Accessibility.Public or Accessibility.Internal or Accessibility.ProtectedOrInternal))
			return false;

		if (namedTypeSymbol.ContainingType is { } container
			&& !IsTypeAccessable(container))
		{
			return false;
		}

		// An unbound generic names only the definition, not its placeholder arguments.
		if (namedTypeSymbol.IsUnboundGenericType)
			return true;

		foreach (var argument in namedTypeSymbol.TypeArguments)
		{
			if (!IsTypeAccessable(argument))
				return false;
		}

		return true;
	}
}
