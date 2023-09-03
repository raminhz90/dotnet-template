using System.Globalization;
using Microsoft.Extensions.Hosting;
using Serilog;
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
			.WriteTo.Conditional(
				x => context.HostingEnvironment.IsDevelopment(),
				x => x.Console(formatProvider: CultureInfo.InvariantCulture)
					.WriteTo.Debug(formatProvider: CultureInfo.InvariantCulture))
			.WriteTo.Conditional(
				x => !context.HostingEnvironment.IsDevelopment(),
				x => x.File("Logs/log.txt", rollingInterval: LogRollingInterval,
					retainedFileCountLimit: LogFileCountLimit, fileSizeLimitBytes: LogFileSizeLimitBytes,
					buffered: true, flushToDiskInterval: TimeSpan.FromSeconds(5), restrictedToMinimumLevel: LogEventLevel.Information,
					formatProvider: CultureInfo.InvariantCulture));
	}
}
