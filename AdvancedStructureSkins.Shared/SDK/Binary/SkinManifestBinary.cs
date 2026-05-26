using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdvancedStructureSkins.Shared.SDK.Binary
{
    [Serializable]
    public class SkinManifestBinary
    {
        public string skinName;
        public string previewTextureName;
        public bool allowedInComp;
        public ShaderManifestBinary[] shaders;
        public TextureSetBinary[] textures;

        public SkinManifest GetSkinManifestFromBundle(AssetBundle bundle)
        {
            SkinManifest manifest = ScriptableObject.CreateInstance<SkinManifest>();
            manifest.skinName = skinName;
            manifest.previewTexture = GetTextureFromBundle(bundle);
            manifest.allowedInComp = allowedInComp;
            manifest.shaders = GetShaderManifestListFromBundle(bundle);
            manifest.textures = textures.Select(t => t.GetManifestFromBundle(bundle)).ToList();
            manifest.textures.ForEach(t => { if (t.previewTexture == null) t.previewTexture = manifest.previewTexture; });
            
            return manifest;
        }

        public List<ShaderManifest> GetShaderManifestListFromBundle(AssetBundle bundle)
        {
            List<ShaderManifest> result = new List<ShaderManifest>();

            foreach (ShaderManifestBinary manifest in shaders) result.Add(manifest.GetShaderManifestFromBundle(bundle));
            
            return result;
        }
        
        public Texture GetTextureFromBundle(AssetBundle bundle)
        {
            var reference = bundle.GetAllAssetNames()
                .FirstOrDefault(n =>
                    n.EndsWith(previewTextureName, StringComparison.OrdinalIgnoreCase));

            if (reference == null)
                return null;

            var tex = bundle.LoadAsset<Texture>(reference);
            if (tex == null) return null;
            
            tex.hideFlags = HideFlags.HideAndDontSave;
            return tex;
        }
    }

    [Serializable]
    public class MaterialOverrideBinaryArray
    {
        public MaterialOverrideBinary[] overrides;

        public MaterialPropertyOverrideManifest GetOverrideManifestFromBundle(AssetBundle bundle)
        {
            return new MaterialPropertyOverrideManifest()
                { overrides = overrides.Select(ob => ob.GetOverrideFromBundle(bundle)).ToList() };
        }
    }
}

