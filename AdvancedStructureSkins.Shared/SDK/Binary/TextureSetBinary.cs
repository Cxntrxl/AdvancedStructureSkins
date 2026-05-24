using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdvancedStructureSkins.Shared.SDK.Binary
{
    public class TextureSetBinary
    {
        public string name;
        public string previewTextureName;
        public TextureEntryBinary[] textures;
        public StructureFlags targets;

        public TextureSetManifest GetManifestFromBundle(AssetBundle bundle)
        {
            TextureSetManifest result = new TextureSetManifest
            {
                name = name,
                previewTexture = GetPreviewTextureFromBundle(bundle),
                targets = targets,
                textures = new List<TextureEntry>()
            };

            foreach (var entry in textures)
            {
                TextureEntry e = new TextureEntry
                {
                    type = entry.type,
                    textures = new List<Texture>()
                };

                foreach (var assetName in entry.textures)
                {
                    if (string.IsNullOrEmpty(assetName))
                        continue;

                    var reference = bundle.GetAllAssetNames()
                        .FirstOrDefault(n =>
                            n.EndsWith(assetName, StringComparison.OrdinalIgnoreCase));

                    if (reference == null)
                        continue;

                    var tex = bundle.LoadAsset<Texture>(reference);
                    if (tex != null)
                        e.textures.Add(tex);
                }

                result.textures.Add(e);
            }

            return result;
        }
        
        public Texture GetPreviewTextureFromBundle(AssetBundle bundle)
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

    public class TextureEntryBinary
    {
        public TextureType type;
        public string[] textures;
    }
}

