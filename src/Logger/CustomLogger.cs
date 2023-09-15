using System.Globalization;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.CallerInfo;
using Serilog.Events;
using Serilog.Extensions.Hosting;

namespace logger;

public static class CustomLogger
{
	private const RollingInterval LogRollingInterval = RollingInterval.Month;
	private const int LogFileCountLimit = 12;
	private const int LogFileSizeLimitBytes = 50 * 1024 * 1024;

	/// <summary>
	/// Creates a logger used during application initialization.
	/// <see href="https://nblumhardt.com/2020/10/bootstrap-logger/"/>.
	/// </summary>
	/// <returns>A reloadable logger that can be reconfigured.</returns>
	public static ReloadableLogger CreateBootstrapLogger() =>
		new LoggerConfiguration()
			.WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
			.WriteTo.Debug(formatProvider: CultureInfo.InvariantCulture)
			.CreateBootstrapLogger();

	public static void LogApplicationTerminatedUnexpectedly(Exception exception)
	{

		LogToConsole(exception);
		static void LogToConsole(Exception exception)
		{

			Console.WriteLine($"Application terminated unexpectedly.");
			Console.WriteLine(exception.ToString());
		}
	}

	public static void ConfigureReloadableLogger(
		HostBuilderContext context,
		IServiceProvider services,
		LoggerConfiguration configuration)
	{
		ArgumentNullException.ThrowIfNull(configuration);
		_ = configuration
			.ReadFrom.Configuration(context.Configuration)
			.ReadFrom.Services(services)
			.Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
			.Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
			//when in development enrich with additional detail
			//turn off any of them you do not need
			.Enrich.When(
				x => context.HostingEnvironment.IsDevelopment(),
				x => x.WithMemoryUsage())
				.Enrich.When(
				x => context.HostingEnvironment.IsDevelopment(),
				x => x.WithProcessId())
				.Enrich.When(
				x => context.HostingEnvironment.IsDevelopment(),
				x => x.WithProcessName())
				.Enrich.When(
				x => context.HostingEnvironment.IsDevelopment(),
				x => x.WithThreadId())
				.Enrich.When(
				x => context.HostingEnvironment.IsDevelopment(),
				x => x.WithThreadName())
				.Enrich.When(
				x => context.HostingEnvironment.IsDevelopment(),
				x => x.WithMemoryUsage())
				.Enrich.When(
				x => context.HostingEnvironment.IsDevelopment(),
				x => x.WithCallerInfo(includeFileInfo: true, assemblyPrefix: "", filePathDepth: 3,
				 excludedPrefixes: new List<string>() { "Serilog", "Microsoft", "System" }))
			.WriteTo.Conditional(
				x => context.HostingEnvironment.IsDevelopment(),
				x => x.Console(formatProvider: CultureInfo.InvariantCulture,
				outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level:u3}] <Process:{ProcessId}:{ProcessName}> <Thread:{ThreadId}:{ThreadName}> <MemoryUsage:{MemoryUsage} Bytes>{NewLine}<Namespace:{Namespace}> <SourceFile:{SourceFile}> <LineNumber:{LineNumber}>{NewLine}{Message}{NewLine}{Exception}{NewLine}")
			)
			// In Production writes the log to a file 
			// and restricts the log level to Information to avoid writing Debug and Verbose logs to disk.
			.WriteTo.Conditional(
				x => !context.HostingEnvironment.IsDevelopment(),
				x => x.File("Logs/log.txt", rollingInterval: LogRollingInterval,
					retainedFileCountLimit: LogFileCountLimit, fileSizeLimitBytes: LogFileSizeLimitBytes,
					buffered: true, flushToDiskInterval: TimeSpan.FromSeconds(1), restrictedToMinimumLevel: LogEventLevel.Information,
					formatProvider: CultureInfo.InvariantCulture));
	}
}
