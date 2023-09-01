using logger;
using Serilog;
using Webapi.HostConfigurations;
using Webapi.Utils;

Log.Logger = CustomLogger.CreateBootstrapLogger();
try
{
	Log.Information("Starting up");
	var builder = new HostBuilder();
	builder.UseContentRoot(Directory.GetCurrentDirectory())
			.ConfigureAppConfiguration(
				(hostingContext, configurationBuilder) =>
				{
					hostingContext.HostingEnvironment.ApplicationName = AssemblyInformation.Current.Product;
					configurationBuilder.AddCustomConfiguration(hostingContext.HostingEnvironment, args);
				})

}
catch (Exception exception)
{
	CustomLogger.LogApplicationTerminatedUnexpectedly(exception);
}
finally
{
	Log.CloseAndFlush();
}

// // Add services to the container.

// builder.Services.AddControllers();
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
// 	_ = app.UseSwagger();
// 	_ = app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();

// app.UseAuthorization();

// app.MapControllers();

// app.Run();
