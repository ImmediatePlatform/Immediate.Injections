using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Immediate.Injections.Generators;

public sealed partial class ImmediateInjectionsGenerator
{
	private static AssemblyRegistrationDefaults TransformAssemblyDefaults(
		GeneratorAttributeSyntaxContext context,
		CancellationToken token
	)
	{
		token.ThrowIfCancellationRequested();

		var arguments = context.Attributes[0].NamedArguments;

		return new()
		{
			DuplicateStrategy = arguments.GetEnumArgumentValue("DuplicateStrategy"),
		};
	}

	private static RegisterServicesMethod? TransformRegisterServicesMethod(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		token.ThrowIfCancellationRequested();

		return context.TargetSymbol switch
		{
			IMethodSymbol { IsValidRegisterServicesMethod: true } ims =>
				new()
				{
					FullName = ims.ToDisplayString(DisplayNameFormatters.MethodFullyQualifiedWithType),
					ReceivesTags = ims.Parameters.Length == 2,
				},

			_ => null,
		};
	}

	private static EquatableReadOnlyList<RegisterClass> TransformRegisterClass0(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		token.ThrowIfCancellationRequested();

		if (context.TargetSymbol is not INamedTypeSymbol targetSymbol)
			return new([]);

		var assemblyAttributes = context.SemanticModel.Compilation.Assembly.GetAttributes();
		var defaultsAttribute = assemblyAttributes.FirstOrDefault(a => a.AttributeClass.IsRegistrationDefaultsAttribute);
		var defaultRegistration = defaultsAttribute?.NamedArguments.GetEnumArgumentValue("RegistrationStrategy") ?? "None";

		return context.Attributes
			.SelectMany(attributeData =>
			{
				token.ThrowIfCancellationRequested();

				var arguments = attributeData.NamedArguments;
				var serviceType = arguments.GetArgumentValue("ServiceType")?.GetArgumentType();
				var tags = arguments.GetArgumentValue("Tags")?.GetStringArray();
				var serviceKey = arguments.GetArgumentValue("ServiceKey")?.ToCSharpString().NullIf("null");
				var factory = arguments.GetArgumentValue("Factory")?.Value as string;
				var duplicateStrategy = arguments.GetEnumArgumentValue("DuplicateStrategy");
				var registrationStrategy = arguments.GetEnumArgumentValue("RegistrationStrategy") ?? (serviceType != null ? "None" : defaultRegistration);
				var useProxy = arguments.GetArgumentValue("UseProxyFactory")?.Value is true;

				if (targetSymbol.IsGenericType
					&& (factory is { } || useProxy))
				{
					return [];
				}

				if (!targetSymbol.IsValidFactoryMethod(factory, isKeyed: serviceKey is { }))
					return [];

				if (registrationStrategy is "None")
				{
					if (
						// if user wants to proxy
						useProxy
						// then either they don't have a factory (because proxy _is_ a factory)
						// or they are trying to register target class (can't proxy self -> self)
						&& (factory is { } || serviceType is null)
					)
					{
						return [];
					}

					if (targetSymbol.IsGenericType)
					{
						if (serviceType == null)
						{
							var unbound = targetSymbol.ConstructUnboundGenericType();

							return [
								BuildRegistration(
									unbound,
									unbound,
									useProxy: false,
									factory: null
								),
							];
						}

						if (serviceType.Arity != targetSymbol.Arity)
							return [];

						if (!serviceType.IsUnboundGenericType)
						{
							var concreteTargetSymbol = targetSymbol.Construct(
								serviceType.TypeArguments,
								serviceType.TypeArgumentNullableAnnotations
							);

							if (context.SemanticModel.Compilation.ClassifyConversion(concreteTargetSymbol, serviceType) is not (
								{ IsIdentity: true } or { IsImplicit: true, IsReference: true }
							))
							{
								return [];
							}

							return [
								BuildRegistration(
									serviceType,
									concreteTargetSymbol,
									useProxy: false,
									factory: null
								),
							];
						}

						if (context.SemanticModel.Compilation.ClassifyConversion(
								targetSymbol,
								serviceType.ConstructedFrom.Construct(targetSymbol.TypeArguments, targetSymbol.TypeArgumentNullableAnnotations)
							) is not (
							{ IsIdentity: true } or { IsImplicit: true, IsReference: true }
						))
						{
							return [];
						}

						return [
							BuildRegistration(
								serviceType,
								targetSymbol.ConstructUnboundGenericType(),
								useProxy: false,
								factory: null
							),
						];
					}

					if (
						serviceType != null
						&& context.SemanticModel.Compilation.ClassifyConversion(targetSymbol, serviceType) is not (
						{ IsIdentity: true } or { IsImplicit: true, IsReference: true }
						)
					)
					{
						return [];
					}

					return [BuildRegistration(serviceType ?? targetSymbol, targetSymbol, useProxy: useProxy, factory: factory)];
				}

				// service type is only valid if we aren't specifying a registration strategy
				if (serviceType != null)
					return [];

				if (registrationStrategy is "Self")
				{
					// what does it mean to proxy to the concrete when targetting self?
					if (useProxy)
						return [];

					if (targetSymbol.IsGenericType)
					{
						var unbound = targetSymbol.ConstructUnboundGenericType();
						return [BuildRegistration(unbound, unbound, useProxy: false, factory: null)];
					}

					return [BuildRegistration(targetSymbol, targetSymbol, useProxy: false, factory: factory)];
				}

				if (registrationStrategy is "ImplementedInterfaces")
				{
					if (targetSymbol.IsGenericType)
					{
						var unbound = targetSymbol.ConstructUnboundGenericType();

						return targetSymbol
							.AllInterfaces
							.Where(i =>
								i.IsGenericType
								&& i.Arity == targetSymbol.Arity
								&& i.TypeArguments.All(tp => tp is ITypeParameterSymbol)
								&& context.SemanticModel.Compilation.ClassifyConversion(
									targetSymbol,
									i.ConstructedFrom.Construct(
										targetSymbol.TypeArguments,
										targetSymbol.TypeArgumentNullableAnnotations
									)
								) is { IsIdentity: true } or { IsImplicit: true, IsReference: true }
							)
							.Select(i => BuildRegistration(i.ConstructUnboundGenericType(), unbound, useProxy: false, factory: null));
					}

					// what does it mean to proxy to the concrete, but also provide a factory when there is no self?
					if (useProxy && factory is { })
						return [];

					return targetSymbol
						.AllInterfaces
						.Select(i => BuildRegistration(i, targetSymbol, useProxy: useProxy, factory: factory));
				}

				if (registrationStrategy is "SelfAndImplementedInterfaces")
				{
					if (targetSymbol.IsGenericType)
					{
						var unbound = targetSymbol.ConstructUnboundGenericType();

						return
						[
							BuildRegistration(unbound, unbound, useProxy: false, factory: null),
							..targetSymbol
								.AllInterfaces
								.Where(i =>
									i.IsGenericType
									&& i.Arity == targetSymbol.Arity
									&& i.TypeArguments.All(tp => tp is ITypeParameterSymbol)
									&& context.SemanticModel.Compilation.ClassifyConversion(
										targetSymbol,
										i.ConstructedFrom.Construct(
											targetSymbol.TypeArguments,
											targetSymbol.TypeArgumentNullableAnnotations
										)
									) is { IsIdentity: true } or { IsImplicit: true, IsReference: true }
								)
								.Select(i => BuildRegistration(i.ConstructUnboundGenericType(), unbound, useProxy: false, factory: null)),
						];
					}

					return
					[
						BuildRegistration(targetSymbol, targetSymbol, useProxy: false, factory: factory),
						..targetSymbol
							.AllInterfaces
							.Select(i => BuildRegistration(i, targetSymbol, useProxy: useProxy, factory: factory)),
					];
				}

				return [];

				RegisterClass BuildRegistration(
					INamedTypeSymbol serviceSymbol,
					INamedTypeSymbol targetSymbol,
					bool useProxy,
					string? factory
				)
				{
					return new RegisterClass
					{
						ServiceType = serviceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
						Implementation = targetSymbol.BuildImplementationArgument(
							useProxy: useProxy,
							isKeyed: serviceKey is { },
							factory: factory
						),
						Tags = tags,
						ServiceKey = serviceKey,
						DuplicateStrategy = duplicateStrategy,
					};
				}
			})
			.ToEquatableReadOnlyList();
	}

	private static EquatableReadOnlyList<RegisterClass> TransformRegisterClass1(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		token.ThrowIfCancellationRequested();

		if (context.TargetSymbol is not INamedTypeSymbol)
			return new([]);

		return context.Attributes
			.Select(attributeData =>
			{
				token.ThrowIfCancellationRequested();

				var targetSymbol = (INamedTypeSymbol)context.TargetSymbol;
				var arguments = attributeData.NamedArguments;

				if (attributeData.AttributeClass is not
					{
						TypeArguments:
						[
						INamedTypeSymbol serviceSymbol,
						],
					})
				{
					return null;
				}

				if (targetSymbol.IsGenericType)
				{
					if (!serviceSymbol.IsGenericType)
						return null;

					if (targetSymbol.Arity != serviceSymbol.Arity)
						return null;

					targetSymbol = targetSymbol.Construct(
						serviceSymbol.TypeArguments,
						serviceSymbol.TypeArgumentNullableAnnotations
					);
				}

				if (
					context.SemanticModel.Compilation.ClassifyConversion(targetSymbol, serviceSymbol) is not (
					{ IsIdentity: true } or { IsImplicit: true, IsReference: true }
					)
				)
				{
					return null;
				}

				var tags = arguments.GetArgumentValue("Tags")?.GetStringArray();
				var serviceKey = arguments.GetArgumentValue("ServiceKey")?.ToCSharpString().NullIf("null");
				var factory = arguments.GetArgumentValue("Factory")?.Value as string;
				var duplicateStrategy = arguments.GetEnumArgumentValue("DuplicateStrategy");
				var useProxy = arguments.GetArgumentValue("UseProxyFactory")?.Value is true;

				if (factory != null && useProxy)
					return null;

				if (!targetSymbol.IsValidFactoryMethod(factory, isKeyed: serviceKey is { }))
					return null;

				return new RegisterClass
				{
					ServiceType = serviceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
					Implementation = targetSymbol.BuildImplementationArgument(useProxy, serviceKey is { }, factory),
					Tags = tags,
					ServiceKey = serviceKey,
					DuplicateStrategy = duplicateStrategy,
				};
			})
			.WhereNotNull()
			.ToEquatableReadOnlyList();
	}

	private static EquatableReadOnlyList<RegisterClass> TransformRegisterClass2(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		token.ThrowIfCancellationRequested();

		if (context.TargetSymbol is not INamedTypeSymbol { IsGenericType: true } targetSymbol)
			return new([]);

		return context.Attributes
			.Select(attributeData =>
			{
				token.ThrowIfCancellationRequested();

				var arguments = attributeData.NamedArguments;

				if (attributeData.AttributeClass is not
					{
						TypeArguments:
						[
						INamedTypeSymbol { IsGenericType: true } serviceSymbol,
						INamedTypeSymbol { IsGenericType: true } implementationSymbol
						],
					}
					|| !SymbolEqualityComparer.Default.Equals(implementationSymbol.OriginalDefinition, targetSymbol))
				{
					return null;
				}

				if (
					context.SemanticModel.Compilation.ClassifyConversion(implementationSymbol, serviceSymbol) is not (
					{ IsIdentity: true } or { IsImplicit: true, IsReference: true }
					)
				)
				{
					return null;
				}

				var tags = arguments.GetArgumentValue("Tags")?.GetStringArray();
				var serviceKey = arguments.GetArgumentValue("ServiceKey")?.ToCSharpString().NullIf("null");
				var factory = arguments.GetArgumentValue("Factory")?.Value as string;
				var duplicateStrategy = arguments.GetEnumArgumentValue("DuplicateStrategy");
				var useProxy = arguments.GetArgumentValue("UseProxyFactory")?.Value is true;

				if (factory != null && useProxy)
					return null;

				if (!targetSymbol.IsValidFactoryMethod(factory, isKeyed: serviceKey is { }))
					return null;

				return new RegisterClass
				{
					ServiceType = serviceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
					Implementation = implementationSymbol.BuildImplementationArgument(useProxy, serviceKey is { }, factory),
					Tags = tags,
					ServiceKey = serviceKey,
					DuplicateStrategy = duplicateStrategy,
				};
			})
			.WhereNotNull()
			.ToEquatableReadOnlyList();
	}
}

file static class Extensions
{
	public static TypedConstant? GetArgumentValue(this ImmutableArray<KeyValuePair<string, TypedConstant>> arguments, string name)
	{
		foreach (var argument in arguments)
		{
			if (string.Equals(name, argument.Key, StringComparison.Ordinal))
				return argument.Value;
		}

		return null;
	}

	public static string? GetEnumArgumentValue(this ImmutableArray<KeyValuePair<string, TypedConstant>> arguments, string name) =>
		arguments.GetArgumentValue(name)?.GetEnumValueName();

	public static string GetEnumValueName(this TypedConstant constant)
	{
		var fullName = constant.ToCSharpString();
		var start = fullName.LastIndexOf('.');
		return fullName[(start + 1)..];
	}

	public static string? GetStringArray(this TypedConstant constant)
	{
		if (constant.Kind != TypedConstantKind.Array)
			return null;

		return string.Join(
			", ",
			constant.Values
				.Select(tc => tc.ToCSharpString())
				.OrderBy(x => x, StringComparer.Ordinal)
		);
	}

	public static INamedTypeSymbol? GetArgumentType(this TypedConstant constant)
	{
		if (constant.Kind != TypedConstantKind.Type)
			return null;

		return constant.Value as INamedTypeSymbol;
	}

	public static string BuildImplementationArgument(
		this INamedTypeSymbol typeSymbol,
		bool useProxy,
		bool isKeyed,
		string? factory
	)
	{
		var type = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

		return (useProxy, isKeyed, factory) switch
		{
			(true, true, _) => $"global::Microsoft.Extensions.DependencyInjection.ServiceProviderKeyedServiceExtensions.GetRequiredKeyedService<{type}>",
			(true, false, _) => $"global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<{type}>",
			(false, _, { }) => $"{type}.{factory}",
			(false, _, null) => $"typeof({type})",
		};
	}
}
