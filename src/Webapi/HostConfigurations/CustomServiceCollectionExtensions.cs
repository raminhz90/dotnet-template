using Boxed.AspNetCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Webapi.Options;

namespace Webapi.HostConfigurations;




/// <summary>
/// <see cref="IServiceCollection"/> extension methods which extend ASP.NET Core services.
/// </summary>
public static class CustomServiceCollectionExtensions
{
	/// <summary>
	/// Configures the settings by binding the contents of the appsettings.json file to the specified Plain Old CLR
	/// Objects (POCO) and adding <see cref="IOptions{T}"/> objects to the services collection.
	/// </summary>
	/// <param name="services">The services.</param>
	/// <param name="configuration">The configuration.</param>
	/// <returns>The services with options services added.</returns>
	public static IServiceCollection AddCustomOptions(
		this IServiceCollection services,
		IConfiguration configuration) =>
		services
			// ConfigureAndValidateSingleton registers IOptions<T> and also T as a singleton to the services collection.
			.ConfigureAndValidateSingleton<ApplicationOptions>(configuration)
			.ConfigureAndValidateSingleton<CompressionOptions>(configuration.GetRequiredSection(nameof(ApplicationOptions.Compression)))
			.ConfigureAndValidateSingleton<ForwardedHeadersOptions>(configuration.GetRequiredSection(nameof(ApplicationOptions.ForwardedHeaders)))
			.Configure<ForwardedHeadersOptions>(
				options =>
				{
					options.KnownNetworks.Clear();
					options.KnownProxies.Clear();
				})
			.ConfigureAndValidateSingleton<HostOptions>(configuration.GetRequiredSection(nameof(ApplicationOptions.Host)))
			.ConfigureAndValidateSingleton<KestrelServerOptions>(configuration.GetRequiredSection(nameof(ApplicationOptions.Kestrel)));

	public static IServiceCollection AddCustomConfigureOptions(this IServiceCollection services) =>
		services
			.ConfigureOptions<ConfigureJsonOptions>()
			.ConfigureOptions<ConfigureRequestLoggingOptions>()
			.ConfigureOptions<ConfigureResponseCompressionOptions>();

	public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services) =>
		services
			.AddHealthChecks()
			// Add health checks for external dependencies here. See https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks
			.Services;
}
