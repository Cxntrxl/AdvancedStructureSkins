using AdvancedStructureSkins.ShaderFeatures;
using AdvancedStructureSkins.Skins;
using UnityEngine.Rendering;

namespace ShiftstoneStructureSkins.Charge;

public class TimeSinceChargingStarted : TimedShaderFeature
{
    public TimeSinceChargingStarted(AdvancedSkin target) : base(target, "_timeSinceChargingStarted") { }
}