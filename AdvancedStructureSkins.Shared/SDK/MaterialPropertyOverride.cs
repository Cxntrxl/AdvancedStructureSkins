using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.Shared.SDK
{
    [Serializable]
    public class MaterialPropertyOverride
    {
        public string propertyName = "";
        public ShaderPropertyType propertyType;
        public Color colorValue;
        public float floatValue;
        public int intValue;
        public Vector4 vectorValue;
        public Texture textureValue;
        public StructureFlags targetStructures;
    }

    public enum StructureType
    {
        Disc, Pillar, Ball, Cube, Wall, SmallRock, LargeRock
    }
}

