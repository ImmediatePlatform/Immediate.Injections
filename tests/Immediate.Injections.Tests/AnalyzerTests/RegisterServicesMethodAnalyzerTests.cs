using Immediate.Injections.Analyzers;
using Immediate.Injections.Tests.GeneratorTests;

namespace Immediate.Injections.Tests.AnalyzerTests;

public sealed class RegisterServicesMethodAnalyzerTests
{
	[Fact]
	public async Task ValidRegisterServicesMethod1DoesNotTrigger() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterServicesMethodAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public class Class
			{
				[RegisterServices]
				public static void CallMe(IServiceCollection services)
				{
				}
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task ValidRegisterServicesMethod2DoesNotTrigger() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterServicesMethodAnalyzer>(
			"""
			using System;

			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public class Class
			{
				[RegisterServices]
				public static void CallMe(IServiceCollection services, ReadOnlySpan<string> tags)
				{
				}
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task AsyncRegisterServicesMethodTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterServicesMethodAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public class Class
			{
				[RegisterServices]
				public static async void {|INJ0001:CallMe|}(IServiceCollection services)
				{
				}
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task NonVoidReturnRegisterServicesMethodTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterServicesMethodAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public class Class
			{
				[RegisterServices]
				public static int {|INJ0001:CallMe|}(IServiceCollection services)
				{
					return 1;
				}
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task MissingParametersRegisterServicesMethodTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterServicesMethodAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public class Class
			{
				[RegisterServices]
				public static void {|INJ0001:CallMe|}()
				{
				}
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task TooManyParametersRegisterServicesMethodTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterServicesMethodAnalyzer>(
			"""
			using System;

			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public class Class
			{
				[RegisterServices]
				public static void {|INJ0001:CallMe|}(IServiceCollection services, ReadOnlySpan<string> tags, int x)
				{
				}
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task IncorrectParametersRegisterServicesMethodTriggers() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RegisterServicesMethodAnalyzer>(
			"""
			using Immediate.Injections.Shared;
			using Microsoft.Extensions.DependencyInjection;
			
			public class Class
			{
				[RegisterServices]
				public static void {|INJ0001:CallMe|}(int x)
				{
				}
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);
}
