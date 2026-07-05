using AdvancedStructureSkins.ShaderFeatures;
using AdvancedStructureSkins.Skins;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.Shiftstones.Charge;

public class TimeSinceChargingStarted : TimedShaderFeature
{
    public TimeSinceChargingStarted(AdvancedSkin target) : base(target, "_timeSinceChargingStarted") { }
}