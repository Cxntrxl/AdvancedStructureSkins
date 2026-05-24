using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Color = UnityEngine.Color;

namespace AdvancedStructureSkins.Shared.SDK.Binary
{
    public class MaterialOverrideBinary
    {
        public string propertyName;
        public ShaderPropertyType type;
        public float floatValue;
        public int intValue;
        public Color colorValue;
        public Vector4 vectorValue;
        public string textureValue;
        public StructureFlags targetStructures;

        public MaterialPropertyOverride GetOverrideFromBundle(AssetBundle bundle)
        {
            MaterialPropertyOverride result = new MaterialPropertyOverride()
            {
                propertyName = propertyName,
                propertyType = type,
                targetStructures = targetStructures
            };

            switch (type)
            {
                case ShaderPropertyType.Color:
                    result.colorValue = colorValue;
                    break;
                case ShaderPropertyType.Vector:
                    result.vectorValue = vectorValue;
                    break;
                case ShaderPropertyType.Float:
                case ShaderPropertyType.Range:
                    result.floatValue = floatValue;
                    break;
                case ShaderPropertyType.Texture:
                    result.textureValue = GetTextureFromBundle(bundle);
                    break;
                case ShaderPropertyType.Int:
                    result.intValue = intValue;
                    break;
            }

            return result;
        }

        public Texture GetTextureFromBundle(AssetBundle bundle)
        {
            if (string.IsNullOrEmpty(textureValue))
                return null;

            var reference = bundle.GetAllAssetNames()
                .FirstOrDefault(n =>
                    n.EndsWith(textureValue, StringComparison.OrdinalIgnoreCase));

            if (reference == null)
                return null;

            return bundle.LoadAsset<Texture>(reference);
        }
    }
}

