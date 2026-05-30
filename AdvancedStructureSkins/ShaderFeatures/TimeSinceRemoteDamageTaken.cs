using AdvancedStructureSkins.Skins;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

public class TimeSinceRemoteDamageTaken : TimedShaderFeature
{
    public TimeSinceRemoteDamageTaken(AdvancedSkin target) : base(target, ShaderPropertyType.Float, "_timeSinceRemoteDamageTaken") { }
}