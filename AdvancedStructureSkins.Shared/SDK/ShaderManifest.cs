using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedStructureSkins.Shared.SDK
{
    [Serializable]
    public class ShaderManifest
    {
        public Material material;
        public List<MaterialPropertyOverride> overrides;
    }
}