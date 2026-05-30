using AdvancedStructureSkins.Skins;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

public class TimeSinceGroundStateChanged : TimedShaderFeature
{
    public TimeSinceGroundStateChanged(AdvancedSkin target) : base(target, ShaderPropertyType.Float, "_timeSinceGroundStateChanged") { }
}