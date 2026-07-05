using AdvancedStructureSkins.Skins;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

public class IsExplodeApplied : ShaderFeature
{
    public IsExplodeApplied(AdvancedSkin target) : base(target, ShaderPropertyType.Float, "_isExplodeApplied", 0f) { }
}