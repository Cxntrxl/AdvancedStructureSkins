using AdvancedStructureSkins.Skins;
using MelonLoader.Utils;
using RumbleModdingAPI.RMAPI;
using UnityEngine;

namespace AdvancedStructureSkins.API;

public static class CustomShaders
{
    private static readonly Dictionary<string, string> CachedFilePaths = new();
    private static readonly Dictionary<string, Shader> CachedShaders = new();
    private static readonly Dictionary<string, Material> CachedMaterials = new();
    private static readonly Dictionary<string, List<MaterialPropertyOverride>> CachedOverrides = new();
    private static readonly Dictionary<string, AssetBundle> CachedBundles = new();

    public static Shader GetShader(string bundleName)
    {
        if (CachedShaders.TryGetValue(bundleName, out Shader existingShader) && existingShader != null)
            return existingShader;
        
        if (!CachedBundles.TryGetValue(bundleName, out AssetBundle bundle) || bundle == null)
        {
            string path = Path.Combine(MelonEnvironment.UserDataDirectory, "Skins", bundleName + ".bundle");

            if (!File.Exists(path))
            {
                path = Path.Combine(MelonEnvironment.UserDataDirectory, "Skins", bundleName);

                if (!File.Exists(path))
                {
                    ASS.Warn($"Shader bundle not found at path: {path}");
                    return null;
                }
            }

            bundle = AssetBundles.LoadAssetBundleFromFile(path);
            if (bundle == null)
            {
                ASS.Warn($"Failed to load AssetBundle from: {path}");
                return null;
            }

            CachedBundles[bundleName] = bundle;
            CachedFilePaths[bundleName] = path;
        }
        
        Shader shader = bundle.LoadAsset<Shader>("shader");
        if (shader == null)
        {
            ASS.Warn($"Shader asset not found in bundle: {bundleName}");
            return null;
        }

        CachedShaders[bundleName] = shader;
        return shader;
    }
    
    public static Material GetMaterial(string bundleName)
    {
        if (CachedMaterials.TryGetValue(bundleName, out Material existingMaterial) && existingMaterial != null)
            return new Material(existingMaterial);
        
        if (!CachedBundles.TryGetValue(bundleName, out AssetBundle bundle) || bundle == null)
        {
            string path = Path.Combine(MelonEnvironment.UserDataDirectory, "Skins", bundleName + ".bundle");

            if (!File.Exists(path))
            {
                path = Path.Combine(MelonEnvironment.UserDataDirectory, "Skins", bundleName);

                if (!File.Exists(path))
                {
                    ASS.Warn($"Shader bundle not found at path: {path}");
                    return null;
                }
            }

            bundle = AssetBundles.LoadAssetBundleFromFile(path);
            if (bundle == null)
            {
                ASS.Warn($"Failed to load AssetBundle from: {path}");
                return null;
            }

            CachedBundles[bundleName] = bundle;
            CachedFilePaths[bundleName] = path;
        }
        
        Material material = bundle.LoadAsset<Material>("material");
        if (material == null)
        {
            ASS.Warn($"Material asset not found in bundle: {bundleName}");
            return null;
        }

        CachedMaterials[bundleName] = material;
        return new Material(material);
    }
    
    public static List<MaterialPropertyOverride> GetOverrides(string bundleName)
    {
        if (CachedOverrides.TryGetValue(bundleName, out List<MaterialPropertyOverride> existingOverrides) && existingOverrides != null)
            return existingOverrides;
        
        if (!CachedBundles.TryGetValue(bundleName, out AssetBundle bundle) || bundle == null)
        {
            string path = Path.Combine(MelonEnvironment.UserDataDirectory, "Skins", bundleName + ".bundle");
            
            if (!File.Exists(path))
            {
                path = Path.Combine(MelonEnvironment.UserDataDirectory, "Skins", bundleName);

                if (!File.Exists(path))
                {
                    ASS.Warn($"Shader bundle not found at path: {path}");
                    return null;
                }
            }

            bundle = AssetBundles.LoadAssetBundleFromFile(path);
            if (bundle == null)
            {
                ASS.Warn($"Failed to load AssetBundle from: {path}");
                return null;
            }
            
            CachedBundles[bundleName] = bundle;
            CachedFilePaths[bundleName] = path;
        }
        
        TextAsset data = bundle.LoadAsset<TextAsset>("overrides");

        if (data == null)
        {
            ASS.Warn($"Failed to load overrides from: {bundleName}");
        }
        
        List<MaterialPropertyOverride> overrides = ReadDataFile(data.text).overrides;
        if (overrides == null)
        {
            ASS.Warn($"Material asset not found in bundle: {bundleName}");
            return null;
        }

        CachedOverrides[bundleName] = overrides;
        return overrides;
    }

    private static MaterialPropertyOverrides ReadDataFile(string data)
    {
        string[] split = data.Split("|");
        MaterialPropertyOverrides result = new MaterialPropertyOverrides();
        
        int processed = 0;
        while (processed < split.Length - 1)
        {
            MaterialPropertyOverride o = new()
            {
                propertyName = split[processed]
            };
            processed++;
            o.propertyType = int.Parse(split[processed]);
            processed++;

            switch (o.propertyType)
            {
                case 0:
                    o.colorValue.r = float.Parse(split[processed]);
                    o.colorValue.g = float.Parse(split[processed + 1]);
                    o.colorValue.b = float.Parse(split[processed + 2]);
                    o.colorValue.a = float.Parse(split[processed + 3]);
                    processed += 4;
                    break;
                case 1:
                    o.vectorValue.x = float.Parse(split[processed]);
                    o.vectorValue.y = float.Parse(split[processed + 1]);
                    o.vectorValue.z = float.Parse(split[processed + 2]);
                    o.vectorValue.w = float.Parse(split[processed + 3]);
                    processed += 4;
                    break;
                case 2:
                    o.floatValue = float.Parse(split[processed]);
                    processed++;
                    break;
                case 3:
                    o.floatValue = float.Parse(split[processed]);
                    processed++;
                    break;
                case 4:
                    ASS.Error("What? How did you even build a texture in your overrides? Dude, this feature isn't even implemented yet..");
                    break;
                case 5:
                    o.intValue = int.Parse(split[processed]);
                    processed++;
                    break;
            }
            
            int numTargets = int.Parse(split[processed]);
            processed++;

            for (int i = 0; i < numTargets; i++)
            {
                o.targetStructures.Add(int.Parse(split[processed]));
                processed++;
            }
            result.overrides.Add(o);
        }

        return result;
    }

    public static void AddToCache(string bundleName, Shader shader = null, Material material = null)
    {
        if (shader == null && material == null)
        {
            ASS.Warn("Please specify a shader or material to add to cache.");
            return;
        }

        if (IsInShaderCache(bundleName))
        {
            ASS.Warn("AddToCache Shader already exists in cache!");
        }
        else if (shader != null)
        {
            CachedShaders[bundleName] = shader;
        }

        if (IsInMaterialCache(bundleName))
        {
            ASS.Warn("AddToCache Material already exists in cache!");
        }
        else if (material != null)
        {
            CachedMaterials[bundleName] = material;
        }
    }

    public static bool IsInAnyCache(string bundleName)
    {
        bool inShaderCache = CachedShaders.TryGetValue(bundleName, out Shader _);
        bool inMaterialCache = CachedMaterials.TryGetValue(bundleName, out Material _);
        return inShaderCache || inMaterialCache;
    }

    private static bool IsInShaderCache(string bundleName)
    {
        return CachedShaders.TryGetValue(bundleName, out Shader _);
    }

    private static bool IsInMaterialCache(string bundleName)
    {
        return CachedMaterials.TryGetValue(bundleName, out Material _);
    }

    public static void ClearCache(bool keepDefaults = true)
    {
        Dictionary<string, Shader> shaderDefaults = new Dictionary<string, Shader>();
        Dictionary<string, Material> materialDefaults = new Dictionary<string, Material>();

        foreach (string key in CachedShaders.Keys)
        {
            if (!key.Contains("default_")) continue;
            shaderDefaults.Add(key, CachedShaders[key]);
        }

        foreach (string key in CachedMaterials.Keys)
        {
            if (!key.Contains("default_")) continue;
            materialDefaults.Add(key, CachedMaterials[key]);
        }
        
        CachedFilePaths.Clear();
        CachedShaders.Clear();
        CachedMaterials.Clear();
        CachedOverrides.Clear();
        CachedBundles.Clear();

        if (keepDefaults)
        {
            foreach (string key in shaderDefaults.Keys)
            {
                CachedShaders.Add(key, shaderDefaults[key]);
            }

            foreach (string key in materialDefaults.Keys)
            {
                CachedMaterials.Add(key, materialDefaults[key]);
            }
        }
    }

    public static void LogCache()
    {
        string info = "Cached Bundles";
        foreach (string bundleName in CachedFilePaths.Keys)
        {
            info += "\nFile Path: " + CachedFilePaths[bundleName];
            info += "\nBundle name: " + bundleName;
            info += "\nBundle Contents: ";
            foreach (var name in CachedBundles[bundleName].GetAllAssetNames())
            {
                info += "\n";
                info += name;
            }

            info += "\n\nLoaded Shader: ";
            info += CachedShaders[bundleName].name;
        }
        
        ASS.Log(info);
    }
}