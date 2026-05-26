using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdvancedStructureSkins.Shared.SDK.Binary
{
    public class ShaderManifestBinary
    {
        public string materialName;
        public MaterialOverrideBinary[] overrides;

        public ShaderManifest GetShaderManifestFromBundle(AssetBundle bundle)
        {
            ShaderManifest result = new ShaderManifest();
            
            var mref = bundle.GetAllAssetNames()
                .FirstOrDefault(n =>
                    n.EndsWith(materialName, StringComparison.OrdinalIgnoreCase));

            if (mref == null)
                return null;

            var mat = bundle.LoadAsset<Material>(mref);
            if (mat == null) return null;
            
            mat.hideFlags = HideFlags.DontUnloadUnusedAsset;
            result.material = mat;
            result.overrides = new List<MaterialPropertyOverride>();

            foreach (var o in overrides)
            {
                result.overrides.Add(o.GetOverrideFromBundle(bundle));
            }

            return result;
        }
    }
}