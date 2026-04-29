using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.Skins;

[Serializable]
public class MaterialPropertyOverrides
{
    public List<MaterialPropertyOverride> overrides = new();
}

[Serializable]
public class MaterialPropertyOverride
{
    public string propertyName;
    public int propertyType;
    public Color colorValue;
    public float floatValue;
    public int intValue;
    public Vector4 vectorValue;
    //public Texture textureValue;
    public List<int> targetStructures = new();
}

public enum StructureType
{
    Disc, Pillar, Ball, Cube, Wall, SmallRock, LargeRock
}