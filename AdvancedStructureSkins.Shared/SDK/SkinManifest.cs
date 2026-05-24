using System.Collections.Generic;
using UnityEngine;

namespace AdvancedStructureSkins.Shared.SDK
{
    public class SkinManifest : ScriptableObject
    {
        public string skinName;

        public Material material;
        public Texture previewTexture;
        public bool allowedInComp;
        public List<MaterialPropertyOverride> overrides;
        public List<TextureSetManifest> textures;
    }
}

