using AdvancedStructureSkins.Skins;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

public class TimeSinceHitByExplosion : TimedShaderFeature
{
    public TimeSinceHitByExplosion(AdvancedSkin target) : base(target, "_timeSinceHitByExplosion") { }
}