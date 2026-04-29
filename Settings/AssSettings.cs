using AdvancedStructureSkins.Skins;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppNewtonsoft.Json;
using MelonLoader.Utils;
using RumbleModdingAPI.RMAPI;
using UnityEngine;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AdvancedStructureSkins.Settings;

public static class AssSettings
{
    public static bool[] shaderGroups = new bool[7];
    public static List<AssShader> shaders;
    private static string _skinDirectory = "/UserData/Skins/";
    
    public static void Init()
    {
        _skinDirectory = Path.Combine(MelonEnvironment.UserDataDirectory, "Skins");
        foreach (string path in GetSkinNames())
        {
            shaders.Add(LoadSkinFromFile(path));
        }
        
        Deserialize(UIHandler.ShaderSettings.Value);
    }

    public static string Serialize()
    {
        var lines = new List<string>();

        lines.Add("[Shader Groups]");
        for (int i = 0; i < shaderGroups.Length; i++)
        {
            lines.Add($"{i}={shaderGroups[i].ToString().ToLower()}");
        }
        lines.Add("");
        
        lines.Add("[Shaders]");
        foreach (var shader in shaders)
        {
            lines.Add($"{shader.name}={shader.flags.ToString()}");
        }
        
        return string.Join("\n", lines);
    }

    public static void Deserialize(string text)
    {
        var lookup = shaders.ToDictionary(x => x.name, x => x);
        string currentSection = "";
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
            
            int index = line.IndexOf('=');
            if (index <= 0)
                continue;

            string key = line.Substring(0, index).Trim();
            string value = line.Substring(index + 1).Trim();

            switch (currentSection)
            {
                case "Shader Groups":
                    shaderGroups[int.Parse(key)] = bool.Parse(value);
                    break;
                case "Shaders":
                    if (lookup.TryGetValue(key, out var shader))
                        shader.flags = byte.Parse(value);
                    break;
            }
        }
    }

    public static string[] GetSkinNames()
    {
        if (!Directory.Exists(_skinDirectory))
        {
            Directory.CreateDirectory(_skinDirectory);
            return Array.Empty<string>();
        }

        return Directory
            .GetFiles(_skinDirectory, "*.asb", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .ToArray();
    }

    public static AssShader LoadSkinFromFile(string path)
    {
        string fullPath = Path.Combine(_skinDirectory, path);
        if (!File.Exists(fullPath))
        {
            ASS.Error("Skin loading failed attempting to load a skin that doesn't exist.. how??");
        }

        string skinName = Path.GetFileNameWithoutExtension(path);
        AssShader shader = new(skinName, (byte)ShaderFlags.None, GetBundleFromFile(path));
        
        return shader;
    }

    public static AssetBundle GetBundleFromFile(string skinName)
    {
        string path = Path.Combine(skinName + ".asb");
        
        if (!File.Exists(path))
        {
            ASS.Warn($"Shader bundle not found at path: {path}");
            return null;
        }

        return AssetBundles.LoadAssetBundleFromFile(path);
    }

    public static byte GetStructureFlag(int index)
    {
        return (byte)(1 << (7 - index));
    }

    public static AssShader[] GetShadersFor(int type)
    {
        List<AssShader> structureShaders = new();
        List<AssShader> globalShaders = new();

        foreach (AssShader s in shaders)
        {
            if ((s.flags & GetStructureFlag(type)) != 0)
                structureShaders.Add(s);
            
            if ((s.flags & GetStructureFlag(7)) != 0)
                globalShaders.Add(s);
        }
        
        return structureShaders.Count > 0 ? structureShaders.ToArray() : globalShaders.ToArray();;
    }
}

public class AssShader
{
    public string name;
    public Shader shader;
    public Material material;
    public List<MaterialPropertyOverride> overrides;
    public byte flags;

    public AssShader(string name, byte flags, AssetBundle bundle)
    {
        this.name = name;
        this.flags = flags;
        shader = bundle.LoadAsset<Shader>("shader");
        material = bundle.LoadAsset<Material>("material");
        
        TextAsset data = bundle.LoadAsset<TextAsset>("overrides");

        if (data == null)
            data = new();

        overrides = ReadDataFile(data.text).overrides;
    }
    
    private static MaterialPropertyOverrides ReadDataFile(string data)
    {
        MaterialPropertyOverrides result = new MaterialPropertyOverrides();

        if (string.IsNullOrEmpty(data))
            return result;
        
        string[] split = data.Split("|");
        
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
}

[System.Flags]
public enum ShaderFlags : byte
{
    None = 0,
    
    Disc = 1 << 0,
    Pillar = 1 << 1,
    Ball = 1 << 2,
    Cube = 1 << 3,
    Wall = 1 << 4,
    SmallRock = 1 << 5,
    LargeRock = 1 << 6,
    
    Global = 1 << 7
}

// PLANNING:
// 
// Load ALL .asb
// Cache .asb data
// Populate settings object with skin toggles
// Serialization + Deserialization w/ json
// Hook UI to reflect settings changes
