using Microsoft.CodeAnalysis;

namespace Immediate.Injections;

internal static class ITypeSymbolExtensions
{
	extension(IMethodSymbol methodSymbol)
	{
		public bool IsValidRegisterServicesMethod =>
			methodSymbol is
			{
				IsStatic: true,
				IsAsync: false,
				ReturnsVoid: true,
				Parameters: [{ Type.IsIServiceCollection: true }] or [{ Type.IsIServiceCollection: true }, { Type.IsReadOnlySpanString: true }],
			};
	}

	extension(ITypeSymbol typeSymbol)
	{
		public bool IsIServiceCollection =>
			typeSymbol is INamedTypeSymbol
			{
				Arity: 0,
				Name: "IServiceCollection",
				ContainingNamespace:
				{
					Name: "DependencyInjection",
					ContainingNamespace:
					{
						Name: "Extensions",
						ContainingNamespace:
						{
							Name: "Microsoft",
							ContainingNamespace.IsGlobalNamespace: true,
						},
					},
				},
			};

		public bool IsReadOnlySpanString =>
			typeSymbol is INamedTypeSymbol
			{
				IsReadOnlySpan: true,
				TypeArguments: [{ SpecialType: SpecialType.System_String }],
			};

		public bool IsReadOnlySpan =>
			typeSymbol is INamedTypeSymbol
			{
				Arity: 1,
				Name: "ReadOnlySpan",
				ContainingNamespace:
				{
					Name: "System",
					ContainingNamespace.IsGlobalNamespace: true,
				},
			};
	}
}
