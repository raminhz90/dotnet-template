
using System.Globalization;

namespace Webapi;


public class TempService : BackgroundService
{
	private readonly Dictionary<string,int> cache = new();
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await Task.Yield();
		while (!stoppingToken.IsCancellationRequested)
		{
			await NewMethod(stoppingToken).ConfigureAwait(false);
		}
	}

	private async Task NewMethod(CancellationToken cancellationToken)
	{
		var random = new Random();
		var waitTime = random.Next(10,300);
		await Task.Delay(TimeSpan.FromMilliseconds(waitTime), cancellationToken).ConfigureAwait(false);
		var key = random.Next(1, 100000).ToString(CultureInfo.InvariantCulture);
		if (this.cache.TryGetValue(key, out var value))
		{
			this.cache[key] = value + 1;
		}
		else
		{
			this.cache.Add(key, 1);
		}
	}
}
