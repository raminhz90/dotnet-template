using Microsoft.AspNetCore.Mvc;

namespace Webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
	public WeatherForecastController(ILogger<WeatherForecastController> logger)
	{
		this._logger = logger;
	}
	private static readonly string[] Summaries = {
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	};
	private readonly ILogger<WeatherForecastController> _logger;

	[HttpGet(Name = "GetWeatherForecast")]
	public async Task<IEnumerable<WeatherForecast>> Get()
	{
		this._logger.LogInformation("GetWeatherForecast called");
		var waitTime = Random.Shared.Next(1, 500);
		await Task.Delay(TimeSpan.FromMilliseconds(waitTime)).ConfigureAwait(false);
		return Enumerable.Range(1, 5).Select(index => new WeatherForecast
		{
			Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
			TemperatureC = Random.Shared.Next(-20, 55),
			Summary = Summaries[Random.Shared.Next(Summaries.Length)]
		})
			.ToArray();
	}
}
