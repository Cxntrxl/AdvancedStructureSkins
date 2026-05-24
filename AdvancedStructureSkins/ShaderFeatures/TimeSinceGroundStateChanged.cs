using AdvancedStructureSkins.Skins;

namespace AdvancedStructureSkins.ShaderFeatures;

public class TimeSinceGroundStateChanged : TimedShaderFeature
{
    public TimeSinceGroundStateChanged(AdvancedSkin target) : base(target, "_timeSinceGroundStateChanged") { }
}