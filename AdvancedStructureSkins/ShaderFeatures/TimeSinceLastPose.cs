using AdvancedStructureSkins.Skins;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

public class TimeSinceLastPose : TimedShaderFeature
{
    public TimeSinceLastPose(AdvancedSkin target) : base(target, ShaderPropertyType.Float, "_timeSinceLastPose") { }
}