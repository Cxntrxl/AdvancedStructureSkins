using AdvancedStructureSkins.Shared.SDK;
using AdvancedStructureSkins.Shared.SDK.Binary;
using AdvancedStructureSkins.Skins;
using AdvancedStructureSkins.UI;
using AdvancedStructureSkins.Util;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppNewtonsoft.Json;
using MelonLoader.Utils;
using RumbleModdingAPI.RMAPI;
using UnityEngine;
using UnityEngine.Playables;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AdvancedStructureSkins.API;

public static class AssAPI
{
    internal static byte ShaderGroups;
    internal static readonly List<AssShader> Shaders = new();
    private static string _skinDirectory = Path.Combine(MelonEnvironment.UserDataDirectory, "Skins");

    internal static readonly List<AssTextureSet> TextureSets = new();
    public static readonly Dictionary<StructureFlags, List<AssTextureSet>> TextureSetsByTarget = new();
    private static readonly Dictionary<string, StructureFlags> StructureFlagMap = new(StringComparer.OrdinalIgnoreCase) {
            { "Disc", StructureFlags.Disc },
            { "Pillar", StructureFlags.Pillar },
            { "Ball", StructureFlags.Ball },
            { "Cube", StructureFlags.Cube },
            { "Wall", StructureFlags.Wall },
            { "SmallRock", StructureFlags.SmallRock },
            { "LargeRock", StructureFlags.LargeRock }
        };
    
    public static void Init()
    {
        ASS.LogVerbose("Initializing AssAPI");
        
        _skinDirectory = Path.Combine(MelonEnvironment.UserDataDirectory, "Skins");
        
        LoadTextureSets();
        LoadAdvancedSkinBundles();
        
        CreatePlaceholderDefaultShaders();
        CreatePlaceholderDefaultTextures();
        
        BuildTextureCache();
        
        ASS.LogVerbose("AssAPI Initialized");
    }

    private static void LoadAdvancedSkinBundles()
    {
        var skins = GetSkinNames();
        int loadedBundles = 0;
        foreach (var path in skins)
        {
            var fullPath = Path.Combine(_skinDirectory, path);
            if (!File.Exists(fullPath))
            {
                ASS.Error("Skin loading failed attempting to load a skin that doesn't exist.. how??");
                continue;
            }

            AssetBundle bundle = AssetBundles.LoadAssetBundleFromFile(fullPath);

            if (bundle == null)
            {
                ASS.Error($"Failed to load .asb: {fullPath}");
                continue;
            }

            string manifestPath = bundle.GetAllAssetNames()
                .FirstOrDefault(x => x.ToLower().Contains("manifest"));

            if (manifestPath == null)
            {
                var shader = LoadShaderFromBundle(bundle);
                if (shader == null)
                {
                    ASS.ErrorVerbose($"[Invalid Bundle] Failed to load .asb at: {fullPath}");
                    continue;
                }
                Shaders.Add(shader);
            }
            else
            {
                TextAsset binary = bundle.LoadAsset<TextAsset>(manifestPath);
                if (binary == null)
                {
                    ASS.ErrorVerbose($"[No Manifest] Failed to load .asb at {fullPath}");
                    continue;
                }

                try
                {
                    SkinManifestBinary referenceManifest = BinaryHandler.Read(binary.bytes);
                    SkinManifest manifest = referenceManifest.GetSkinManifestFromBundle(bundle);

                    Shaders.Add(LoadShaderFromManifest(manifest));
                    TextureSets.AddRange(LoadTextureSetsFromManifest(manifest));
                }
                catch (Exception ex)
                {
                    ASS.Error($"Failed to load {Path.GetFileName(fullPath)}: {ex.Message}");
                    loadedBundles--;
                }
            }

            loadedBundles++;
        }
        ASS.LogVerbose(loadedBundles + " .asb Files Loaded");
    }
    
    private static string[] GetSkinNames()
    {
        if (Directory.Exists(_skinDirectory))
            return Directory.GetFiles(_skinDirectory, "*.*", SearchOption.TopDirectoryOnly)
                .Where(f =>
                    f.EndsWith(".bundle", StringComparison.OrdinalIgnoreCase) ||
                    f.EndsWith(".asb", StringComparison.OrdinalIgnoreCase))
                .Select(Path.GetFileName)
                .ToArray();
        
        Directory.CreateDirectory(_skinDirectory);
        return Array.Empty<string>();
    }

    #region UI
    
    internal static string Serialize()
    {
        // ReSharper disable once UseObjectOrCollectionInitializer
        var lines = new List<string>();
        
        lines.Add("[Shader Settings]");
        lines.Add($"Shader Groups={ShaderGroups.ToString()}");
        lines.Add("");
        
        lines.Add("[Shaders]");
        lines.AddRange(Shaders.Select(shader => $"{shader.name}={shader.enabledForStructure.ToString()}"));
        lines.Add("");
        
        lines.Add("[Textures]");
        lines.AddRange(TextureSets.Select(textures => $"{textures.name}={(byte)textures.enabledForStructure}"));
        lines.Add("");
        
        return string.Join("\n", lines);
    }

    private static void Deserialize(string text)
    {
        Shaders.RemoveAll(x => x == null);
        
        var lookup = Shaders
            .Where(x => x != null && !string.IsNullOrEmpty(x.name))
            .ToDictionary(x => x.name, x => x);
        var currentSection = "";
        var lines = text.Split('\n');

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();

            if (string.IsNullOrEmpty(line))
                continue;

            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                currentSection = line.Substring(1, line.Length - 2);
                continue;
            }
            
            var index = line.IndexOf('=');
            if (index <= 0)
                continue;

            var key = line[..index].Trim();
            var value = line[(index + 1)..].Trim();

            switch (currentSection)
            {
                case "Shader Settings":
                    switch (key)
                    {
                        case "Shader Groups":
                            ShaderGroups = byte.Parse(value);
                            break;
                    }
                    break;
                case "Shaders":
                    if (lookup.TryGetValue(key, out var shader))
                        shader.enabledForStructure = byte.Parse(value);
                    break;
                case "Textures":
                    var skin = TextureSets.FirstOrDefault(t => t.name == key);
                    if (skin == null) continue;
                    
                    skin.enabledForStructure = byte.Parse(value);
                    break;
            }
        }
    }

    public static string SyncUI(string text)
    {
        if (!string.IsNullOrEmpty(text))
            Deserialize(text);
        return Serialize();
    }

    #endregion
    
    #region Shaders

    private static AssShader LoadShaderFromBundle(AssetBundle bundle)
    {
        return bundle.GetAllAssetNames().Contains("material") ? new(Path.GetFileNameWithoutExtension(bundle.name), bundle) : null;
    }

    private static AssShader LoadShaderFromManifest(SkinManifest manifest)
    {
        return new(manifest);
    }

    public static byte GetStructureFlag(int index)
    {
        return index switch
        {
            0 => (byte)StructureFlags.Disc,
            1 => (byte)StructureFlags.Pillar,
            2 => (byte)StructureFlags.Ball,
            3 => (byte)StructureFlags.Cube,
            4 => (byte)StructureFlags.Wall,
            5 => (byte)StructureFlags.SmallRock,
            6 => (byte)StructureFlags.LargeRock,
            7 => (byte)StructureFlags.Global,
            _ => (byte)StructureFlags.None
        };
    }

    public static AssShader[] GetShadersFor(byte type)
    {
        AssShader[] result;
        if ((type & ShaderGroups) != 0)
        {
            result = Shaders.Where(shader => 
                    (shader.enabledForStructure & type) != 0 &&
                    (!UIHandler.CompMode.Value || shader.allowedInComp))
                .ToArray();
        }
        else
        {
            result = Shaders.Where(shader => 
                    (shader.enabledForStructure & (byte)StructureFlags.Global) != 0 &&
                    (!UIHandler.CompMode.Value || shader.allowedInComp))
                .ToArray();
        }

        if (result.Length > 0) return result;
        
        return new [] { GetDefaultShaderFor(type) };
    }

    // Default shaders are grabbed when a structure type is initially spawned.
    // This is fine in theory, but since player's UI selections are serialized from the shader list
    // at init, the default shaders must be in the shader list *at init*
    // as such, we create placeholder entries which are initialized correctly on summon
    // *before* they are applied to the structure.
    private static void CreatePlaceholderDefaultShaders()
    {
        foreach (StructureFlags flag in Enum.GetValues(
                     typeof(StructureFlags)))
        {
            if (flag is StructureFlags.None or StructureFlags.Global)
                continue;

            Shaders.Add(new AssShader($"Default_{flag}", (byte)flag));
        }
    }

    public static AssShader GetDefaultShaderFor(byte type)
    {
        return Shaders.FirstOrDefault(shader => (shader.defaultShaderFor & type) != 0);
    }
    
    #endregion
    
    #region Textures

    private static void LoadTextureSets()
    {
        TextureSets.Clear();

        if (!Directory.Exists(_skinDirectory))
            Directory.CreateDirectory(_skinDirectory);

        foreach (string structurePath in Directory.GetDirectories(_skinDirectory))
        {
            string structureName = Path.GetFileName(structurePath);

            if (!StructureFlagMap.TryGetValue(structureName, out StructureFlags target))
                continue;

            LoadStructureDirectory(structurePath, target);
        }

        ASS.Log($"Loaded {TextureSets.Count} texture sets.");
    }
    
    private static void LoadStructureDirectory(string structurePath, StructureFlags target)
    {
        if (AssTextureSet.IsSingleSkin(structurePath))
        {
            TextureSets.Add(
                new AssTextureSet(
                    structurePath,
                    (byte)target
                    )
                );
        }
        else if (AssTextureSet.IsMultiSkin(structurePath))
        {
            foreach (string dir in Directory.GetDirectories(structurePath))
            {
                TextureSets.Add(
                    new AssTextureSet (
                            dir,
                            (byte)target
                        )
                    );
            }
        }
    }
    
    private static void BuildTextureCache()
    {
        TextureSetsByTarget.Clear();

        foreach (StructureFlags flag in Enum.GetValues(
                     typeof(StructureFlags)))
        {
            if (flag is StructureFlags.None or StructureFlags.Global)
                continue;

            TextureSetsByTarget[flag] = new List<AssTextureSet>();
        }

        foreach (AssTextureSet set in TextureSets)
        {
            foreach (var targetStructure in TextureSetsByTarget.Keys.Where(targetFlag => (set.targets & (byte)targetFlag) != 0))
            {
                TextureSetsByTarget[targetStructure]
                    .Add(set);
            }
        }
    }

    public static AssTextureSet[] GetEnabledTextureSetsForStructure(StructureFlags structureType)
    {
        AssTextureSet[] enabled = TextureSetsByTarget[structureType].Where(set => (set.enabledForStructure & (byte)structureType) != 0).ToArray();
        if (enabled.Length > 0) return enabled;

        return TextureSetsByTarget[structureType].Where(set => set.isDefaultTextureSet).ToArray();
    }

    public static AssTextureSet GetDefaultTextureSetFor(StructureFlags structureType)
    {
        return TextureSetsByTarget[structureType].FirstOrDefault(set => set.isDefaultTextureSet);
    }
    
    // Default textures are grabbed when a structure type is initially spawned.
    // This is fine in theory, but since player's UI selections are serialized from the texture list
    // at init, the default textures must be in the texture list *at init*
    // as such, we create placeholder entries which are initialized correctly on summon
    // *before* they are applied to the structure.
    private static void CreatePlaceholderDefaultTextures()
    {
        foreach (StructureFlags flag in Enum.GetValues(
                     typeof(StructureFlags)))
        {
            if (flag is StructureFlags.None or StructureFlags.Global)
                continue;

            TextureSets.Add(new AssTextureSet($"Default_{flag}", (byte)flag, true));
        }
    }

    private static AssTextureSet[] LoadTextureSetsFromManifest(SkinManifest manifest)
    {
        return manifest.textures.Select(setManifest => new AssTextureSet(setManifest)).ToArray();
    } 
    
    #endregion
}
