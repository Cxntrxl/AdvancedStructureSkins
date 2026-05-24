using AdvancedStructureSkins.Skins;

namespace AdvancedStructureSkins.ShaderFeatures;

public class TimeSinceLocalDamageTaken : TimedShaderFeature
{
    public TimeSinceLocalDamageTaken(AdvancedSkin target) : base(target, "_timeSinceLocalDamageTaken") { }
}