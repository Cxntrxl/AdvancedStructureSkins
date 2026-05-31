using AdvancedStructureSkins.Skins;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

public class LastExplosionStrength : ShaderFeature
{
    public LastExplosionStrength(AdvancedSkin target) : base(target, ShaderPropertyType.Float, "_lastExplosionStrength", 0) { }
}