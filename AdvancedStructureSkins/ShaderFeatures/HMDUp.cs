using AdvancedStructureSkins.Skins;
using RumbleModdingAPI.RMAPI;
using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

// ReSharper disable once InconsistentNaming
public class HMDUp : ShaderFeature
{
    public HMDUp(AdvancedSkin target) : base(target, ShaderPropertyType.Vector, "_HMDUp", Vector3.up) { }
    
    protected override void OnUpdate()
    {
        base.OnUpdate();
        SetProperty(Calls.Players.GetLocalPlayerController().PlayerVR.Headset.Transform.up);
    }
}