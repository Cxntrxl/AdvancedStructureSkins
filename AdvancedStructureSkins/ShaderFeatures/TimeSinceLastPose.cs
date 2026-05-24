using AdvancedStructureSkins.Skins;

namespace AdvancedStructureSkins.ShaderFeatures;

public class TimeSinceLastPose : TimedShaderFeature
{
    public TimeSinceLastPose(AdvancedSkin target) : base(target, "_timeSinceLastPose") { }
}