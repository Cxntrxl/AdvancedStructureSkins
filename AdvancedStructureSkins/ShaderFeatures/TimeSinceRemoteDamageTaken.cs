using AdvancedStructureSkins.Skins;

namespace AdvancedStructureSkins.ShaderFeatures;

public class TimeSinceRemoteDamageTaken : TimedShaderFeature
{
    public TimeSinceRemoteDamageTaken(AdvancedSkin target) : base(target, "_timeSinceRemoteDamageTaken") { }
}