using AdvancedStructureSkins.Shared.SDK;
using AdvancedStructureSkins.Skins;
using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.API;

[Serializable]
public class AssShader
{
    public readonly string name;
    public Material material;
    public Texture previewTexture;
    public readonly List<MaterialPropertyOverride> overrides;
    public byte enabledForStructure;
    public bool initialized = false;
    public byte defaultShaderFor = 0;
    public bool allowedInComp = true;

    public AssShader(SkinManifest manifest)
    {
        name = manifest.skinName;
        enabledForStructure = (byte)StructureFlags.None;
        material = manifest.material;
        material.hideFlags = HideFlags.DontUnloadUnusedAsset;
        previewTexture = manifest.previewTexture;
        overrides = manifest.overrides;
        allowedInComp = manifest.allowedInComp;
        initialized = true;
    }
    
    public AssShader(string name, byte defaultFor)
    {
        this.name = name;
        enabledForStructure = (byte)StructureFlags.None;
        overrides = new List<MaterialPropertyOverride>();
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
        material = bundle.LoadAsset<Material>("material");
        material.hideFlags = HideFlags.DontUnloadUnusedAsset;
        
        var data = bundle.LoadAsset<TextAsset>("overrides");
        overrides = ReadDataFile(data != null ? data.text : "").ToList();

        initialized = true;
    }
    
    private static MaterialPropertyOverride[] ReadDataFile(string data)
    {
        List<MaterialPropertyOverride> result = new();

        if (string.IsNullOrEmpty(data))
            return result.ToArray();
        
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

        return result.ToArray();
    }
}