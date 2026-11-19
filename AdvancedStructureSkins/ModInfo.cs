using System.Reflection;

namespace AdvancedStructureSkins;

internal static class ModInfo
{
    public static string Name { get; }
    public static string Version { get; }
    public static string Author { get; }
    public static string Description { get; }
    public static bool Experimental { get; }

    static ModInfo()
    {
        var assembly = Assembly.GetExecutingAssembly();

        string GetMetatdata(string key) =>
            assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                .FirstOrDefault(x => x.Key == key)?.Value ?? string.Empty;
        
        Name = GetMetatdata("ModName");
        Version = GetMetatdata("ModVersion");
        Author = GetMetatdata("ModAuthor");
        Description = GetMetatdata("ModDescription");
        
        bool.TryParse(GetMetatdata("ExperimentalBuild"), out bool isExperimental);
        Experimental = isExperimental;
    }
}