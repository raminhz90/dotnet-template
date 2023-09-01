using System.Globalization;
using Serilog;
using Serilog.Extensions.Hosting;

namespace logger;

public static class CustomLogger
{
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

}
