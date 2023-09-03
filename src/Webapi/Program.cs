using logger;
using Serilog;
using Webapi.HostConfigurations;
using Webapi.Options;
using Webapi.Utils;

Log.Logger = CustomLogger.CreateBootstrapLogger();
try
{
	Log.Information("Starting up");
	IHostBuilder builder = new HostBuilder();
	_ = builder.UseContentRoot(Directory.GetCurrentDirectory())
			.ConfigureAppConfiguration(
				(hostingContext, configurationBuilder) =>
				{
					hostingContext.HostingEnvironment.ApplicationName = AssemblyInformation.Current.Product;
					_ = configurationBuilder.AddCustomConfiguration(hostingContext.HostingEnvironment, args);
				})
			.UseSerilog(CustomLogger.ConfigureReloadableLogger)
			.UseDefaultServiceProvider(
				(context, options) =>
				{
					var isDevelopment = context.HostingEnvironment.IsDevelopment();
					options.ValidateScopes = isDevelopment;
					options.ValidateOnBuild = isDevelopment;
				})
				.ConfigureWebHost(ConfigureWebHostBuilder)
			.UseConsoleLifetime();
	var host = builder.Build();
	await host.RunAsync().ConfigureAwait(false);

}
catch (Exception exception)
{
	CustomLogger.LogApplicationTerminatedUnexpectedly(exception);
}
finally
{
	await Log.CloseAndFlushAsync().ConfigureAwait(false);
}


static void ConfigureWebHostBuilder(IWebHostBuilder webHostBuilder) =>
	   webHostBuilder
		   .UseKestrel(
			   (builderContext, options) =>
			   {
				   options.AddServerHeader = false;
				   _ = options.Configure(
					   builderContext.Configuration.GetRequiredSection(nameof(ApplicationOptions.Kestrel)),
					   reloadOnChange: false);
			   })
		   // Used for IIS and IIS Express for in-process hosting. Use UseIISIntegration for out-of-process hosting.
		   .UseIIS()
		   .UseStartup<Startup>();
