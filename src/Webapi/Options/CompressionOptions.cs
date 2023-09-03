using System.ComponentModel.DataAnnotations;

namespace Webapi.Options;

public class CompressionOptions
{
	/// <summary>
	/// Gets a list of MIME types to be compressed in addition to the default set used by ASP.NET Core.
	/// </summary>
	[Required]
	public IReadOnlyCollection<string> MimeTypes { get; } = Array.Empty<string>();
}
