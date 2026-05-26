using AdvancedStructureSkins.Shared.SDK;
using AdvancedStructureSkins.Skins;
using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.API;

[Serializable]
public class AssShader
{
    public readonly string name;
    public Texture previewTexture;
    public List<ShaderManifest> shaders = new List<ShaderManifest>();
    public byte enabledForStructure;
    public bool initialized = false;
    public byte defaultShaderFor = 0;
    public bool allowedInComp = true;

    public AssShader(SkinManifest manifest)
    {
        name = manifest.skinName;
        enabledForStructure = (byte)StructureFlags.None;
        previewTexture = manifest.previewTexture;
        shaders = manifest.shaders;
        allowedInComp = manifest.allowedInComp;
        initialized = true;
    }
    
    public AssShader(string name, byte defaultFor)
    {
        this.name = name;
        enabledForStructure = (byte)StructureFlags.None;
        shaders.Add(new ShaderManifest { overrides = new List<MaterialPropertyOverride>() });
        defaultShaderFor = defaultFor;
    }
    
    public AssShader(string name, AssetBundle bundle)
    {
        foreach (var item in bundle.AllAssetNames())
        {
            ASS.LogVerbose(item);
        }
        
        this.name = name;
        enabledForStructure = (byte)StructureFlags.None;
        
        var data = bundle.LoadAsset<TextAsset>("overrides");
        shaders = new List<ShaderManifest>
        {
            new ShaderManifest
            {
                material = bundle.LoadAsset<Material>("material"),
                overrides = ReadDataFile(data != null ? data.text : "").ToList()
            }
        };
        
        shaders[0].material.hideFlags = HideFlags.DontUnloadUnusedAsset;
        
        initialized = true;
    }
    
    private static List<MaterialPropertyOverride> ReadDataFile(string data)
    {
        List<MaterialPropertyOverride> result = new();

        if (string.IsNullOrEmpty(data))
            return result;
        
        var split = data.Split("|");
        
        var processed = 0;
        while (processed < split.Length - 1)
        {
            MaterialPropertyOverride o = new()
            {
                propertyName = split[processed]
            };
            processed++;
            o.propertyType = (ShaderPropertyType)int.Parse(split[processed]);
            processed++;

            switch ((int)o.propertyType)
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
                o.targetStructures |= (StructureFlags)(1 << int.Parse(split[processed]));
                processed++;
            }
            result.Add(o);
        }

        return result;
    }
}