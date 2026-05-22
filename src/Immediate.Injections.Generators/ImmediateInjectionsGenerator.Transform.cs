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
		string? duplicateStrategy = null;
		string? registrationStrategy = null;

		foreach (var argument in context.Attributes[0].NamedArguments)
		{
			switch (argument.Key)
			{
				case "RegistrationStrategy":
					registrationStrategy = GetEnumValueName(argument.Value);
					break;

				case "DuplicateStrategy":
					duplicateStrategy = GetEnumValueName(argument.Value);
					break;

				default:
					break;
			}
		}

		return new()
		{
			DuplicateStrategy = duplicateStrategy,
			RegistrationStrategy = registrationStrategy,
		};

		static string GetEnumValueName(TypedConstant constant)
		{
			var fullName = constant.ToCSharpString();
			var start = fullName.LastIndexOf('.');
			return fullName[(start + 1)..];
		}
	}

	private RegisterServicesMethod? TransformRegisterServicesMethod(GeneratorAttributeSyntaxContext context, CancellationToken token)
	{
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
}
