using System.Diagnostics.CodeAnalysis;
using AdvancedStructureSkins.API;
using AdvancedStructureSkins.Util;
using HarmonyLib;
using Il2CppRUMBLE.MoveSystem;
using MelonLoader.Utils;
using RumbleModdingAPI.RMAPI;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace AdvancedStructureSkins.Skins;

[SuppressMessage("ReSharper", "Unity.PreferAddressByIdToGraphicsParams")]
public static class SkinHandler
{ 
    private static readonly Dictionary<string, int> Types = new()
    {
        { "Disc", 0 },
        { "Pillar", 1 },
        { "Ball", 2 },
        { "RockCube", 3 },
        { "Wall", 4 },
        { "SmallRock", 5 },
        { "LargeRock", 6 },
        
        { "StructureTarget", 0 },
        { "DockedDisk", 0 },
        { "PrisonedPillar", 1 },
        { "BoulderBall", 2 },
        { "CageCube", 3 },
        { "WrappedWall", 4 }
    };

    private static readonly string[] TEXProperties =
    {
        "Texture2D_2058E65A",
        "Texture2D_3812B1EC",
        "Texture2D_8F187FEF",
        "_Grounded_noise"
    };

    private static readonly string[] TEXTypes =
    {
        "Normal",
        "Main",
        "Mat",
        "Grounded"
    };

    private static readonly string[] TEXPaths =
    {
        "Skins/Disc/", 
        "Skins/Pillar/", 
        "Skins/Ball/", 
        "Skins/Cube/", 
        "Skins/Wall/", 
        "Skins/SmallRock/", 
        "Skins/LargeRock/" 
    };

    private static List<SkinTextures>[] _textures = new List<SkinTextures>[7];

    public static void Init()
    {
        ReloadTexturesFromFile();
        ASS.LogVerbose("SkinHandler Initialized");
    }

    public static void ReloadTexturesFromFile()
    {
        _textures = new List<SkinTextures>[7];
        for (int i = 0; i < _textures.Length; i++)
        {
            _textures[i] = new();
            
            string path = Path.Combine(MelonEnvironment.UserDataDirectory, TEXPaths[i]);

            if (!Directory.Exists(path)) continue;
            
            string[] skins = Directory.GetDirectories(path);

            if (skins.Length <= 0 | IsMultiTextureSingleSkin(skins))
            {
                if (Path.GetFileName(path).StartsWith("_")) continue;
                
                _textures[i].Add(new SkinTextures(path));
                continue;
            }
            
            foreach (string skin in skins)
            {
                if (Path.GetFileName(skin).StartsWith("_")) continue;
                
                _textures[i].Add(new SkinTextures(skin));
            }
        }
    }

    private static bool IsMultiTextureSingleSkin(string[] paths)
    {
        foreach (string path in paths)
        {
            if (!TEXTypes.Contains(Path.GetFileName(path), StringComparer.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }

    public static void ApplySkinTo(Structure structure)
    {
        if (!Types.Keys.Contains(structure.gameObject.name))
        {
            ASS.Warn("Found unsupported structure with name '" +  structure.gameObject.name + "'");
            return;
        }
        
        MeshRenderer mr = structure.meshRenderer;
        AdvancedSkin advancedSkin = structure.GetComponent<AdvancedSkin>();
        int type = Types[structure.gameObject.name];
        
        if (mr == null)
        {
            ASS.Error("Could not find MeshRenderer on structure!");
            return;
        }
        
        /*if (!CustomShaders.IsInAnyCache("default_" + Types.Keys.ElementAt(type)))
        {
            Material clone = new Material(mr.material) { hideFlags = HideFlags.DontUnloadUnusedAsset };
            clone.SetTexture("Texture2D_8F187FEF", Texture2D.blackTexture);
            
            CustomShaders.AddToCache("default_" + Types.Keys.ElementAt(type), clone.shader, clone);
        }*/

        AssShader[] shaders = AssAPI.GetShadersFor(type);
        if (shaders.Length <= 0)
            return;
        
        AssShader shader = shaders[ASS.Random.Next(0, shaders.Length)];

        // ApplyMeshTo(advancedSkin, type);
        //ApplyTexturesTo(advancedSkin, type);
        ApplyShaderTo(advancedSkin, type, shader);
        ApplyOverridesTo(advancedSkin, type, shader);
    }

    /*private static void ApplyMeshTo(AdvancedSkin advancedSkin, int type)
    {
        bool useLegacy = false;
        if (useLegacy && advancedSkin.meshSkin == null)
        {
            advancedSkin.UseMeshObject(oldStructures);
        }

        advancedSkin.meshSkin?.SetActive(useLegacy);
    }*/

    private static void ApplyTexturesTo(AdvancedSkin advancedSkin, int type)
    {
        //Material defaultMat = CustomShaders.GetMaterial("default_" + Types.Keys.ElementAt(type));

        foreach (MeshRenderer mr in advancedSkin.GetRenderers())
        {
            MaterialPropertyBlock properties = new MaterialPropertyBlock();
            mr.GetPropertyBlock(properties);
        
            int textureIndex = ASS.Random.Next(_textures[type].Count);
            for (int i = 0; i < TEXProperties.Length; i++)
            {
                if (_textures[type].Count <= 0 || !_textures[type][textureIndex].HasTextureFor(i))
                {
                    //ApplyDefaultTexturesTo(properties, defaultMat, i);
                    continue;
                }
            
                Texture2D texture = _textures[type][textureIndex].GetTexture(i);
                properties.SetTexture(TEXProperties[i], texture);
            }
        
            if (_textures[type].Count > 0 && _textures[type][textureIndex].HasTextureFor(1))
            {
                properties.SetColor("Color_D943764B", Color.white);
            }
            else
            {
                properties.SetColor("Color_D943764B", new Color(176f/255f, 142f/255f, 115f/255f));
            }
        
            mr.SetPropertyBlock(properties);
        }
    }

    private static void ApplyDefaultTexturesTo(MaterialPropertyBlock properties, Material defaultMat, int textureIndex)
    {
        properties.SetTexture(TEXProperties[textureIndex], Texture2D.blackTexture);
        Texture defaultTex = defaultMat.GetTexture(TEXProperties[textureIndex]);
        if (defaultTex == null) return;
        
        properties.SetTexture(TEXProperties[textureIndex], defaultTex);
    }

    // ReSharper disable once UnusedParameter.Local
    private static void ApplyShaderTo(AdvancedSkin advancedSkin, int type, AssShader shader)
    {
        if (shader.material == null)
        {
            ASS.WarnVerbose("Could not find material on AssShader, gave up executing ApplyShaderTo()");
            return;
        }
        
        Material newMat = new Material(shader.material);

        foreach (MeshRenderer mr in advancedSkin.GetRenderers())
        {
            newMat.CopyMatchingPropertiesFromMaterial(mr.material);
            mr.material = newMat;
        }
    }

    private static void ApplyOverridesTo(AdvancedSkin advancedSkin, int type, AssShader shader)
    {
        List<MaterialPropertyOverride> overrides = shader.overrides;
        if (overrides == null) return;

        foreach (MeshRenderer mr in advancedSkin.GetRenderers())
        {
            foreach (MaterialPropertyOverride propOver in overrides)
            {
                if (!propOver.targetStructures.Contains(type)) continue;

                switch ((ShaderPropertyType)propOver.propertyType)
                {
                    case ShaderPropertyType.Color:
                        mr.material.SetColor(propOver.propertyName, propOver.colorValue);
                        break;
                    case ShaderPropertyType.Vector:
                        mr.material.SetVector(propOver.propertyName, propOver.vectorValue);
                        break;
                    case ShaderPropertyType.Float:
                        mr.material.SetFloat(propOver.propertyName, propOver.floatValue);
                        break;
                    case ShaderPropertyType.Range:
                        mr.material.SetFloat(propOver.propertyName, propOver.floatValue);
                        break;
                    case ShaderPropertyType.Int:
                        mr.material.SetInt(propOver.propertyName, propOver.intValue);
                        break;
                }
            }
        }
    }

    public static string ConvertResourceName(string input)
    {
        Dictionary<string, string> resourceNameMap = new Dictionary<string, string>()
        {
            { "Disc", "Disc" },
            { "Pillar", "Pillar" },
            { "Ball", "Ball" },
            { "RockCube", "RockCube" },
            { "Wall", "Wall" },
            { "SmallRock", "SmallRock" },
            { "LargeRock", "LargeRock" },

            { "StructureTarget", "Disc" },
            { "DockedDisk", "Disc" },
            { "PrisonedPillar", "Pillar" },
            { "BoulderBall", "Ball" },
            { "CageCube", "RockCube" },
            { "WrappedWall", "Wall" }
        };
        
        return resourceNameMap[input];
    }
}