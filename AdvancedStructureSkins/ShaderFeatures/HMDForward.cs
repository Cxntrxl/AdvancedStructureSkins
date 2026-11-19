using AdvancedStructureSkins.Skins;
using RumbleModdingAPI.RMAPI;
using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

// ReSharper disable once InconsistentNaming
public class HMDForward : ShaderFeature
{
    public HMDForward(AdvancedSkin target) : base(target, ShaderPropertyType.Vector, "_HMDForward", Vector3.forward) { }
    
    protected override void OnUpdate()
    {
        base.OnUpdate();
        SetProperty(Calls.Players.GetLocalPlayerController().PlayerVR.Headset.Transform.forward);
    }
}