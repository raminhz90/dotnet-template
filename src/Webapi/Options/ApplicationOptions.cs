using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Webapi.Options;


public class ApplicationOptions
{

	[Required]
	public Dictionary<string, CacheProfile> CacheProfiles { get; } = new();

	[Required]
	public CompressionOptions Compression { get; set; } = default!;

	[Required]
	public ForwardedHeadersOptions ForwardedHeaders { get; set; } = default!;

	[Required]
	public HostOptions Host { get; set; } = default!;

	[Required]
	public KestrelServerOptions Kestrel { get; set; } = default!;
}
