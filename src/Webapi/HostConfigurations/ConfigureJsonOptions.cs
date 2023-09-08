using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Webapi.HostConfigurations;

public class ConfigureJsonOptions : IConfigureOptions<JsonOptions>
{
	private readonly IWebHostEnvironment webHostEnvironment;

	public ConfigureJsonOptions(IWebHostEnvironment webHostEnvironment) =>
		this.webHostEnvironment = webHostEnvironment;

	public void Configure(JsonOptions options)
	{
		var jsonSerializerOptions = options.JsonSerializerOptions;
		jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
		jsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
		// Pretty print the JSON in development for easier debugging.
		var v = this.webHostEnvironment.IsDevelopment();
		jsonSerializerOptions.WriteIndented = v;
	}
}
