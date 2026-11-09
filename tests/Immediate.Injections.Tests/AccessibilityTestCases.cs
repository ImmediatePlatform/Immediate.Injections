using System.Text.RegularExpressions;

namespace Immediate.Injections.Tests;

public static partial class AccessibilityTestCases
{
	// Exercise the shared accessibility rules once, with representative cases for each registration path.
	public static TheoryData<string> RejectedLifetimes { get; } =
	[
		Wrap("""
			[{|INJ0002:RegisterScoped|}]
			file sealed class Service : IService;
			"""),
		Wrap("""
			file class Container { [{|INJ0002:RegisterScoped|}]
			public sealed class Service : IService; }
			"""),
		Wrap("""
			public class Container { [{|INJ0002:RegisterScoped|}]
			private sealed class Service : IService; }
			"""),
		Wrap("""
			public class Container { [{|INJ0002:RegisterScoped|}]
			protected sealed class Service : IService; }
			"""),
		Wrap("""
			public class Container { [{|INJ0002:RegisterScoped|}]
			private protected sealed class Service : IService; }
			"""),
		Wrap("""
			public class Container { private class Hidden { public class Middle { [{|INJ0002:RegisterScoped|}]
			public sealed class Service : IService; } } }
			"""),
		Wrap("""
			public class Container { [{|INJ0002:RegisterScoped<IService>|}]
			private sealed class Service : IService; }
			"""),
		Wrap("""
			public class Container { [{|INJ0002:RegisterScoped<IService<string>, Service<string>>|}]
			private sealed class Service<T> : IService<T>; }
			"""),
		Wrap("""
			public class Container {
			private interface IHidden;
			[{|INJ0002:RegisterScoped(ServiceType = typeof(IHidden))|}]
			public sealed class Service : IHidden;
			}
			"""),
		Wrap("""
			public class Container {
			private interface IHidden;
			[{|INJ0002:RegisterScoped<IHidden>|}]
			public sealed class Service : IHidden;
			}
			"""),
		Wrap("""
			public class Container {
			private interface IHidden<T>;
			[{|INJ0002:RegisterScoped<IHidden<string>, Service<string>>|}]
			public sealed class Service<T> : IHidden<T>;
			}
			"""),
		Wrap("""
			public class Container {
			private interface IHidden<T>;
			[{|INJ0002:RegisterScoped(ServiceType = typeof(IHidden<string>))|}]
			public sealed class Service<T> : IHidden<T>;
			}
			"""),
		Wrap("""
			public class Container {
			private interface IHidden<T>;
			[{|INJ0002:RegisterScoped(ServiceType = typeof(IHidden<>))|}]
			public sealed class Service<T> : IHidden<T>;
			}
			"""),
		Wrap("""
			public class Container {
			private sealed class Hidden;
			[{|INJ0002:RegisterScoped(ServiceType = typeof(IService<Hidden[]>))|}]
			public sealed class Service<T> : IService<T>;
			}
			"""),
		Wrap("""
			public class Container {
			private sealed class Hidden;
			[{|INJ0002:RegisterScoped<IService<Hidden[]>>|}]
			public sealed class Service<T> : IService<T>;
			}
			"""),
		Wrap("""
			public class Container {
			private sealed class Hidden;
			[{|INJ0002:RegisterScoped<IService<Hidden[]>, Service<Hidden[]>>|}]
			public sealed class Service<T> : IService<T>;
			}
			"""),
		Wrap("""
			public class Container {
			private sealed class Hidden;
			[{|INJ0002:RegisterScoped<IService<object>, Service<Hidden[]>>|}]
			public sealed class Service<T> : IService<T>;
			}
			"""),
		Wrap("""
			public class Container { private interface IHidden; [{|INJ0002:RegisterScoped(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)|}]
			public sealed class Service : IHidden; }
			"""),
		Wrap("""
			public class Container { private interface IHidden<T>; [{|INJ0002:RegisterScoped(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)|}]
			public sealed class Service<T> : IHidden<T>; }
			"""),
		Wrap("""
			[{|INJ0002:RegisterScoped(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)|}]
			public sealed class Service;
			"""),
		Wrap("""
			[{|INJ0002:RegisterScoped(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)|}]
			public sealed class Service : IDisposable { public void Dispose() { } }
			"""),
	];

	public static TheoryData<string> AcceptedLifetimes { get; } =
	[
		Wrap("""
			internal class Container { [RegisterScoped]
			public sealed class Service : IService; }
			"""),
		Wrap("""
			public class Container { [RegisterScoped]
			protected internal sealed class Service : IService; }
			"""),
		Wrap("""
			[RegisterScoped<IService>]
			internal sealed class Service : IService;
			"""),
		Wrap("""
			[RegisterScoped<IService<string>, Service<string>>]
			internal sealed class Service<T> : IService<T>;
			"""),
		Wrap("""
			[RegisterScoped<IService<object>, Service<string[]>>]
			public sealed class Service<T> : IService<T>;
			"""),
		Wrap("""
			[RegisterScoped(ServiceType = typeof(IService<>))]
			public sealed class Service<T> : IService<T>;
			"""),
		Wrap("""
			public class Container { private interface IHidden; [RegisterScoped(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces)]
			public sealed class Service : IHidden; }
			"""),
	];

	public static TheoryData<string> RejectedOptions { get; } =
	[
		Wrap("""
			[{|INJ0016:RegisterOptions|}]
			file sealed class Service;
			"""),
	];

	public static TheoryData<string> AcceptedOptions { get; } =
	[
		Wrap("""
			internal class Container { [RegisterOptions] public sealed class Service; }
			"""),
	];

	public static TheoryData<string> RejectedHostedServices { get; } =
	[
		Wrap("""
			public class Container { private class Hidden { public class Middle { [{|INJ0018:RegisterHostedService|}] public sealed class Service : WorkerBase; } } }
			"""),
	];

	public static TheoryData<string> AcceptedHostedServices { get; } =
	[
		Wrap("""
			[RegisterHostedService]
			internal sealed class Service : WorkerBase;
			"""),
	];

	public static TheoryData<string> RejectedMethods { get; } =
	[
		Wrap("""
			public class Manual { [RegisterServices] private static void {|INJ0001:CallMe|}(IServiceCollection services) { } }
			"""),
		Wrap("""
			public class Manual { [RegisterServices] protected static void {|INJ0001:CallMe|}(IServiceCollection services) { } }
			"""),
		Wrap("""
			public class Manual { [RegisterServices] private protected static void {|INJ0001:CallMe|}(IServiceCollection services) { } }
			"""),
		Wrap("""
			public class Outer { private class Hidden { public class Manual { [RegisterServices] public static void {|INJ0001:CallMe|}(IServiceCollection services) { } } } }
			"""),
	];

	public static TheoryData<string> AcceptedMethods { get; } =
	[
		Wrap("""
			public class Manual { [RegisterServices] protected internal static void CallMe(IServiceCollection services) { } }
			"""),
		Wrap("""
			internal class Container { public class Manual { [RegisterServices] internal static void CallMe(IServiceCollection services, ReadOnlySpan<string> tags) { } } }
			"""),
	];

	[GeneratedRegex(@"\{\|INJ\d{4}:(?<core>.*?)\|\}", RegexOptions.ExplicitCapture)]
	private static partial Regex MarkupTag();

	public static string WithoutMarkup(string source)
	{
		ArgumentNullException.ThrowIfNull(source);

		return MarkupTag().Replace(source, "$1");
	}

	private static string Wrap(string source) =>
		"""
		using System;
		using System.Threading;
		using System.Threading.Tasks;
		using Immediate.Injections.Shared;
		using Microsoft.Extensions.DependencyInjection;
		using Microsoft.Extensions.Hosting;

		public interface IService;
		public interface IService<out T>;
		public abstract class WorkerBase : BackgroundService
		{
		    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
		}
		""" + Environment.NewLine + source;
}
