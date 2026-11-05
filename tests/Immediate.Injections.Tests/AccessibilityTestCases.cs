namespace Immediate.Injections.Tests;

public static class AccessibilityTestCases
{
	public static TheoryData<string> RejectedLifetimes { get; } =
	[
		Wrap("""
			public static partial class RegistrationServiceCollectionExtensions { [{|INJ0002:RegisterScoped|}]
			[{|INJ0002:RegisterSingleton|}]
			[{|INJ0002:RegisterTransient|}] private sealed class Service : IService; }
			"""),
		Wrap("""
			[{|INJ0002:RegisterScoped|}]
			[{|INJ0002:RegisterSingleton|}]
			[{|INJ0002:RegisterTransient|}]
			file sealed class Service : IService;
			"""),
		Wrap("""
			file class Container { [{|INJ0002:RegisterScoped|}]
			[{|INJ0002:RegisterSingleton|}]
			[{|INJ0002:RegisterTransient|}] public sealed class Service : IService; }
			"""),
		Wrap("""
			public class Container { [{|INJ0002:RegisterScoped|}]
			[{|INJ0002:RegisterSingleton|}]
			[{|INJ0002:RegisterTransient|}] private sealed class Service : IService; }
			"""),
		Wrap("""
			public class Container { [{|INJ0002:RegisterScoped|}]
			[{|INJ0002:RegisterSingleton|}]
			[{|INJ0002:RegisterTransient|}] protected sealed class Service : IService; }
			"""),
		Wrap("""
			public class Container { [{|INJ0002:RegisterScoped|}]
			[{|INJ0002:RegisterSingleton|}]
			[{|INJ0002:RegisterTransient|}] private protected sealed class Service : IService; }
			"""),
		Wrap("""
			public class Container { private class Hidden { public class Middle { [{|INJ0002:RegisterScoped|}]
			[{|INJ0002:RegisterSingleton|}]
			[{|INJ0002:RegisterTransient|}] public sealed class Service : IService; } } }
			"""),
		Wrap("""
			public static partial class RegistrationServiceCollectionExtensions { [{|INJ0002:RegisterScoped<IService>|}]
			[{|INJ0002:RegisterSingleton<IService>|}]
			[{|INJ0002:RegisterTransient<IService>|}] private sealed class Service : IService; }
			"""),
		Wrap("""
			[{|INJ0002:RegisterScoped<IService>|}]
			[{|INJ0002:RegisterSingleton<IService>|}]
			[{|INJ0002:RegisterTransient<IService>|}]
			file sealed class Service : IService;
			"""),
		Wrap("""
			file class Container { [{|INJ0002:RegisterScoped<IService>|}]
			[{|INJ0002:RegisterSingleton<IService>|}]
			[{|INJ0002:RegisterTransient<IService>|}] public sealed class Service : IService; }
			"""),
		Wrap("""
			public class Container { [{|INJ0002:RegisterScoped<IService>|}]
			[{|INJ0002:RegisterSingleton<IService>|}]
			[{|INJ0002:RegisterTransient<IService>|}] private sealed class Service : IService; }
			"""),
		Wrap("""
			public class Container { [{|INJ0002:RegisterScoped<IService>|}]
			[{|INJ0002:RegisterSingleton<IService>|}]
			[{|INJ0002:RegisterTransient<IService>|}] protected sealed class Service : IService; }
			"""),
		Wrap("""
			public class Container { [{|INJ0002:RegisterScoped<IService>|}]
			[{|INJ0002:RegisterSingleton<IService>|}]
			[{|INJ0002:RegisterTransient<IService>|}] private protected sealed class Service : IService; }
			"""),
		Wrap("""
			public class Container { private class Hidden { public class Middle { [{|INJ0002:RegisterScoped<IService>|}]
			[{|INJ0002:RegisterSingleton<IService>|}]
			[{|INJ0002:RegisterTransient<IService>|}] public sealed class Service : IService; } } }
			"""),
		Wrap("""
			public static partial class RegistrationServiceCollectionExtensions { [{|INJ0002:RegisterScoped<IService<string>, Service<string>>|}]
			[{|INJ0002:RegisterSingleton<IService<string>, Service<string>>|}]
			[{|INJ0002:RegisterTransient<IService<string>, Service<string>>|}] private sealed class Service<T> : IService<T>; }
			"""),
		Wrap("""
			[{|INJ0002:RegisterScoped<IService<string>, Service<string>>|}]
			[{|INJ0002:RegisterSingleton<IService<string>, Service<string>>|}]
			[{|INJ0002:RegisterTransient<IService<string>, Service<string>>|}]
			file sealed class Service<T> : IService<T>;
			"""),
		Wrap("""
			file class Container { [{|INJ0002:RegisterScoped<IService<string>, Service<string>>|}]
			[{|INJ0002:RegisterSingleton<IService<string>, Service<string>>|}]
			[{|INJ0002:RegisterTransient<IService<string>, Service<string>>|}] public sealed class Service<T> : IService<T>; }
			"""),
		Wrap("""
			public class Container { [{|INJ0002:RegisterScoped<IService<string>, Service<string>>|}]
			[{|INJ0002:RegisterSingleton<IService<string>, Service<string>>|}]
			[{|INJ0002:RegisterTransient<IService<string>, Service<string>>|}] private sealed class Service<T> : IService<T>; }
			"""),
		Wrap("""
			public class Container { [{|INJ0002:RegisterScoped<IService<string>, Service<string>>|}]
			[{|INJ0002:RegisterSingleton<IService<string>, Service<string>>|}]
			[{|INJ0002:RegisterTransient<IService<string>, Service<string>>|}] protected sealed class Service<T> : IService<T>; }
			"""),
		Wrap("""
			public class Container { [{|INJ0002:RegisterScoped<IService<string>, Service<string>>|}]
			[{|INJ0002:RegisterSingleton<IService<string>, Service<string>>|}]
			[{|INJ0002:RegisterTransient<IService<string>, Service<string>>|}] private protected sealed class Service<T> : IService<T>; }
			"""),
		Wrap("""
			public class Container { private class Hidden { public class Middle { [{|INJ0002:RegisterScoped<IService<string>, Service<string>>|}]
			[{|INJ0002:RegisterSingleton<IService<string>, Service<string>>|}]
			[{|INJ0002:RegisterTransient<IService<string>, Service<string>>|}] public sealed class Service<T> : IService<T>; } } }
			"""),
		Wrap("""
			public class Container {
			private interface IHidden;
			[{|INJ0002:RegisterScoped(ServiceType = typeof(IHidden))|}]
			[{|INJ0002:RegisterSingleton(ServiceType = typeof(IHidden))|}]
			[{|INJ0002:RegisterTransient(ServiceType = typeof(IHidden))|}]
			public sealed class Service : IHidden;
			}
			"""),
		Wrap("""
			public class Container {
			private interface IHidden;
			[{|INJ0002:RegisterScoped<IHidden>|}]
			[{|INJ0002:RegisterSingleton<IHidden>|}]
			[{|INJ0002:RegisterTransient<IHidden>|}]
			public sealed class Service : IHidden;
			}
			"""),
		Wrap("""
			public class Container {
			private interface IHidden<T>;
			[{|INJ0002:RegisterScoped<IHidden<string>, Service<string>>|}]
			[{|INJ0002:RegisterSingleton<IHidden<string>, Service<string>>|}]
			[{|INJ0002:RegisterTransient<IHidden<string>, Service<string>>|}]
			public sealed class Service<T> : IHidden<T>;
			}
			"""),
		Wrap("""
			public class Container {
			private interface IHidden<T>;
			[{|INJ0002:RegisterScoped(ServiceType = typeof(IHidden<string>))|}]
			[{|INJ0002:RegisterSingleton(ServiceType = typeof(IHidden<string>))|}]
			[{|INJ0002:RegisterTransient(ServiceType = typeof(IHidden<string>))|}]
			public sealed class Service<T> : IHidden<T>;
			}
			"""),
		Wrap("""
			public class Container {
			private interface IHidden<T>;
			[{|INJ0002:RegisterScoped(ServiceType = typeof(IHidden<>))|}]
			[{|INJ0002:RegisterSingleton(ServiceType = typeof(IHidden<>))|}]
			[{|INJ0002:RegisterTransient(ServiceType = typeof(IHidden<>))|}]
			public sealed class Service<T> : IHidden<T>;
			}
			"""),
		Wrap("""
			public class Container {
			private sealed class Hidden;
			[{|INJ0002:RegisterScoped(ServiceType = typeof(IService<Hidden[]>))|}]
			[{|INJ0002:RegisterSingleton(ServiceType = typeof(IService<Hidden[]>))|}]
			[{|INJ0002:RegisterTransient(ServiceType = typeof(IService<Hidden[]>))|}]
			public sealed class Service<T> : IService<T>;
			}
			"""),
		Wrap("""
			public class Container {
			private sealed class Hidden;
			[{|INJ0002:RegisterScoped<IService<Hidden[]>>|}]
			[{|INJ0002:RegisterSingleton<IService<Hidden[]>>|}]
			[{|INJ0002:RegisterTransient<IService<Hidden[]>>|}]
			public sealed class Service<T> : IService<T>;
			}
			"""),
		Wrap("""
			public class Container {
			private sealed class Hidden;
			[{|INJ0002:RegisterScoped<IService<Hidden[]>, Service<Hidden[]>>|}]
			[{|INJ0002:RegisterSingleton<IService<Hidden[]>, Service<Hidden[]>>|}]
			[{|INJ0002:RegisterTransient<IService<Hidden[]>, Service<Hidden[]>>|}]
			public sealed class Service<T> : IService<T>;
			}
			"""),
		Wrap("""
			public class Container {
			private sealed class Hidden;
			[{|INJ0002:RegisterScoped<IService<object>, Service<Hidden[]>>|}]
			[{|INJ0002:RegisterSingleton<IService<object>, Service<Hidden[]>>|}]
			[{|INJ0002:RegisterTransient<IService<object>, Service<Hidden[]>>|}]
			public sealed class Service<T> : IService<T>;
			}
			"""),
		Wrap("""
			public class Container { private interface IHidden; [{|INJ0002:RegisterScoped(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)|}]
			[{|INJ0002:RegisterSingleton(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)|}]
			[{|INJ0002:RegisterTransient(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)|}] public sealed class Service : IHidden; }
			"""),
		Wrap("""
			public class Container { private interface IHidden<T>; [{|INJ0002:RegisterScoped(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)|}]
			[{|INJ0002:RegisterSingleton(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)|}]
			[{|INJ0002:RegisterTransient(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)|}] public sealed class Service<T> : IHidden<T>; }
			"""),
		Wrap("""
			[{|INJ0002:RegisterScoped(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)|}]
			[{|INJ0002:RegisterSingleton(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)|}]
			[{|INJ0002:RegisterTransient(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)|}] public sealed class Service;
			"""),
		Wrap("""
			[{|INJ0002:RegisterScoped(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)|}]
			[{|INJ0002:RegisterSingleton(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)|}]
			[{|INJ0002:RegisterTransient(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)|}] public sealed class Service : IDisposable { public void Dispose() { } }
			"""),
	];

	public static TheoryData<string, string[]> AcceptedLifetimes { get; } =
	[
		(Wrap("""
			[RegisterScoped]
			[RegisterSingleton]
			[RegisterTransient]
			public sealed class Service : IService;
			"""), ["II.RegisterScoped`0.g.cs", "II.RegisterSingleton`0.g.cs", "II.RegisterTransient`0.g.cs"]),
		(Wrap("""
			[RegisterScoped]
			[RegisterSingleton]
			[RegisterTransient]
			internal sealed class Service : IService;
			"""), ["II.RegisterScoped`0.g.cs", "II.RegisterSingleton`0.g.cs", "II.RegisterTransient`0.g.cs"]),
		(Wrap("""
			public class Container { [RegisterScoped]
			[RegisterSingleton]
			[RegisterTransient] internal sealed class Service : IService; }
			"""), ["II.RegisterScoped`0.g.cs", "II.RegisterSingleton`0.g.cs", "II.RegisterTransient`0.g.cs"]),
		(Wrap("""
			internal class Container { [RegisterScoped]
			[RegisterSingleton]
			[RegisterTransient] public sealed class Service : IService; }
			"""), ["II.RegisterScoped`0.g.cs", "II.RegisterSingleton`0.g.cs", "II.RegisterTransient`0.g.cs"]),
		(Wrap("""
			public class Container { [RegisterScoped]
			[RegisterSingleton]
			[RegisterTransient] protected internal sealed class Service : IService; }
			"""), ["II.RegisterScoped`0.g.cs", "II.RegisterSingleton`0.g.cs", "II.RegisterTransient`0.g.cs"]),
		(Wrap("""
			[RegisterScoped<IService>]
			[RegisterSingleton<IService>]
			[RegisterTransient<IService>]
			public sealed class Service : IService;
			"""), ["II.RegisterScoped`1.g.cs", "II.RegisterSingleton`1.g.cs", "II.RegisterTransient`1.g.cs"]),
		(Wrap("""
			[RegisterScoped<IService>]
			[RegisterSingleton<IService>]
			[RegisterTransient<IService>]
			internal sealed class Service : IService;
			"""), ["II.RegisterScoped`1.g.cs", "II.RegisterSingleton`1.g.cs", "II.RegisterTransient`1.g.cs"]),
		(Wrap("""
			public class Container { [RegisterScoped<IService>]
			[RegisterSingleton<IService>]
			[RegisterTransient<IService>] internal sealed class Service : IService; }
			"""), ["II.RegisterScoped`1.g.cs", "II.RegisterSingleton`1.g.cs", "II.RegisterTransient`1.g.cs"]),
		(Wrap("""
			internal class Container { [RegisterScoped<IService>]
			[RegisterSingleton<IService>]
			[RegisterTransient<IService>] public sealed class Service : IService; }
			"""), ["II.RegisterScoped`1.g.cs", "II.RegisterSingleton`1.g.cs", "II.RegisterTransient`1.g.cs"]),
		(Wrap("""
			public class Container { [RegisterScoped<IService>]
			[RegisterSingleton<IService>]
			[RegisterTransient<IService>] protected internal sealed class Service : IService; }
			"""), ["II.RegisterScoped`1.g.cs", "II.RegisterSingleton`1.g.cs", "II.RegisterTransient`1.g.cs"]),
		(Wrap("""
			[RegisterScoped<IService<string>, Service<string>>]
			[RegisterSingleton<IService<string>, Service<string>>]
			[RegisterTransient<IService<string>, Service<string>>]
			public sealed class Service<T> : IService<T>;
			"""), ["II.RegisterScoped`2.g.cs", "II.RegisterSingleton`2.g.cs", "II.RegisterTransient`2.g.cs"]),
		(Wrap("""
			[RegisterScoped<IService<string>, Service<string>>]
			[RegisterSingleton<IService<string>, Service<string>>]
			[RegisterTransient<IService<string>, Service<string>>]
			internal sealed class Service<T> : IService<T>;
			"""), ["II.RegisterScoped`2.g.cs", "II.RegisterSingleton`2.g.cs", "II.RegisterTransient`2.g.cs"]),
		(Wrap("""
			public class Container { [RegisterScoped<IService<string>, Service<string>>]
			[RegisterSingleton<IService<string>, Service<string>>]
			[RegisterTransient<IService<string>, Service<string>>] internal sealed class Service<T> : IService<T>; }
			"""), ["II.RegisterScoped`2.g.cs", "II.RegisterSingleton`2.g.cs", "II.RegisterTransient`2.g.cs"]),
		(Wrap("""
			internal class Container { [RegisterScoped<IService<string>, Service<string>>]
			[RegisterSingleton<IService<string>, Service<string>>]
			[RegisterTransient<IService<string>, Service<string>>] public sealed class Service<T> : IService<T>; }
			"""), ["II.RegisterScoped`2.g.cs", "II.RegisterSingleton`2.g.cs", "II.RegisterTransient`2.g.cs"]),
		(Wrap("""
			public class Container { [RegisterScoped<IService<string>, Service<string>>]
			[RegisterSingleton<IService<string>, Service<string>>]
			[RegisterTransient<IService<string>, Service<string>>] protected internal sealed class Service<T> : IService<T>; }
			"""), ["II.RegisterScoped`2.g.cs", "II.RegisterSingleton`2.g.cs", "II.RegisterTransient`2.g.cs"]),
		(Wrap("""
			[RegisterScoped<IService<object>, Service<string[]>>]
			[RegisterSingleton<IService<object>, Service<string[]>>]
			[RegisterTransient<IService<object>, Service<string[]>>]
			public sealed class Service<T> : IService<T>;
			"""), ["II.RegisterScoped`2.g.cs", "II.RegisterSingleton`2.g.cs", "II.RegisterTransient`2.g.cs"]),
		(Wrap("""
			[RegisterScoped(ServiceType = typeof(IService<>))]
			[RegisterSingleton(ServiceType = typeof(IService<>))]
			[RegisterTransient(ServiceType = typeof(IService<>))]
			public sealed class Service<T> : IService<T>;
			"""), ["II.RegisterScoped`0.g.cs", "II.RegisterSingleton`0.g.cs", "II.RegisterTransient`0.g.cs"]),
		(Wrap("""
			public class Container { private interface IHidden;
			[RegisterScoped(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)]
			[RegisterSingleton(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)]
			[RegisterTransient(RegistrationStrategy = RegistrationStrategy.ImplementedInterfaces)]
			public sealed class Service : IService, IHidden, IDisposable { public void Dispose() { } } }
			"""), ["II.RegisterScoped`0.g.cs", "II.RegisterSingleton`0.g.cs", "II.RegisterTransient`0.g.cs"]),
		(Wrap("""
			public class Container { private interface IHidden;
			[RegisterScoped(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces)]
			[RegisterSingleton(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces)]
			[RegisterTransient(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces)]
			public sealed class Service : IService, IHidden, IDisposable { public void Dispose() { } } }
			"""), ["II.RegisterScoped`0.g.cs", "II.RegisterSingleton`0.g.cs", "II.RegisterTransient`0.g.cs"]),
		(Wrap("""
			public class Container { private interface IHidden; [RegisterScoped(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces)]
			[RegisterSingleton(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces)]
			[RegisterTransient(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces)] public sealed class Service : IHidden; }
			"""), ["II.RegisterScoped`0.g.cs", "II.RegisterSingleton`0.g.cs", "II.RegisterTransient`0.g.cs"]),
	];

	public static TheoryData<string> RejectedOptions { get; } =
	[
		Wrap("""
			public static partial class RegistrationServiceCollectionExtensions { [{|INJ0016:RegisterOptions|}] private sealed class Service; }
			"""),
		Wrap("""
			[{|INJ0016:RegisterOptions|}]
			file sealed class Service;
			"""),
		Wrap("""
			file class Container { [{|INJ0016:RegisterOptions|}] public sealed class Service; }
			"""),
		Wrap("""
			public class Container { [{|INJ0016:RegisterOptions|}] private sealed class Service; }
			"""),
		Wrap("""
			public class Container { [{|INJ0016:RegisterOptions|}] protected sealed class Service; }
			"""),
		Wrap("""
			public class Container { [{|INJ0016:RegisterOptions|}] private protected sealed class Service; }
			"""),
		Wrap("""
			public class Container { private class Hidden { public class Middle { [{|INJ0016:RegisterOptions|}] public sealed class Service; } } }
			"""),
	];

	public static TheoryData<string, string[]> AcceptedOptions { get; } =
	[
		(Wrap("""
			[RegisterOptions]
			public sealed class Service;
			"""), ["II.RegisterOptions.g.cs"]),
		(Wrap("""
			[RegisterOptions]
			internal sealed class Service;
			"""), ["II.RegisterOptions.g.cs"]),
		(Wrap("""
			public class Container { [RegisterOptions] internal sealed class Service; }
			"""), ["II.RegisterOptions.g.cs"]),
		(Wrap("""
			internal class Container { [RegisterOptions] public sealed class Service; }
			"""), ["II.RegisterOptions.g.cs"]),
		(Wrap("""
			public class Container { [RegisterOptions] protected internal sealed class Service; }
			"""), ["II.RegisterOptions.g.cs"]),
	];

	public static TheoryData<string> RejectedHostedServices { get; } =
	[
		Wrap("""
			public static partial class RegistrationServiceCollectionExtensions { [{|INJ0018:RegisterHostedService|}] private sealed class Service : WorkerBase; }
			"""),
		Wrap("""
			[{|INJ0018:RegisterHostedService|}]
			file sealed class Service : WorkerBase;
			"""),
		Wrap("""
			file class Container { [{|INJ0018:RegisterHostedService|}] public sealed class Service : WorkerBase; }
			"""),
		Wrap("""
			public class Container { [{|INJ0018:RegisterHostedService|}] private sealed class Service : WorkerBase; }
			"""),
		Wrap("""
			public class Container { [{|INJ0018:RegisterHostedService|}] protected sealed class Service : WorkerBase; }
			"""),
		Wrap("""
			public class Container { [{|INJ0018:RegisterHostedService|}] private protected sealed class Service : WorkerBase; }
			"""),
		Wrap("""
			public class Container { private class Hidden { public class Middle { [{|INJ0018:RegisterHostedService|}] public sealed class Service : WorkerBase; } } }
			"""),
	];

	public static TheoryData<string, string[]> AcceptedHostedServices { get; } =
	[
		(Wrap("""
			[RegisterHostedService]
			public sealed class Service : WorkerBase;
			"""), ["II.RegisterHostedServices.g.cs"]),
		(Wrap("""
			[RegisterHostedService]
			internal sealed class Service : WorkerBase;
			"""), ["II.RegisterHostedServices.g.cs"]),
		(Wrap("""
			public class Container { [RegisterHostedService] internal sealed class Service : WorkerBase; }
			"""), ["II.RegisterHostedServices.g.cs"]),
		(Wrap("""
			internal class Container { [RegisterHostedService] public sealed class Service : WorkerBase; }
			"""), ["II.RegisterHostedServices.g.cs"]),
		(Wrap("""
			public class Container { [RegisterHostedService] protected internal sealed class Service : WorkerBase; }
			"""), ["II.RegisterHostedServices.g.cs"]),
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
			file class Manual { [RegisterServices] public static void {|INJ0001:CallMe|}(IServiceCollection services) { } }
			"""),
		Wrap("""
			file class Container { public class Manual { [RegisterServices] public static void {|INJ0001:CallMe|}(IServiceCollection services) { } } }
			"""),
		Wrap("""
			public class Container { private class Manual { [RegisterServices] public static void {|INJ0001:CallMe|}(IServiceCollection services) { } } }
			"""),
		Wrap("""
			public class Container { protected class Manual { [RegisterServices] public static void {|INJ0001:CallMe|}(IServiceCollection services) { } } }
			"""),
		Wrap("""
			public class Container { private protected class Manual { [RegisterServices] public static void {|INJ0001:CallMe|}(IServiceCollection services) { } } }
			"""),
		Wrap("""
			public class Outer { private class Hidden { public class Manual { [RegisterServices] public static void {|INJ0001:CallMe|}(IServiceCollection services) { } } } }
			"""),
		Wrap("""
			public static partial class RegistrationServiceCollectionExtensions { [RegisterServices] private static void {|INJ0001:CallMe|}(IServiceCollection services) { } }
			"""),
	];

	public static TheoryData<string, string[]> AcceptedMethods { get; } =
	[
		(Wrap("""
			public class Manual { [RegisterServices] public static void CallMe(IServiceCollection services) { } }
			"""), ["II.RegisterServicesMethods.g.cs"]),
		(Wrap("""
			public class Manual { [RegisterServices] internal static void CallMe(IServiceCollection services) { } }
			"""), ["II.RegisterServicesMethods.g.cs"]),
		(Wrap("""
			public class Manual { [RegisterServices] protected internal static void CallMe(IServiceCollection services) { } }
			"""), ["II.RegisterServicesMethods.g.cs"]),
		(Wrap("""
			internal class Container { public class Manual { [RegisterServices] internal static void CallMe(IServiceCollection services, ReadOnlySpan<string> tags) { } } }
			"""), ["II.RegisterServicesMethods.g.cs"]),
	];

	public static string WithoutMarkup(string source)
	{
		ArgumentNullException.ThrowIfNull(source);
		return source.Replace("{|INJ0001:", "", StringComparison.Ordinal)
			.Replace("{|INJ0002:", "", StringComparison.Ordinal)
			.Replace("{|INJ0016:", "", StringComparison.Ordinal)
			.Replace("{|INJ0018:", "", StringComparison.Ordinal)
			.Replace("|}", "", StringComparison.Ordinal);
	}

	private static string Wrap(string source) =>
		"""
		using System;
		using System.Threading;
		using System.Threading.Tasks;
		using Immediate.Injections.Shared;
		using Microsoft.Extensions.DependencyInjection;
		using Microsoft.Extensions.Hosting;
		using Microsoft.Extensions.Options;

		public interface IService;
		public interface IService<out T>;
		public abstract class WorkerBase : BackgroundService
		{
		    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
		}
		""" + Environment.NewLine + source;
}
