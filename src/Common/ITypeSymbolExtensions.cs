using System.Diagnostics.CodeAnalysis;
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

	extension([NotNullWhen(true)] ITypeSymbol? typeSymbol)
	{
		public bool IsIServiceCollection =>
			typeSymbol is INamedTypeSymbol
			{
				Arity: 0,
				Name: "IServiceCollection",
				ContainingNamespace.IsDependencyInjection: true,
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

		public bool IsRegisterServicesAttribute =>
			typeSymbol is INamedTypeSymbol
			{
				Arity: 0,
				Name: "RegisterServicesAttribute",
				ContainingNamespace.IsImmediateInjectionsShared: true,
			};
	}

	extension(INamespaceSymbol namespaceSymbol)
	{
		public bool IsDependencyInjection =>
			namespaceSymbol is
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
			};

		public bool IsImmediateInjectionsShared =>
			namespaceSymbol is
			{
				Name: "Shared",
				ContainingNamespace:
				{
					Name: "Injections",
					ContainingNamespace:
					{
						Name: "Immediate",
						ContainingNamespace.IsGlobalNamespace: true,
					},
				},
			};
	}
}
