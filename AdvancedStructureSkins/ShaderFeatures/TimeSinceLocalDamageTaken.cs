using AdvancedStructureSkins.Skins;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

public class TimeSinceLocalDamageTaken : TimedShaderFeature
{
    public TimeSinceLocalDamageTaken(AdvancedSkin target) : base(target, ShaderPropertyType.Float, "_timeSinceLocalDamageTaken") { }
}