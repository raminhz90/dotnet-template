using System.Reflection;

namespace Webapi.Utils;
public record struct AssemblyInformation(string Product, string Description, string Version)
{
	public AssemblyInformation(Assembly assembly)
		: this(
			assembly.GetCustomAttribute<AssemblyProductAttribute>()!.Product,
			assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()!.Description,
			assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()!.Version)
	{
	}
	public static readonly AssemblyInformation Current = new(typeof(AssemblyInformation).Assembly);

}
