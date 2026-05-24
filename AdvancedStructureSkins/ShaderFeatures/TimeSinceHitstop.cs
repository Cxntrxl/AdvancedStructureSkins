using AdvancedStructureSkins.Skins;

namespace AdvancedStructureSkins.ShaderFeatures;

public class TimeSinceHitstop : TimedShaderFeature
{
    public TimeSinceHitstop(AdvancedSkin target) : base(target, "_timeSinceHitstop") { }
}