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
		return new([]);
	}

	private static RegisterClass? TransformRegisterClass1(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		token.ThrowIfCancellationRequested();

		if (context.TargetSymbol is not INamedTypeSymbol { IsGenericType: false } targetSymbol)
			return null;

		var attributeData = context.Attributes[0];
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

		if (!targetSymbol.IsValidFactoryMethod(factory, isKeyed: serviceKey is { }))
			return null;

		return new RegisterClass
		{
			ServiceType = serviceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
			Implementation = targetSymbol.BuildImplementationArgument(factory),
			Tags = tags,
			ServiceKey = serviceKey,
			Factory = factory,
			DuplicateStrategy = duplicateStrategy,
		};
	}

	private static RegisterClass? TransformRegisterClass2(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
		token.ThrowIfCancellationRequested();

		if (context.TargetSymbol is not INamedTypeSymbol { IsGenericType: true } targetSymbol)
			return null;

		var attributeData = context.Attributes[0];
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

		if (!targetSymbol.IsValidFactoryMethod(factory, isKeyed: serviceKey is { }))
			return null;

		return new RegisterClass
		{
			ServiceType = serviceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
			Implementation = implementationSymbol.BuildImplementationArgument(factory),
			Tags = tags,
			ServiceKey = serviceKey,
			Factory = factory,
			DuplicateStrategy = duplicateStrategy,
		};
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

		return string.Join(", ", constant.Values.Select(tc => tc.ToCSharpString()));
	}

	public static string BuildImplementationArgument(
		this INamedTypeSymbol typeSymbol,
		string? factory
	)
	{
		var type = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

		return factory switch
		{
			{ } => $"{type}.{factory}",
			null => $"typeof({type})",
		};
	}
}
