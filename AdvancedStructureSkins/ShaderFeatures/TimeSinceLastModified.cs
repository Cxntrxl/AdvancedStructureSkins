using AdvancedStructureSkins.Skins;

namespace AdvancedStructureSkins.ShaderFeatures;

public class TimeSinceLastModified : TimedShaderFeature
{
    public TimeSinceLastModified(AdvancedSkin target) : base(target, "_timeSinceLastModified") { }
}