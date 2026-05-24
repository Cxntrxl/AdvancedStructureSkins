using System;
using System.Linq;
using UnityEngine;

namespace AdvancedStructureSkins.Shared.SDK.Binary
{
    [Serializable]
    public class SkinManifestBinary
    {
        public int version = 1;
        public string skinName;
        public string materialName;
        public string previewTextureName;
        public bool allowedInComp;
        public MaterialOverrideBinary[] overrides;
        public TextureSetBinary[] textures;

        public SkinManifest GetSkinManifestFromBundle(AssetBundle bundle)
        {
            SkinManifest manifest = ScriptableObject.CreateInstance<SkinManifest>();
            manifest.skinName = skinName;
            manifest.material = GetMaterialFromBundle(bundle);
            manifest.previewTexture = GetTextureFromBundle(bundle);
            manifest.allowedInComp = allowedInComp;
            manifest.overrides = overrides.Select(o => o.GetOverrideFromBundle(bundle)).ToList();
            manifest.textures = textures.Select(t => t.GetManifestFromBundle(bundle)).ToList();
            manifest.textures.ForEach(t => { if (t.previewTexture == null) t.previewTexture = manifest.previewTexture; });
            
            return manifest;
        }

        public Material GetMaterialFromBundle(AssetBundle bundle)
        {
            foreach (var name in bundle.GetAllAssetNames())
            {
                if (name.EndsWith(materialName + ".mat", StringComparison.OrdinalIgnoreCase) ||
                    name.EndsWith(materialName, StringComparison.OrdinalIgnoreCase))
                {
                    return bundle.LoadAsset<Material>(name);
                }
            }

            return null;
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
}

