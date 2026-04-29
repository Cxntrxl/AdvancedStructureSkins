using AdvancedStructureSkins.Skins;
using AdvancedStructureSkins.Util;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppNewtonsoft.Json;
using MelonLoader.Utils;
using RumbleModdingAPI.RMAPI;
using UnityEngine;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AdvancedStructureSkins.API;

public static class AssAPI
{
    public static bool[] shaderGroups = new bool[7];
    public static List<AssShader> shaders = new();
    private static string _skinDirectory = "/UserData/Skins/";
    
    public static void Init()
    {
        ASS.LogVerbose("Initializing AssAPI");
        _skinDirectory = Path.Combine(MelonEnvironment.UserDataDirectory, "Skins");
        
        string[] skins = GetSkinNames();
        foreach (string path in skins)
        {
            shaders.Add(LoadSkinFromFile(path));
        }
        ASS.LogVerbose(skins.Length + " .asb Files Loaded");
        
        ASS.LogVerbose("AssAPI Initialized");
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

    public static string InitialUIUpdate(string text)
    {
        if (!string.IsNullOrEmpty(text))
            Deserialize(text);
        return Serialize();
    }

    public static string[] GetSkinNames()
    {
        if (!Directory.Exists(_skinDirectory))
        {
            Directory.CreateDirectory(_skinDirectory);
            return Array.Empty<string>();
        }

        return Directory
            .GetFiles(_skinDirectory, "*.bundle", SearchOption.TopDirectoryOnly)
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
        AssShader shader = new(skinName, (byte)ShaderFlags.None, GetBundleFromFile(fullPath));
        
        return shader;
    }

    public static AssetBundle GetBundleFromFile(string path)
    {
        if (!File.Exists(path))
        {
            ASS.Warn($"Shader bundle not found at path: {path}");
            return null;
        }

        return AssetBundles.LoadAssetBundleFromFile(path);
    }

    public static byte GetStructureFlag(int index)
    {
        return index switch
        {
            0 => (byte)ShaderFlags.Disc,
            1 => (byte)ShaderFlags.Pillar,
            2 => (byte)ShaderFlags.Ball,
            3 => (byte)ShaderFlags.Cube,
            4 => (byte)ShaderFlags.Wall,
            5 => (byte)ShaderFlags.SmallRock,
            6 => (byte)ShaderFlags.LargeRock,
            7 => (byte)ShaderFlags.Global,
            _ => (byte)ShaderFlags.None
        };
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
        foreach (var item in bundle.AllAssetNames())
        {
            ASS.LogVerbose(item);
        }
        
        this.name = name;
        this.flags = flags;
        shader = bundle.LoadAsset<Shader>("shader");
        shader.hideFlags = HideFlags.DontUnloadUnusedAsset;
        material = bundle.LoadAsset<Material>("material");
        material.hideFlags = HideFlags.DontUnloadUnusedAsset;
        
        TextAsset data = bundle.LoadAsset<TextAsset>("overrides");
        overrides = ReadDataFile(data != null ? data.text : "").overrides;
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
