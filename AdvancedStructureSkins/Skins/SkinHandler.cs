using System.Diagnostics.CodeAnalysis;
using AdvancedStructureSkins.API;
using AdvancedStructureSkins.Shared.SDK;
using Il2CppRUMBLE.MoveSystem;
using UnityEngine;
using UnityEngine.Rendering;

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
        "Texture2D_3812B1EC", // Main
        "Texture2D_2058E65A", // Normal
        "Texture2D_8F187FEF", // Mat
        "_Grounded_noise" // Ground
    };

    private static readonly string[] TEXTypes =
    {
        "Normal",
        "Main",
        "Mat",
        "Grounded"
    };

    public static void Init()
    {
        ASS.LogVerbose("SkinHandler Initialized");
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
        StructureFlags type = (StructureFlags)AssAPI.GetStructureFlag(Types[structure.gameObject.name]);
        
        if (mr == null)
        {
            ASS.Error("Could not find MeshRenderer on structure!");
            return;
        }

        TrySaveDefaults(advancedSkin, type);
        
        ApplyTexturesTo(advancedSkin, type);

        AssShader[] shaders = AssAPI.GetShadersFor((byte)type);
        if (shaders.Length <= 0)
            return;
        
        AssShader shader = shaders[ASS.Random.Next(0, shaders.Length)];
        
        ApplyShaderTo(advancedSkin, type, shader);
        ApplyOverridesTo(advancedSkin, type, shader);
    }

    private static void TrySaveDefaults(AdvancedSkin advancedSkin, StructureFlags type)
    {
        var mr = advancedSkin.MeshRenderer;
        var shader = AssAPI.GetDefaultShaderFor((byte)type);
        var textures = AssAPI.GetDefaultTextureSetFor(type);

        if (!shader.initialized)
        {
            shader.shaders[0].material = new Material(mr.material) { hideFlags = HideFlags.DontUnloadUnusedAsset };
            shader.initialized = true;
        }

        if (!textures.initialized)
        {
            foreach (TextureType texType in Enum.GetValues(typeof(TextureType)))
            {
                var tex = mr.material.GetTexture(TEXProperties[(int)texType]);

                if (tex == null)
                {
                    ASS.Warn($"Texture Property {TEXProperties[(int)texType]} doesn't exist.");
                    continue;
                }

                textures.AddExistingTexture(texType, tex);
            }

            textures.initialized = true;
        }
    }

    private static void ApplyTexturesTo(AdvancedSkin advancedSkin, StructureFlags type)
    {
        var skins = AssAPI.GetEnabledTextureSetsForStructure(type);
        if (skins.Length <= 0) return;
        
        AssTextureSet skin = skins[ASS.Random.Next(skins.Length)];

        MaterialPropertyBlock properties = new MaterialPropertyBlock();
        
        MeshRenderer mr = advancedSkin.MeshRenderer;
        mr.GetPropertyBlock(properties);
            
        for (int i = 0; i < TEXProperties.Length; i++)
        {
            Texture texture = skin.GetRandomTexture((TextureType)i);
            if (texture == null)
            {
                ApplyDefaultTexturesTo(properties, type, i);
                continue;
            }
                
            properties.SetTexture(TEXProperties[i], texture);
        }

        // Rumble's default move shader tints the structures.
        // This is fine by default, since the default textures are basically white
        // But we don't want to tint custom textures set by players.
        properties.SetColor("Color_D943764B",
            skin.isDefaultTextureSet ? new Color(176f / 255f, 142f / 255f, 115f / 255f) : Color.white);
            
        mr.SetPropertyBlock(properties);
        advancedSkin.currentTexture = skin;
    }

    private static void ApplyDefaultTexturesTo(MaterialPropertyBlock properties, StructureFlags type, int tIndex)
    {
        properties.SetTexture(TEXProperties[tIndex], Texture2D.blackTexture);
        Texture defaultTex = AssAPI.GetDefaultTextureSetFor(type).GetRandomTexture((TextureType)tIndex);
        if (defaultTex == null) return;
        
        properties.SetTexture(TEXProperties[tIndex], defaultTex);
    }

    // ReSharper disable once UnusedParameter.Local
    private static void ApplyShaderTo(AdvancedSkin advancedSkin, StructureFlags type, AssShader shader)
    {
        if (shader.shaders == null)
        {
            ASS.WarnVerbose("Could not find material list on AssShader, gave up executing ApplyShaderTo()");
            return;
        }
        MeshRenderer mr = advancedSkin.MeshRenderer;

        Material[] materials = GetMaterials(shader, mr);
        if (materials.Length <= 0)
        {
            ASS.WarnVerbose("AssShader returned no materials. Gave up executing ApplyShaderTo()");
            return;
        }

        mr.materials = materials;
        
        advancedSkin.RefreshEnabledShaderFeatures();
        advancedSkin.currentShader = shader;
    }

    private static Material[] GetMaterials(AssShader shader, MeshRenderer meshRenderer)
    {
        List<Material> materials = new List<Material>();

        foreach (ShaderManifest manifest in shader.shaders)
        {
            Material newMat = new Material(manifest.material);
            
            newMat.CopyMatchingPropertiesFromMaterial(meshRenderer.materials[0]);
            materials.Add(newMat);
        }
        
        return materials.ToArray();
    }

    private static void ApplyOverridesTo(AdvancedSkin advancedSkin, StructureFlags type, AssShader shader)
    {
        List<ShaderManifest> manifests = shader.shaders;
        if (manifests == null) return;

        MeshRenderer mr = advancedSkin.MeshRenderer;

        foreach (ShaderManifest manifest in manifests)
        {
            for (int i = 0; i < manifest.overrides.Count; i++)
            {
                if (manifest.overrides[i] == null) continue;
                if ((manifest.overrides[i].targetStructures & type) == 0) continue;
                if (mr.materials.Count < i) continue;
                if (mr.materials[i] == null) continue;
                
                switch (manifest.overrides[i].propertyType)
                {
                    case ShaderPropertyType.Color:
                        mr.materials[i].SetColor(manifest.overrides[i].propertyName, manifest.overrides[i].colorValue);
                        break;
                    case ShaderPropertyType.Vector:
                        mr.materials[i].SetVector(manifest.overrides[i].propertyName, manifest.overrides[i].vectorValue);
                        break;
                    case ShaderPropertyType.Float:
                    case ShaderPropertyType.Range:
                        mr.materials[i].SetFloat(manifest.overrides[i].propertyName, manifest.overrides[i].floatValue);
                        break;
                    case ShaderPropertyType.Texture:
                        mr.materials[i].SetTexture(manifest.overrides[i].propertyName, manifest.overrides[i].textureValue);
                        break;
                    case ShaderPropertyType.Int:
                        mr.materials[i].SetInt(manifest.overrides[i].propertyName, manifest.overrides[i].intValue);
                        break;
                }
            }
        }
    }
}