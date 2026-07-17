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

		public bool IsIServiceProvider =>
			typeSymbol is INamedTypeSymbol
			{
				Arity: 0,
				Name: "IServiceProvider",
				ContainingNamespace:
				{
					Name: "System",
					ContainingNamespace.IsGlobalNamespace: true,
				},
			};

		public bool IsReadOnlySpanString =>
			typeSymbol is INamedTypeSymbol
			{
				IsReadOnlySpan: true,
				TypeArguments: [{ SpecialType: SpecialType.System_String }],
			};

		public bool IsIDisposable =>
			typeSymbol is INamedTypeSymbol
			{
				Arity: 0,
				Name: "IDisposable" or "IAsyncDisposable",
				ContainingNamespace:
				{
					Name: "System",
					ContainingNamespace.IsGlobalNamespace: true,
				},
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

		public bool IsImmediateAssemblyIdentifierAttribute =>
			typeSymbol is INamedTypeSymbol
			{
				Arity: 0,
				Name: "ImmediateAssemblyIdentifierAttribute",
				ContainingNamespace:
				{
					Name: "Shared",
					ContainingNamespace:
					{
						Name: "Handlers",
						ContainingNamespace:
						{
							Name: "Immediate",
							ContainingNamespace.IsGlobalNamespace: true,
						},
					},
				},
			};

		public bool IsRegisterServicesAttribute =>
			typeSymbol is INamedTypeSymbol
			{
				Arity: 0,
				Name: "RegisterServicesAttribute",
				ContainingNamespace.IsImmediateInjectionsShared: true,
			};

		public bool IsRegistrationDefaultsAttribute =>
			typeSymbol is INamedTypeSymbol
			{
				Arity: 0,
				Name: "RegistrationDefaultsAttribute",
				ContainingNamespace.IsImmediateInjectionsShared: true,
			};

		public bool IsRegisterTypeAttribute =>
			typeSymbol is INamedTypeSymbol
			{
				Arity: 0 or 1 or 2,
				Name: "RegisterScopedAttribute" or "RegisterSingletonAttribute" or "RegisterTransientAttribute",
				ContainingNamespace.IsImmediateInjectionsShared: true,
			};

		public bool IsRegisterType0Attribute =>
			typeSymbol is INamedTypeSymbol
			{
				Arity: 0,
				Name: "RegisterScopedAttribute" or "RegisterSingletonAttribute" or "RegisterTransientAttribute",
				ContainingNamespace.IsImmediateInjectionsShared: true,
			};

		public bool IsRegisterType1Attribute =>
			typeSymbol is INamedTypeSymbol
			{
				Arity: 1,
				Name: "RegisterScopedAttribute" or "RegisterSingletonAttribute" or "RegisterTransientAttribute",
				ContainingNamespace.IsImmediateInjectionsShared: true,
			};

		public bool IsRegisterType2Attribute =>
			typeSymbol is INamedTypeSymbol
			{
				Arity: 2,
				Name: "RegisterScopedAttribute" or "RegisterSingletonAttribute" or "RegisterTransientAttribute",
				ContainingNamespace.IsImmediateInjectionsShared: true,
			};
	}

	extension(INamedTypeSymbol typeSymbol)
	{
		public bool IsValidFactoryMethod(string? factory, bool isKeyed)
		{
			if (factory is null)
				return true;

			return typeSymbol.GetMembers()
				.Where(m => m is { IsStatic: true, Kind: SymbolKind.Method })
				.Where(m => string.Equals(m.Name, factory, StringComparison.Ordinal))
				.Cast<IMethodSymbol>()
				.Where(ims => SymbolEqualityComparer.Default.Equals(ims.ReturnType, typeSymbol))
				.Any(
					ims => isKeyed
						? ims is { Parameters: [{ Type.IsIServiceProvider: true }, { Type.SpecialType: SpecialType.System_Object }] }
						: ims is { Parameters: [{ Type.IsIServiceProvider: true }] }
				);
		}
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
