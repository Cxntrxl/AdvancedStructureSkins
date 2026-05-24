using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedStructureSkins.Shared.SDK
{
    [Serializable]
    public class TextureSetManifest
    {
        public string name;
        public Texture previewTexture;
        public List<TextureEntry> textures = new List<TextureEntry>();
        public StructureFlags targets;
    }

    [Serializable]
    public class TextureEntry
    {
        public TextureType type;
        public List<Texture> textures;
    }
}

