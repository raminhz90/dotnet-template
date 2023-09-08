using Boxed.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Serilog;
using Webapi.HostConfigurations;

namespace Webapi;
/// <summary>
/// The main start-up class for the application.
/// </summary>
public class Startup
{
	private readonly IConfiguration configuration;

	private readonly IWebHostEnvironment webHostEnvironment;

	/// <summary>
	/// Initializes a new instance of the <see cref="Startup"/> class.
	/// </summary>
	/// <param name="configuration">The application configuration, where key value pair settings are stored. See
	/// http://docs.asp.net/en/latest/fundamentals/configuration.html</param>
	/// <param name="webHostEnvironment">The environment the application is running under. This can be Development,
	/// Staging or Production by default. See http://docs.asp.net/en/latest/fundamentals/environments.html</param>
	public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
	{
		this.configuration = configuration;
		this.webHostEnvironment = webHostEnvironment;
	}

	/// <summary>
	/// Configures the services to add to the ASP.NET Core Injection of Control (IoC) container. This method gets
	/// called by the ASP.NET runtime. See
	/// http://blogs.msdn.com/b/webdev/archive/2014/06/17/dependency-injection-in-asp-net-vnext.aspx
	/// </summary>
	/// <param name="services">The services.</param>
	public void ConfigureServices(IServiceCollection services)
	{
		_ = services
			.AddDistributedMemoryCache()
			.AddResponseCompression()
			.AddRouting();
		_ = services.AddResponseCaching()
		.AddCustomHealthChecks()
		// .AddOpenTelemetryTracing(builder => builder.AddCustomTracing(this.webHostEnvironment))
		.AddSwaggerGen()
		.AddHttpContextAccessor()
		// Add useful interface for accessing the ActionContext outside a controller.
		.AddSingleton<IActionContextAccessor, ActionContextAccessor>()
		.AddServerTiming();
		_ = services
		.AddControllers();
		_ = services
		.AddCustomOptions(this.configuration);
		_ = services
		.AddCustomConfigureOptions();
		_ = services
		.AddProjectCommands()
		.AddProjectMappers()
		.AddProjectRepositories()
		.AddProjectServices();
	}

	/// <summary>
	/// Configures the application and HTTP request pipeline. Configure is called after ConfigureServices is
	/// called by the ASP.NET runtime.
	/// </summary>
	/// <param name="application">The application builder.</param>
	public void Configure(IApplicationBuilder application)
	{
		if (this.webHostEnvironment.IsDevelopment())
		{
			_ = application.UseServerTiming()
			.UseSwaggerUI()
			.UseDeveloperExceptionPage();

		}
		_ = application
			.UseForwardedHeaders()
			.UseRouting()
			.UseResponseCaching()
			.UseResponseCompression()
			.UseStaticFiles()
			.UseSerilogRequestLogging()
			.UseRequestCanceled()
			.UseEndpoints(
				builder =>
				{
					_ = builder.MapControllers();
					_ = builder
						.MapHealthChecks("/status");
					_ = builder
						.MapHealthChecks("/status/self", new HealthCheckOptions() { Predicate = _ => false });
					if (this.webHostEnvironment.IsDevelopment())
					{
						_ = builder.MapSwagger();

					}
				})
			.UseIf(
				this.webHostEnvironment.IsDevelopment(),
				x => x.UseSwaggerUI());
	}
}
