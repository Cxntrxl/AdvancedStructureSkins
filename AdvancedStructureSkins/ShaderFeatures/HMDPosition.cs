using AdvancedStructureSkins.Skins;
using RumbleModdingAPI.RMAPI;
using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

// ReSharper disable once InconsistentNaming
public class HMDPosition : ShaderFeature
{
    public HMDPosition(AdvancedSkin target) : base(target, ShaderPropertyType.Vector, "_HMDPosition", Vector3.zero) { }
    
    protected override void OnUpdate()
    {
        base.OnUpdate();
        SetProperty(Calls.Players.GetLocalPlayerController().PlayerVR.Headset.Transform.position);
    }
}