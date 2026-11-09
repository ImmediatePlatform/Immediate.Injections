using Microsoft.CodeAnalysis;

namespace Immediate.Injections;

internal static class RegistrationAccessibility
{
	public static bool IsTypeAccessable(IAssemblySymbol assembly, ITypeSymbol? typeSymbol)
	{
		return typeSymbol switch
		{
			IArrayTypeSymbol { ElementType: var elementType } => IsTypeAccessable(assembly, elementType),

			INamedTypeSymbol namedTypeSymbol => IsTypeAccessable(assembly, namedTypeSymbol),

			null => false,

			_ => true,
		};
	}

	public static bool IsTypeAccessable(IAssemblySymbol assembly, INamedTypeSymbol namedTypeSymbol)
	{
		if (namedTypeSymbol.IsFileLocal)
			return false;

		if (!(
				namedTypeSymbol.DeclaredAccessibility is Accessibility.Public
				|| (
					namedTypeSymbol.DeclaredAccessibility is Accessibility.Internal or Accessibility.ProtectedOrInternal
					&& SymbolEqualityComparer.Default.Equals(namedTypeSymbol.ContainingAssembly, assembly)
				)
		))
		{
			return false;
		}

		if (namedTypeSymbol.ContainingType is { } container
			&& !IsTypeAccessable(assembly, container))
		{
			return false;
		}

		// An unbound generic names only the definition, not its placeholder arguments.
		if (namedTypeSymbol.IsUnboundGenericType)
			return true;

		foreach (var argument in namedTypeSymbol.TypeArguments)
		{
			if (!IsTypeAccessable(assembly, argument))
				return false;
		}

		return true;
	}
}
