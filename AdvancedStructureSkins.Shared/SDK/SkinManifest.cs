using System.Collections.Generic;
using UnityEngine;

namespace AdvancedStructureSkins.Shared.SDK
{
    public class SkinManifest : ScriptableObject
    {
        public string skinName;
        
        public Texture previewTexture;
        public bool allowedInComp;
        public List<ShaderManifest> shaders;
        public List<TextureSetManifest> textures;
    }
}

