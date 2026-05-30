using AdvancedStructureSkins.Skins;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

public class TimeSinceLastModified : TimedShaderFeature
{
    public TimeSinceLastModified(AdvancedSkin target) : base(target, ShaderPropertyType.Float, "_timeSinceLastModified") { }
}