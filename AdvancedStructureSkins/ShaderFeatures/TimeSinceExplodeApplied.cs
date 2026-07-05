using AdvancedStructureSkins.Skins;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

public class TimeSinceExplodeApplied : TimedShaderFeature
{
    public TimeSinceExplodeApplied(AdvancedSkin target) : base(target, "_timeSinceExplodeApplied") { }
}